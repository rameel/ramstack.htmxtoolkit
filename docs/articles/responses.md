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

The callback runs only when `Request.IsHtmxRequest()` is `true`, so not for history restoration. An action that also handles normal navigation can use separate results:

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

| Goal                                | API                    | Header               |
|-------------------------------------|------------------------|----------------------|
| Request another URL through HTMX    | `Location`             | `HX-Location`        |
| Navigate with a full page load      | `Redirect`             | `HX-Redirect`        |
| Reload the current page             | `Refresh`              | `HX-Refresh`         |
| Add a browser history entry         | `PushUrl`              | `HX-Push-Url`        |
| Replace the current history entry   | `ReplaceUrl`           | `HX-Replace-Url`     |
| Leave the browser history unchanged | `PreventHistoryUpdate` | `HX-Push-Url: false` |
| Change the swap target              | `Retarget`             | `HX-Retarget`        |
| Change the swap strategy            | `Reswap`               | `HX-Reswap`          |
| Select part of the response         | `Reselect`             | `HX-Reselect`        |
| Dispatch client events              | `TriggerEvent`         | `HX-Trigger*`        |

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

The object is serialized the way ASP.NET Core serializes JSON responses by default: property names are camel-cased,
while dictionary keys and `null` values are written as is. `Id` becomes `id` in `event.detail`:

```javascript
document.addEventListener("product-saved", event => {
    console.log("Saved product:", event.detail.id);
});
```

Only property names in the detail are camel-cased. The event name is sent exactly as passed to `TriggerEvent`,
and browser event names are case-sensitive.

Put the listener in a JavaScript file loaded once by the layout, after HTMX. Listening on `document`
keeps it active when a form or component is replaced. Avoid registering it again inside each partial.
The `hx-trigger` listener can still refresh a component while JavaScript handles the event data.

### Timing and repeated events

HTMX 1.9.x and 2.x distinguish receive, after-swap, and after-settle response headers.
HTMX 4.x delivers these Toolkit events through `HX-Trigger` when the request completes,
after the swap and settle steps when a swap occurs.
See [Version compatibility](version-compatibility.md#response-events).

Calling `TriggerEvent` several times adds events to the same response, and the browser receives the events
of each header in call order. A JSON header cannot hold the same name twice, so when a name repeats within
one header, the [Toolkit script](antiforgery.md#what-the-toolkit-script-does) delivers all events of that header in order.
Keep the script loaded when using repeated event names: without it, none of the events
in that header are triggered. In HTMX 4.x all timings share `HX-Trigger`, so this also applies to a name
added with different timings.

### Serialize details with source-generated metadata

The object overload discovers the shape of each detail type through reflection the first time it serializes that type.
Pass source-generated JSON metadata to do this work at build time instead: the first event of each type is serialized
without the reflection delay, the payload can be written by generated code, and the app needs less memory.
Because no reflection is involved, this overload also supports trimming and Native AOT.

Each payload needs a named type listed in the JSON context, so anonymous objects cannot be used.
Define the payload and the context in their own C# file:

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

Response headers allow only ASCII characters. Other characters in string values and dictionary keys
are escaped as `\uXXXX`, but property names and enum names are written as the encoder of the metadata
options escaped them. `AppJsonContext.Default` uses the default encoder, which escapes these names as well.
If you take the metadata from options with a relaxed encoder, such as Minimal API `JsonOptions`,
which use `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`, keep these names ASCII-only;
otherwise, the server rejects the header.

### Serialize details with custom options

The object overload always uses the defaults described above, not the JSON options configured for the application.
To serialize a detail with other settings, for example with the application's options that write enums as strings,
pass `JsonSerializerOptions`:

```csharp
public sealed class ProductsController(IOptions<JsonOptions> jsonOptions) : Controller
{
    [HttpPost]
    public IActionResult Save(Product product)
    {
        Response.Htmx(htmx => htmx.TriggerEvent(
            "product-saved",
            new { product.Id, product.Status },
            jsonOptions.Value.JsonSerializerOptions));

        return PartialView("_ProductRow", product);
    }
}
```

`JsonOptions` here is `Microsoft.AspNetCore.Mvc.JsonOptions`. In Minimal APIs, use `SerializerOptions`
of `Microsoft.AspNetCore.Http.Json.JsonOptions`. Like the object overload, this overload uses reflection;
for trimming and Native AOT, use [source-generated metadata](#serialize-details-with-source-generated-metadata).

The detail is always written as compact JSON, with non-ASCII characters in string values and dictionary keys
escaped as described above. Minimal API `JsonOptions` use a relaxed encoder, so with them, keep property names
and enum names ASCII-only.

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

Apply the attribute to an action, a controller, or a Razor Pages page model.
Razor Pages ignore filter attributes on handler methods, so put it on the page model class.
When a controller and its action specify the same header, the action's value takes effect.
A derived class inherits the attribute from its base class; an attribute declared on the derived class
replaces the inherited one entirely.

The attribute sets its headers before the action or page handler runs, so it works as a default.
The code can replace a header with `Response.Htmx(...)` or remove it through `Response.GetHtmxHeaders()`,
for example to retarget validation errors.

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
Assigning `null` or an empty string to a string property, such as `Retarget`, removes its header.

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
