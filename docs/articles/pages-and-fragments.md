# Full pages and fragments

An HTMX application usually returns a small HTML fragment for an HTMX request and a complete document
for normal browser navigation. Supporting both paths keeps links usable without JavaScript
and gives history restoration a full-page fallback.

## Use one URL for both representations

An MVC action can select the result from the request:

```csharp
public IActionResult Details(int id)
{
    var product = catalog.Get(id);

    if (Request.IsHtmxRequest(out var htmx) && !htmx.HistoryRestoreRequest)
        return PartialView("_ProductDetails", product);

    return View(product);
}
```

The Razor Page equivalent assigns data for a normal page request and returns a partial for HTMX:

```csharp
public IActionResult OnGet(int id)
{
    Product = catalog.Get(id);

    if (Request.IsHtmxRequest(out var htmx) && !htmx.HistoryRestoreRequest)
        return Partial("_ProductDetails", Product);

    return Page();
}
```

When HTMX marks a request with `HX-History-Restore-Request`, return the complete document it expects to restore.
In HTMX 1.x and 2.x this commonly follows a miss in the local snapshot cache.
HTMX 4.x normally retrieves history entries from the server rather than keeping local DOM snapshots;
the optional `hx-history-cache` extension restores snapshot caching.

## Use a dedicated fragment endpoint

A dedicated endpoint is appropriate when the fragment is an operation rather than another representation of a page:

```csharp
public IActionResult OnGetInventory(int productId) =>
    Partial("_Inventory", inventory.Get(productId));
```

```html
<button hx-page="/Products/Details"
        hx-page-handler="Inventory"
        hx-route-productid="@Model.Product.Id"
        hx-target="#inventory">
    Check inventory
</button>
```

This style works well for validation, polling, autocomplete, dialogs, and inline editing.

## Avoid rendering a layout into a target

If a response unexpectedly inserts navigation, `<head>`, or the whole page into a component,
the endpoint returned `View()` or `Page()` instead of a partial. Inspect the response body in browser developer tools
before changing `hx-target` or `hx-swap`.

Keep the full page and partial view separate:

```text
Pages/Products/Details.cshtml
Pages/Products/_ProductDetails.cshtml
```

The page owns the document layout. The partial owns only the element intended for the target.

## Preserve progressive enhancement

Prefer real links and forms when there is a meaningful non-HTMX behavior:

```html
<a asp-page="/Products/Details"
   asp-route-id="@product.Id"
   hx-boost="true"
   hx-target="#main">
    @product.Name
</a>
```

Without HTMX, the browser follows the `href`. With HTMX, the link is boosted and the server can detect `IsHtmxBoosted()`.

Buttons that exist only to update a local widget can use generated `hx-*` URLs without a navigation fallback.

## MVC action selection

Controllers can expose separate actions for the same route and constrain one to HTMX requests:

```csharp
[HttpGet("/products/{id:int}")]
public IActionResult Details(int id) =>
    View(catalog.Get(id));

[HttpGet("/products/{id:int}")]
[HtmxRequest]
public IActionResult DetailsFragment(int id) =>
    PartialView("_ProductDetails", catalog.Get(id));
```

`[HtmxRequest]` participates in MVC action selection. It does not reject requests inside an already selected action
and it does not apply to Razor Page handlers or Minimal API delegates.

## Minimal APIs

Minimal APIs can make the same decision and return HTML explicitly:

```csharp
app.MapGet("/products/{id:int}", (int id, HttpRequest request) =>
{
    var product = catalog.Get(id);
    var html = render.Product(product, includeLayout: !request.IsHtmxRequest());
    return Results.Content(html, "text/html");
});
```

HtmxToolkit detects and configures requests and responses, but it does not provide an HTML renderer for Minimal APIs.

Continue with [Read HTMX requests](requests.md) and [Control HTMX responses](responses.md).
