# Control HTMX responses

HTMX response headers let the server change the target, swap strategy, browser URL, navigation,
or client-side events for a particular response.

## Configure a response fluently

```csharp
Response.Htmx(response => response
    .Retarget("#content")
    .Reswap(HtmxSwap.InnerHtml)
    .TriggerEvent("content-updated"));

return PartialView("_Content", model);
```

This example assumes a fragment endpoint. `Response.Htmx(...)` changes headers, not the result body or status code;
choose `PartialView`, `View`, or another result to match the request.

The callback runs only for an HTMX request. An action that also handles normal navigation can use separate results:

```csharp
var url = Url.Action("Index", "Dashboard")!;

if (Request.IsHtmxRequest())
{
    Response.Htmx(htmx => htmx.Redirect(url));
    return Ok();
}

return Redirect(url);
```

An ordinary browser request receives the normal ASP.NET Core redirect.
An HTMX request receives a 200 response with `HX-Redirect`.

> [!IMPORTANT]
> Do not return an ASP.NET Core 3xx redirect with an `HX-*` response header. The browser follows the redirect internally,
> so HTMX receives the final response and cannot process headers from the intermediate 3xx response.
> Return a 2xx response such as `Ok()` for the HTMX path.

The API is available from any `HttpResponse`, including Minimal API handlers:

```csharp
app.MapPost("/account/sign-in", (HttpResponse response, LinkGenerator links) =>
{
    var url = links.GetPathByName(response.HttpContext, "dashboard")!;

    response.Htmx(htmx => htmx.Redirect(url));
    return Results.Ok();
});
```

## Choose a response instruction

| Goal | API | Header |
|---|---|---|
| Request another URL through HTMX | `Location` | `HX-Location` |
| Navigate with a full page load | `Redirect` | `HX-Redirect` |
| Reload the current page | `Refresh` | `HX-Refresh` |
| Add a browser history entry | `PushUrl` | `HX-Push-Url` |
| Replace the current history entry | `ReplaceUrl` | `HX-Replace-Url` |
| Leave the browser history unchanged | `PreventHistoryUpdate` | `HX-Push-Url: false` |
| Change the swap target | `Retarget` | `HX-Retarget` |
| Change the swap strategy | `Reswap` | `HX-Reswap` |
| Select part of the response | `Reselect` | `HX-Reselect` |
| Dispatch client events | `TriggerEvent` | `HX-Trigger*` |

> [!TIP]
> `Location`, `Redirect`, `PushUrl`, and `ReplaceUrl` take a URL string. Generate it with `Url.Action`, `Url.Page`,
> or `LinkGenerator` instead of writing the path by hand, so that the URL follows route changes and `PathBase`.

### Location or redirect

Use `Location` to issue another HTMX request and swap its response without a full-page reload.
Supply `HtmxLocationOptions` when the follow-up request needs a target, swap, selection,
values, headers, or history behavior. For example, load a details fragment into the page,
but show the product page URL in the address bar:

```csharp
Response.Htmx(htmx => htmx.Location(
    Url.Action(nameof(Details), new { id = product.Id })!,
    new HtmxLocationOptions
    {
        Target = "#product-details",
        Swap = HtmxSwap.OuterHtml,
        HistoryUrl = Url.Action(nameof(Show), new { id = product.Id })
    }));
```

A reload or a shared link then opens the `Show` URL, so that URL must render the full page.

Use `SwapExpression` instead of `Swap` for a complete expression with swap modifiers,
such as `outerHTML settle:200ms`.

`HistoryAction` defaults to `HtmxHistoryAction.Push`, which adds a history entry.
`HistoryUrl` sets the URL for that entry; when it is `null`, HTMX uses the URL of the loaded page.
To replace the current entry, select `HtmxHistoryAction.Replace`:

```csharp
Response.Htmx(htmx => htmx.Location(Url.Action("Index", "Dashboard")!, new HtmxLocationOptions
{
    HistoryAction = HtmxHistoryAction.Replace
}));
```

Use `HtmxHistoryAction.None` to request no history update; `HistoryUrl` is ignored in this mode.
Custom history URLs and `None` require HTMX 2.0.8 or later 2.x, or HTMX 4.x;
`Replace` requires HTMX 2.0.9 or later 2.x, or HTMX 4.x.
In HTMX 2.x, a boosted element specified as `Source` can still add a history entry in `None` mode.
History response headers from the loaded page can override the requested action.

Use `Redirect` when the browser must perform a normal navigation, for example after authentication
or when the next page depends on a full document load. The response carrying `HX-Redirect`
must use a non-redirect status such as 200.

### History updates

```csharp
Response.Htmx(htmx => htmx.PushUrl(Url.Action(nameof(Show), new { id = product.Id })!));
```

`PushUrl` adds a history entry; `ReplaceUrl` changes the current one, which suits search-as-you-type
results that should not add an entry for every keystroke. `PreventHistoryUpdate()` leaves the history unchanged,
even when `hx-push-url`, `hx-replace-url`, or boosted navigation requests an update. Use it, for example,
when a boosted form returns validation errors, so the address bar keeps the form's URL instead of the POST URL.
Each method removes the header set by the others, so the last call takes effect.

### Targets, selection, and swaps

The server can send validation errors to a different target than the successful response:

