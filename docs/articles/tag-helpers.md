# Tag Helpers

HtmxToolkit Tag Helpers generate URLs and JSON attributes through ASP.NET Core routing and Razor encoding.
They complement ordinary HTMX attributes such as `hx-target`, `hx-trigger`, and `hx-swap`.

Register the namespace and Tag Helpers in `_ViewImports.cshtml`:

```razor
@using Ramstack.HtmxToolkit
@addTagHelper *, Ramstack.HtmxToolkit
```

## Generate request URLs

### Razor Pages

```razor
<button hx-get
        asp-page="/Orders/Details"
        asp-page-handler="Status"
        asp-route-id="@Model.OrderId"
        hx-target="#order-status">
    Refresh status
</button>
```

The Tag Helper reads the standard ASP.NET Core `asp-*` routing attributes and writes the generated URL
to the HTMX request attribute. Write that attribute without a value; it selects the method:
`hx-get`, `hx-post`, `hx-put`, `hx-patch`, or `hx-delete`.

```razor
<button hx-post
        asp-page="/Orders/Edit"
        asp-page-handler="Archive"
        asp-route-id="@Model.OrderId">
    Archive
</button>
```

### MVC controllers

```razor
<button hx-post
        asp-area="Admin"
        asp-controller="Users"
        asp-action="Disable"
        asp-route-id="@user.Id"
        hx-target="#user-@user.Id">
    Disable
</button>
```

### Named routes and route dictionaries

Name an endpoint, then use that name in `asp-route`. For example, replace the `[HttpGet]` attribute on
the `Details` action in the [MVC tutorial](mvc.md) with:

```csharp
[HttpGet("/products/{id:int}", Name = "product-details")]
public IActionResult Details(int id)
{
    if (id != 42)
        return NotFound();

    return PartialView("_ProductDetails", _products.Current);
}
```

The tutorial's `MapDefaultControllerRoute()` also maps this attribute-routed action.
In `Views/Products/Index.cshtml`, use the route name instead of controller and action attributes:

```razor
<button hx-get
        asp-route="product-details"
        asp-route-id="42"
        hx-target="#product-panel">
    Product details
</button>

<div id="product-panel"></div>
```

The generated URL is `/products/42`. Use `asp-all-route-data` when route values are already in a dictionary:

```razor
@{
    var routeValues = new Dictionary<string, string>
    {
        ["id"] = "42",
        ["tab"] = "reviews"
    };
}

<button hx-get
        asp-route="product-details"
        asp-all-route-data="routeValues"
        hx-target="#product-panel">
    Product details
</button>
```

Here `id` fills the route parameter and `tab` becomes a query parameter: `/products/42?tab=reviews`.
The action can accept a `string? tab` parameter if it needs that value.
Use `asp-route-*` for individual values or `asp-all-route-data` for the dictionary.

`asp-fragment="reviews"` appends `#reviews` to the generated URL. The browser does not send the fragment
to the server, so use `hx-target`, not the fragment, to choose the element to update.

Choose one routing mode: `asp-route`, controller/action, or page/handler.
Combining modes throws an `InvalidOperationException`. So does supplying more than one HTMX request
attribute, or an HTMX request attribute with a value, even an empty one such as `hx-get=""`.
The Tag Helper recognizes the `hx-` prefix only, not `data-hx-`.

### Links, forms, and buttons

The standard ASP.NET Core Tag Helpers process the same `asp-*` attributes on `<a>`, `<form>`, `<button>`,
and submit or image `<input>` elements. They render `href`, `action`, or `formaction` with the same URL,
so links and forms keep working without HTMX:

```razor
<a hx-get asp-page="/Orders/Details" asp-route-id="@Model.OrderId" hx-target="#order">Details</a>
```

Keep these side effects in mind:

- Add `method="get"` to a `<form hx-get>`. Otherwise the Form Tag Helper renders `method="post"` and
  a hidden antiforgery input, and HTMX sends the token in the query string.
- A link always gets a working `href`, which a browser can follow with an ordinary GET request,
  for example when the link is opened in a new tab or prefetched. Use a `<button>` rather than `<a>`
  for `hx-post`, `hx-put`, `hx-patch`, and `hx-delete`.
- On other elements, the Tag Helper processes `asp-*` attributes only together with an HTMX request
  attribute. Without one, they are rendered unchanged.

### HTMX 4.x `hx-action`

HTMX 4.x can take the URL from `hx-action` and the method from `hx-method`.
Write `hx-action` without a value to have the Tag Helper fill it:

```razor
<button hx-action
        hx-method="post"
        asp-page="/Orders/Edit"
        asp-page-handler="Archive"
        asp-route-id="@Model.OrderId">
    Archive
</button>
```

An `hx-action` that already has a URL and no `asp-*` attributes is left as is. With HTMX 1.9.x or 2.x,
which do not support `hx-action`, an `hx-action` without a value and with `asp-*` attributes throws an `InvalidOperationException`.

## Send additional values

Use `hx-val-*` when fixed values should be included as request parameters:

```razor
<button hx-get
        asp-page="/Reports"
        asp-page-handler="Preview"
        hx-val-category="science"
        hx-val-format="summary"
        hx-target="#preview">
    Preview
</button>
```

The Tag Helper emits an encoded `hx-vals` JSON object. A dictionary can be supplied through `hx-all-vals`:

```razor
<button hx-get="/reports/preview"
        hx-all-vals="@Model.PreviewValues">
    Preview
</button>
```

HTMX 1.9.x and 2.x inherit the generated `hx-vals` from parent elements automatically.
For HTMX 4.x, add `hx-vals-inherited="true"` to the Razor element to generate the explicit inheritance modifier:

