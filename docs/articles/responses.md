# HTMX responses

HTMX response headers let the server change the target, swap strategy, browser URL, navigation,
or client-side events for a particular response.

## Configure a response fluently

```csharp
Response.Htmx(response => response
    .Retarget("#content")
    .Reswap(HtmxSwap.InnerHtml)
    .TriggerEvent("content-updated"));

return View();
```

The callback runs only for an HTMX request. This makes it safe for an action that also handles normal navigation:

```csharp
if (Request.IsHtmxRequest())
{
    Response.Htmx(htmx => htmx.Redirect("/dashboard"));
    return Ok();
}

return Redirect("/dashboard");
```

An ordinary browser request receives the normal ASP.NET Core redirect.
An HTMX request receives a 200 response with `HX-Redirect`.

> [!IMPORTANT]
> Do not return an ASP.NET Core 3xx redirect with an `HX-*` response header. The browser follows the redirect internally,
> so HTMX receives the final response and cannot process headers from the intermediate 3xx response.
> Return a 2xx response such as `Ok()` for the HTMX path.

The API is available from any `HttpResponse`, including Minimal API handlers:

```csharp
app.MapPost("/account/sign-in", (HttpResponse response) =>
{
    response.Htmx(htmx => htmx.Redirect("/dashboard"));
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
| Change the swap target | `Retarget` | `HX-Retarget` |
| Change the swap strategy | `Reswap` | `HX-Reswap` |
| Select part of the response | `Reselect` | `HX-Reselect` |
| Dispatch client events | `TriggerEvent(s)` | `HX-Trigger*` |

### Location or redirect

Use `Location` to issue another HTMX request and swap its response without a full-page reload.
Supply `HtmxLocationOptions` when the follow-up request needs a target, swap, selection,
values, headers, or history behavior:

```csharp
Response.Htmx(htmx => htmx.Location(
    $"/products/{product.Id}",
    new HtmxLocationOptions
    {
        Source = "#save-product",
        Target = "#product-details",
        Swap = HtmxSwap.OuterHtml,
        Select = "#product-details",
        Push = $"/products/{product.Id}"
    }));
```

Use `Redirect` when the browser must perform a normal navigation, for example after authentication
or when the next page depends on a full document load. The response carrying `HX-Redirect`
must use a non-redirect status such as 200.

### History updates

```csharp
Response.Htmx(htmx => htmx.PushUrl($"/products/{product.Id}"));
```

`PushUrl` adds a history entry; `ReplaceUrl` changes the current one. `PreventPushUrl()`
and `PreventReplaceUrl()` emit `false` to suppress a client history update requested elsewhere.

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

Use the typed `HtmxSwap` overload for a swap style. Use the string overload for a full expression containing modifiers:

```csharp
Response.Htmx(htmx => htmx.Reswap("innerHTML show:#result:top"));
```

`Reselect("#result")` selects only the matching part of a larger response before it is swapped.

## Trigger client events

Dispatch an event after saving so another element can refresh itself:

```csharp
Response.Htmx(htmx => htmx.TriggerEvent(
    "product-saved",
    new { product.Id },
    HtmxTriggerTiming.AfterSwap));
```

```html
<aside hx-get="/products/summary"
       hx-trigger="product-saved from:body">
</aside>
```

HTMX 1.x and 2.x distinguish receive, after-swap, and after-settle response headers.
HTMX 4.x delivers these Toolkit events through `HX-Trigger` when the request completes, after the swap when one occurs.
See [Version compatibility](version-compatibility.md).

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

`HtmxResponseHeaderNames` exposes header-name constants for lower-level integrations.

## Avoid closures in a hot path

The generic overload passes state explicitly:

```csharp
Response.Htmx(
    static (htmx, id) => htmx.TriggerEvent("product-saved", new { id }),
    product.Id);
```

Use it when allocation measurements justify the extra syntax; the ordinary overload is clearer for most endpoints.

See [Common recipes](recipes.md) for validation, redirects, events, and polling in context.
