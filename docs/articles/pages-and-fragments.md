# Full pages and fragments

An HTMX application usually returns a small HTML fragment for an HTMX request and a complete document
for normal browser navigation. Supporting both paths keeps links usable without JavaScript
and gives history restoration a full-page fallback.

## Return a full page or a fragment from the same URL

An MVC action can select the result from the request:

```csharp
public IActionResult Details(int id)
{
    var product = catalog.Get(id);

    if (Request.IsHtmxPartialRequest())
        return PartialView("_ProductDetails", product);

    return View(product);
}
```

The Razor Page equivalent assigns data for a normal page request and returns a partial for HTMX:

```csharp
public IActionResult OnGet(int id)
{
    Product = catalog.Get(id);

    if (Request.IsHtmxPartialRequest())
        return Partial("_ProductDetails", Product);

    return Page();
}
```

`IsHtmxPartialRequest()` is `true` only when HTMX swaps the response into an element inside the page.
A boosted link that replaces the `<body>`, an `hx-select` request, and a history restoration
get the complete document. The method reads the `HX-Request-Type` header;
see [Check full or partial requests](requests.md#check-full-or-partial-requests) for how it is computed.

With HTMX 1.9.x and 2.x, the header comes from the Toolkit script, so load the script on every page that uses HTMX.
Without it, the endpoints above return the full page to every request.

## Use a dedicated fragment endpoint

A dedicated endpoint is appropriate when the fragment is an operation rather than another representation of a page:

```csharp
public IActionResult OnGetInventory(int productId) =>
    Partial("_Inventory", inventory.Get(productId));
```

```razor
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
the endpoint returned `View()` or `Page()` instead of a partial. With the shared endpoint above and HTMX 1.9.x or 2.x,
the usual reason is a page without the Toolkit script: the request has no `HX-Request-Type` header, so
`IsHtmxPartialRequest()` returns `false`. Inspect the request headers and the response body in browser developer tools
before changing `hx-target` or `hx-swap`.

Keep the full page and partial view separate:

```text
Pages/Products/Details.cshtml
Pages/Products/_ProductDetails.cshtml
```

The page owns the document layout. The partial owns only the element intended for the target.

## Preserve progressive enhancement

Prefer real links and forms when there is a meaningful non-HTMX behavior:

```razor
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

Controllers can expose separate actions for the same route and let MVC choose between them:

```csharp
[HttpGet("/products/{id:int}")]
public IActionResult Details(int id) =>
    View(catalog.Get(id));

[HttpGet("/products/{id:int}")]
[HtmxRequest(RequestType = HtmxRequestType.Partial)]
public IActionResult DetailsFragment(int id) =>
    PartialView("_ProductDetails", catalog.Get(id));
```

HTMX requests for a fragment select `DetailsFragment`. Everything else, including boosted navigation
and history restoration, selects `Details`.

`[HtmxRequest]` participates in MVC action selection. It does not reject requests inside an already selected action
and it does not apply to Razor Page handlers or Minimal API delegates.
See [Select MVC actions declaratively](requests.md#select-mvc-actions-declaratively) for its options.

## Minimal APIs

Minimal APIs can make the same decision and return HTML explicitly:

```csharp
app.MapGet("/products/{id:int}", (int id, HttpRequest request) =>
{
    var product = catalog.Get(id);
    var fragment = request.IsHtmxPartialRequest();
    var html = render.Product(product, includeLayout: !fragment);
    return Results.Content(html, "text/html");
});
```

Here `catalog` and `render` stand for your application's data service and renderer. HtmxToolkit does not provide
an HTML renderer. See [Minimal APIs](minimal-apis.md) for a complete example without those placeholders.

## Cache full pages and fragments separately

If you cache a URL that returns different HTML based on request headers, those headers must be part of the
cache key. Otherwise a normal navigation can receive a cached fragment, or a widget can receive a cached page.
For the shared endpoint above, append a `Vary` header before returning either result:

```csharp
Response.Headers.Append("Vary", "HX-Request, HX-History-Restore-Request, HX-Request-Type");
```

Include any other headers that change the result, such as `HX-Boosted` or `HX-Target` if your endpoint uses them.
Appending preserves existing `Vary` values. If you generate ETags, distinguish full and partial responses too.
See [HTMX caching](https://htmx.org/docs/#caching).

ASP.NET Core output caching needs its own policy; a `Vary` response header is not a substitute for its cache-key settings.
On ASP.NET Core 7 or later, a policy for public, cacheable HTML could include:

```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("HtmxHtml", policy => policy.SetVaryByHeader(
        "HX-Request", "HX-History-Restore-Request", "HX-Request-Type"));
});
```

Register `app.UseOutputCache()` in the request pipeline and apply the named policy with
`[OutputCache(PolicyName = "HtmxHtml")]` in MVC or `.CacheOutput("HtmxHtml")` on a Minimal API endpoint.
The MVC attribute is in `Microsoft.AspNetCore.OutputCaching`.
See [ASP.NET Core output caching](https://learn.microsoft.com/aspnet/core/performance/caching/output)
for middleware placement and cache eligibility.

Do not use a shared cache for user-specific content or documents containing antiforgery tokens.
The introductory examples leave HTML response caching disabled.

Continue with [Read HTMX requests](requests.md) and [Control HTMX responses](responses.md).