```razor
<section hx-vals-inherited="true"
         hx-val-tenant="@Model.TenantId">
    ...
</section>
```

This produces `hx-vals:inherited='{"tenant":"..."}'`.

HTMX 1.9.x and 2.x merge inherited values: a child value overrides a value with the same name
while other inherited values remain. In HTMX 4.x, a plain child `hx-vals` replaces the inherited
object entirely. Add `hx-vals-append="true"` to merge the child object instead:

```razor
<section hx-vals-inherited="true"
         hx-val-tenant="@Model.TenantId">
    <button hx-post="/reports/preview"
            hx-vals-append="true"
            hx-val-format="summary">
        Preview
    </button>
</section>
```

The button emits `hx-vals:append='{"format":"summary"}'`.

## Send custom headers

```razor
<section hx-header-X-Tenant="@Model.TenantId">
    <button hx-get="/reports" hx-target="#report">Load report</button>
</section>
```

`hx-header-*` and `hx-all-headers` generate `hx-headers` JSON. Header names are compared without case.
HTMX 1.9.x and 2.x inherit the generated attribute automatically.
For HTMX 4.x, use the Toolkit input `hx-headers-inherited="true"` on a parent element:

```razor
<section hx-headers-inherited="true"
         hx-header-X-Tenant="@Model.TenantId">
    ...
</section>
```

This generates `hx-headers:inherited`.
Use `hx-headers-append="true"` on a child declaration to merge its headers into the inherited object.

Do not use custom client headers as proof of identity or authorization; clients can modify them.
Authenticate and authorize on the server.

## Configure one request

Use `hx-request-*` attributes rather than hand-writing version-specific JSON:

```razor
<button hx-get
        asp-page="/Reports"
        asp-page-handler="Preview"
        hx-request-timeout="2000"
        hx-request-credentials="@HtmxRequestCredentials.SameOrigin"
        hx-target="#preview">
    Generate preview
</button>
```

The output depends on the configured HTMX target:

```html
<!-- HTMX 1.9.x and 2.x -->
<button hx-request='{"timeout":2000,"credentials":false}' ...>

<!-- HTMX 4.x -->
<button hx-config='{"timeout":2000,"credentials":"same-origin"}' ...>
```

| Razor attribute          | V1/V2           | V4                     |
|--------------------------|-----------------|------------------------|
| `hx-request-timeout`     | Yes             | Yes                    |
| `hx-request-credentials` | Boolean mapping | Fetch credentials mode |
| `hx-request-no-headers`  | Yes             | No                     |
| `hx-request-cache`       | No              | Yes                    |
| `hx-request-redirect`    | No              | Yes                    |
| `hx-request-referrer`    | No              | Yes                    |
| `hx-request-integrity`   | No              | Yes                    |
| `hx-request-validate`    | No              | Yes                    |

Unsupported properties are omitted from the generated JSON. In particular, `HtmxRequestCredentials.Omit`
cannot be represented by HTMX 1.9.x or 2.x and is omitted for those targets.

Use `HtmxRequestCache` and `HtmxRequestRedirect` for the HTMX 4.x Fetch options:

```razor
<button hx-get="/Reports/Preview"
        hx-request-cache="@HtmxRequestCache.NoCache"
        hx-request-redirect="@HtmxRequestRedirect.Error">
    Generate preview
</button>
```

This generates `hx-config='{"cache":"no-cache","redirect":"error"}'`.
Leaving either property unset omits that option from the JSON.
`HtmxRequestCache.OnlyIfCached` requires the same-origin Fetch mode, which is the HTMX default;
the request fails if `HtmxV4Config.Mode` specifies another mode.

## Apply attribute modifiers

HTMX 4.x requires attribute inheritance to be explicit by default.
Razor cannot use the colon-form HTMX modifier as a bound Tag Helper input,
so HtmxToolkit provides hyphenated boolean inputs and generates the correct client attribute:

| Razor Tag Helper input        | Generated HTMX 4 attribute |
|-------------------------------|----------------------------|
| `hx-request-inherited="true"` | `hx-config:inherited`      |
| `hx-request-append="true"`    | `hx-config:append`         |
| `hx-vals-inherited="true"`    | `hx-vals:inherited`        |
| `hx-vals-append="true"`       | `hx-vals:append`           |
| `hx-headers-inherited="true"` | `hx-headers:inherited`     |
| `hx-headers-append="true"`    | `hx-headers:append`        |

For example, inherit per-request timeout configuration from a parent:

```razor
<section hx-request-inherited="true"
         hx-request-timeout="2000">
    ...
</section>
```

Use `inherited` on a parent declaration to make it available to descendants, and `append` on a child
declaration to merge rather than replace the inherited object. When both inputs are `true` on the same
element, HtmxToolkit emits one combined attribute such as `hx-config:inherited:append`.

For HTMX 1.9.x and 2.x, these boolean inputs do not change the generated names.
`hx-request`, `hx-vals`, and `hx-headers` already inherit from parents: child values override matching
parent values and keep the rest.

Alternatively, set `HtmxV4Config.ImplicitInheritance` to `true` to enable inheritance globally;
the `*-inherited` inputs are then unnecessary. Child declarations can still use the corresponding
`*-append` input when they need to merge with inherited values.

## Render global configuration

`<htmx-config />` renders the version-specific application configuration and antiforgery metadata:

```razor
<head>
    <htmx-config />
</head>
```

The equivalent `<meta htmx-config />` form is also supported. Prefer the dedicated element for readability.

See [Application configuration](configuration.md) and [Antiforgery and Toolkit script](antiforgery.md)
for the required layout setup.

Refer to the [API reference](../api/index.md) for every property and accepted type.