```csharp
if (!ModelState.IsValid)
{
    Response.Htmx(htmx => htmx
        .Retarget("#validation-errors")
        .Reswap(HtmxSwap.InnerHtml));

    return PartialView("_ValidationSummary", ModelState);
}
```

The validation partial above returns status 200. If you use 422 instead, also enable swapping that status in
[response handling](configuration-v2.md#handle-validation-responses); `Retarget` and `Reswap` do not override the
HTMX 2.x default of leaving error responses unswapped.

Use the typed `HtmxSwap` overload for a swap style. Use the string overload for a full expression containing modifiers:

```csharp
Response.Htmx(htmx => htmx.Reswap("innerHTML show:#result:top"));
```

`Reselect("#result")` selects only the matching part of a larger response before it is swapped.

## Trigger client events

### Refresh another component

Dispatch an event after saving so another element can refresh itself. No event data is needed when
the listener will request its latest HTML from the server:

```csharp
Response.Htmx(htmx => htmx.TriggerEvent(
    "product-saved",
    HtmxTriggerTiming.AfterSwap));
```

```html
<aside hx-get="/products/summary"
       hx-trigger="product-saved from:body">
</aside>
```

Keep the listener outside any element that the save replaces. `from:body` lets it hear an event from
another element. Give it both a request URL and a trigger.
For a complete form, save handler, and listening component, see [Forms, validation, and events](forms-and-validation.md).

### Read event data in JavaScript

Pass an object when a browser listener needs information from the response:

```csharp
Response.Htmx(htmx => htmx.TriggerEvent(
    "product-saved",
    new { product.Id },
    HtmxTriggerTiming.AfterSwap));
```

The object is serialized with camelCase property names, so `Id` becomes `id` in `event.detail`:

```javascript
document.addEventListener("product-saved", event => {
    console.log("Saved product:", event.detail.id);
});
```

Only the detail uses camelCase. The event name is sent exactly as passed to `TriggerEvent`,
and browser event names are case-sensitive.

Put the listener in a JavaScript file loaded once by the layout, after HTMX. Listening on `document`
keeps it active when a form or component is replaced. Avoid registering it again inside each partial.
The `hx-trigger` listener can still refresh a component while JavaScript handles the event data.

### Timing and repeated events

HTMX 1.x and 2.x distinguish receive, after-swap, and after-settle response headers.
HTMX 4.x delivers these Toolkit events through `HX-Trigger` when the request completes, after the swap when one occurs.
See [Version compatibility](version-compatibility.md#response-events).

Calling `TriggerEvent` several times adds events to the same response, and the browser receives them in call order.
A JSON header cannot hold the same name twice, so when a name repeats within one header, all events of that header
are sent under the reserved `rs:event` name and the [Toolkit script](antiforgery.md#what-the-toolkit-script-does)
replays them in order. Keep the script loaded when using repeated event names: without it, none of the events
in that header are triggered. Do not use the name `rs:event`.

### Serialize details with Native AOT

The object overload uses reflection to serialize details. For trimming and Native AOT,
pass source-generated JSON metadata instead. Define the payload and JSON context in their own C# file:

```csharp
using System.Text.Json.Serialization;

internal sealed record ProductSaved(int Id);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ProductSaved))]
internal partial class AppJsonContext : JsonSerializerContext;
```

Then use that context in the handler:

```csharp
var detail = new ProductSaved(product.Id);

Response.Htmx(
    static (htmx, detail) => htmx.TriggerEvent(
        "product-saved",
        detail,
        AppJsonContext.Default.ProductSaved,
        HtmxTriggerTiming.AfterSwap),
    detail);
```

The JSON naming policy keeps the payload the same, so the JavaScript listener above still reads `event.detail.id`.

## Configure declaratively in MVC

`HtmxResponseAttribute` is convenient when response behavior is constant:

```csharp
[HtmxRequest]
[HtmxResponse(
    Retarget = "#inventory-status",
    Reswap = HtmxSwap.OuterHtml)]
public IActionResult Inventory(string sku) =>
    PartialView("_Inventory", inventory.Find(sku));
```

The attribute supports `Refresh`, `Reswap`, `ReswapExpression`, `Retarget`, and `Reselect`.
Use the fluent API when values depend on runtime state or when triggering events and navigation.

## Access headers directly

Use `Response.GetHtmxHeaders()` when middleware or shared infrastructure needs property access instead of fluent chaining:

```csharp
var htmx = Response.GetHtmxHeaders();
htmx.Retarget = "#notice";
htmx.Reswap = HtmxSwap.OuterHtml;
```

Unlike `Response.Htmx(...)`, direct header access is not conditional: the caller decides whether the headers should be written.
The properties can also be read, for example by a filter that sets a default `Retarget` only when the action
has not chosen one.

`HtmxResponseHeaderNames` exposes header-name constants for lower-level integrations.

## Avoid closures in a hot path

The generic overload passes state explicitly, so a `static` lambda does not capture variables
and no closure is allocated:

```csharp
var url = Url.Action(nameof(Show), new { id = product.Id })!;

Response.Htmx(static (htmx, url) => htmx.PushUrl(url), url);
```

Use it when allocation measurements justify the extra syntax; the ordinary overload is clearer for most endpoints.

See [Common recipes](recipes.md) for validation, redirects, events, and polling in context.
