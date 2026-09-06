# Tag Helpers

HtmxToolkit Tag Helpers generate URLs and JSON attributes through ASP.NET Core routing and Razor encoding.
They complement ordinary HTMX attributes such as `hx-target`, `hx-trigger`, and `hx-swap`.

Register the namespace and Tag Helpers in `_ViewImports.cshtml`:

```html
@using Ramstack.HtmxToolkit
@addTagHelper *, Ramstack.HtmxToolkit
```

## Generate request URLs

### Razor Pages

```html
<button hx-page="/Orders/Details"
        hx-page-handler="Status"
        hx-route-id="@Model.OrderId"
        hx-target="#order-status">
    Refresh status
</button>
```

This generates `hx-get` because no HTMX method attribute was supplied.
Add an empty `hx-post`, `hx-put`, `hx-patch`, or `hx-delete` to select another method:

```html
<button hx-post
        hx-page="/Orders/Edit"
        hx-page-handler="Archive"
        hx-route-id="@Model.OrderId">
    Archive
</button>
```

### MVC controllers

```html
<button hx-post
        hx-area="Admin"
        hx-controller="Users"
        hx-action="Disable"
        hx-route-id="@user.Id"
        hx-target="#user-@user.Id">
    Disable
</button>
```

### Named routes and route dictionaries

```html
<a hx-route="product-details"
   hx-route-id="@product.Id"
   hx-fragment="reviews"
   hx-target="#product-panel">
    Product details
</a>
```

Use `hx-all-route-data` for a dictionary and `hx-route-*` for individual values.

> [!NOTE]
> `hx-fragment` is an input of `HtmxUrlTagHelper`, analogous to ASP.NET Core's `asp-fragment`.
> It adds `#reviews` to the generated URL; it is not an HTMX client attribute.

`hx-route`, controller/action, and page/handler identify mutually exclusive routing modes.
Supplying more than one mode throws an `InvalidOperationException`.
Supplying more than one HTMX method is also invalid.

## Send additional values

Use `hx-val-*` when fixed values should be included as request parameters:

```html
<button hx-page="/Reports"
        hx-page-handler="Preview"
        hx-val-category="science"
        hx-val-format="summary"
        hx-target="#preview">
    Preview
</button>
```

The Tag Helper emits an encoded `hx-vals` JSON object. A dictionary can be supplied through `hx-all-vals`:

```html
<button hx-get="/reports/preview"
        hx-all-vals="@Model.PreviewValues">
    Preview
</button>
```

HTMX 1.x and 2.x inherit the generated `hx-vals` from parent elements automatically.
For HTMX 4.x, add `hx-vals-inherited="true"` to the Razor element to generate the explicit inheritance modifier:

```razor
<section hx-vals-inherited="true"
         hx-val-tenant="@Model.TenantId">
    ...
</section>
```

With the default HTMX 4 meta character, this produces `hx-vals:inherited='{"tenant":"..."}'`.

HTMX 1.x and 2.x merge inherited values: a child value overrides a value with the same name
while other inherited values remain. HTMX 4.x behaves differently — a plain child `hx-vals` replaces
the inherited object entirely. Merging the child object into the inherited values
requires the HTMX 4 `:append` modifier, which HtmxToolkit does not currently generate.

## Send custom headers

```html
<section hx-header-X-Tenant="@Model.TenantId">
    <button hx-get="/reports" hx-target="#report">Load report</button>
</section>
```

`hx-header-*` and `hx-all-headers` generate `hx-headers` JSON. Header names are compared without case.
HTMX 1.x and 2.x inherit the generated attribute automatically.
For HTMX 4.x, use the Toolkit input `hx-headers-inherited="true"` on a parent element:

```razor
<section hx-headers-inherited="true"
         hx-header-X-Tenant="@Model.TenantId">
    ...
</section>
```

This generates `hx-headers:inherited` when the default HTMX 4 meta character is used.

Do not use custom client headers as proof of identity or authorization; clients can modify them.
Authenticate and authorize on the server.

## Configure one request

Use `hx-request-*` attributes rather than hand-writing version-specific JSON:

```html
<button hx-page="/Reports"
        hx-page-handler="Preview"
        hx-request-timeout="2000"
        hx-request-credentials="HtmxRequestCredentials.SameOrigin"
        hx-target="#preview">
    Generate preview
</button>
```

The output depends on the configured HTMX target:

```html
<!-- HTMX 1.x and 2.x -->
<button hx-request='{"timeout":2000,"credentials":false}' ...>

<!-- HTMX 4.x -->
<button hx-config='{"timeout":2000,"credentials":"same-origin"}' ...>
```

| Razor attribute | V1/V2 | V4 |
|---|---|---|
| `hx-request-timeout` | Yes | Yes |
| `hx-request-credentials` | Boolean mapping | Fetch credentials mode |
| `hx-request-no-headers` | Yes | No |
| `hx-request-cache` | No | Yes |
| `hx-request-redirect` | No | Yes |
| `hx-request-referrer` | No | Yes |
| `hx-request-integrity` | No | Yes |
| `hx-request-validate` | No | Yes |

Unsupported properties are omitted from the generated JSON. In particular, `HtmxRequestCredentials.Omit`
cannot be represented by HTMX 1.x or 2.x and is omitted for those targets.

## Inherit generated attributes

HTMX 4.x requires attribute inheritance to be explicit by default.
Razor cannot use the colon-form HTMX modifier as a bound Tag Helper input,
so HtmxToolkit provides hyphenated boolean inputs and generates the correct client attribute:

| Razor Tag Helper input | Generated HTMX 4 attribute |
|---|---|
| `hx-request-inherited="true"` | `hx-config:inherited` |
| `hx-vals-inherited="true"` | `hx-vals:inherited` |
| `hx-headers-inherited="true"` | `hx-headers:inherited` |

For example, inherit per-request timeout configuration from a parent:

```razor
<section hx-request-inherited="true"
         hx-request-timeout="2000">
    ...
</section>
```

For HTMX 1.x and 2.x, these boolean inputs do not change the generated names
because `hx-request`, `hx-vals`, and `hx-headers` are already merge-inherited automatically.

If `HtmxV4Config.MetaCharacter` is configured, HtmxToolkit uses that character instead of `:` in the generated attribute name.
Alternatively, set `HtmxV4Config.ImplicitInheritance` to `true` to enable inheritance globally;
the `*-inherited` inputs are then unnecessary.

HtmxToolkit does not currently expose an `*-append` Tag Helper input.
The table above documents only modifiers the Toolkit can generate.

## Render global configuration

`<htmx-config />` renders the version-specific application configuration and antiforgery metadata:

```html
<head>
    <htmx-config />
</head>
```

The equivalent `<meta htmx-config />` form is also supported. Prefer the dedicated element for readability.

See [Application configuration](configuration.md) and [Antiforgery and Toolkit script](antiforgery.md)
for the required layout setup.

Refer to the [API reference](../api/index.md) for every property and accepted type.
