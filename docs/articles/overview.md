# Overview

Ramstack.HtmxToolkit connects server-rendered ASP.NET Core applications to HTMX. It removes stringly typed header
handling and manual JSON or URL construction while preserving the normal HTMX programming model.

The package targets .NET 6 and can be used by applications running on .NET 6 or later. It supports HTMX 1.9.x, 2.x, and 4.x.

## What the toolkit provides

| Area | Toolkit feature |
|---|---|
| Incoming requests | `IsHtmxRequest`, `IsHtmxBoosted`, and strongly typed request headers |
| Outgoing responses | Fluent `Response.Htmx(...)` API and strongly typed response headers |
| MVC | `[HtmxRequest]` action selection and `[HtmxResponse]` response configuration |
| Razor | Tag Helpers for routes, values, headers, and per-request options |
| Configuration | Version-specific HTMX configuration rendered by `<htmx-config />` |
| Security | Automatic ASP.NET Core antiforgery headers for unsafe HTMX requests |
| Assets | A small companion script with a cacheable endpoint or inline rendering |

HtmxToolkit does not include the HTMX library and does not replace HTMX attributes such as `hx-target`, `hx-trigger`,
or `hx-swap`. Add a supported HTMX release to the application separately.

## Request lifecycle

1. Razor renders an element with ordinary HTMX attributes and, optionally, HtmxToolkit Tag Helper attributes.
2. HTMX sends a request with its `HX-*` headers.
3. ASP.NET Core detects the HTMX request and returns a server-rendered fragment.
4. The endpoint can add response instructions such as `HX-Retarget`, `HX-Redirect`, or `HX-Trigger`.
5. HTMX swaps the fragment and applies those response instructions in the browser.

```html
<button hx-page="/Products"
        hx-page-handler="Details"
        hx-route-id="42"
        hx-target="#product-details">
    View product
</button>

<div id="product-details"></div>
```

```csharp
public IActionResult OnGetDetails(int id)
{
    var product = catalog.Get(id);
    return Partial("_ProductDetails", product);
}
```

The Tag Helper generates the handler URL; HTMX performs the request and swap; ASP.NET Core renders the fragment.

## Choose the integration style

Use a shared endpoint when the full page and fragment represent the same resource. Check `Request.IsHtmxRequest()`
and select the appropriate result.

Use a dedicated handler when a fragment is an independent operation, such as validation, polling, or an inline edit.

Use `[HtmxRequest]` when MVC should choose between actions based on whether the request came from HTMX.
This is an action constraint, so it is intended for controllers rather than Razor Page handlers or Minimal APIs.

Continue with [Getting started](getting-started.md) for a complete setup.
