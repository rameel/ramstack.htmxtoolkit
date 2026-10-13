# Overview

Ramstack.HtmxToolkit helps ASP.NET Core applications work with HTMX. Read request headers through C# properties,
set response instructions with a fluent API, and generate request URLs and JSON attributes from Razor.

## How HTMX updates a page

HTMX is a JavaScript library that sends requests from HTML elements and puts the HTML response into the page.
For example, a button can request product details and replace the contents of a nearby panel.
The server renders the new HTML; you do not need to build that HTML from JSON in the browser.

Four attributes describe most interactions:

| Attribute | What it controls | Example |
|---|---|---|
| `hx-get` or `hx-post` | The request method and URL | `hx-get="/Products?handler=Details&id=42"` |
| `hx-trigger` | When to send the request | `hx-trigger="click"` |
| `hx-target` | The element to update | `hx-target="#product-details"` |
| `hx-swap` | How to insert the response | `hx-swap="innerHTML"` |

A button uses `click` by default, and a form uses `submit`. The default swap is `innerHTML`, which replaces
the target's contents and keeps the target itself. `outerHTML` replaces the target element too, so the response
must include any ID or request attributes needed for the next interaction.

HtmxToolkit adds the ASP.NET Core side of this flow: route generation in Razor, request inspection in C#,
and response headers that tell HTMX what to do next.

## What the toolkit provides

| Area | Toolkit feature |
|---|---|
| Incoming requests | `IsHtmxPartialRequest`, `IsHtmxFullRequest`, `IsHtmxRequest`, `IsHtmxBoosted`, and strongly typed request headers |
| Outgoing responses | Fluent `Response.Htmx(...)` API and strongly typed response headers |
| MVC | `[HtmxRequest]` action selection by request type and boosted navigation, and `[HtmxResponse]` response configuration |
| Razor | Tag Helpers for routes, values, headers, and per-request options |
| Configuration | Version-specific HTMX configuration rendered by `<htmx-config />` |
| Security | Automatic ASP.NET Core antiforgery headers for unsafe HTMX requests |
| Browser script | The [Toolkit script](antiforgery.md#what-the-toolkit-script-does), served as a static web asset and loaded from the layout |

HtmxToolkit does not include the HTMX library and does not replace HTMX attributes such as `hx-target`, `hx-trigger`,
or `hx-swap`. Add a supported HTMX release to the application separately.

## Request lifecycle

1. Razor renders an element with ordinary HTMX attributes and, optionally, HtmxToolkit Tag Helper attributes.
2. HTMX sends a request with its `HX-*` headers.
3. The ASP.NET Core endpoint renders HTML and can add response instructions such as `HX-Retarget`, `HX-Redirect`, or `HX-Trigger`.
4. HTMX reads the response headers and updates the page, navigates, or dispatches events as instructed.

For example, write this in a Razor view:

```razor
<button hx-page="/Products"
        hx-page-handler="Details"
        hx-route-id="42"
        hx-target="#product-details">
    View product
</button>

<div id="product-details"></div>
```

Razor processes the toolkit attributes on the server. The browser receives markup similar to:

```html
<button hx-get="/Products?id=42&amp;handler=Details" hx-target="#product-details">
    View product
</button>

<div id="product-details"></div>
```

The query parameter order may differ. `hx-page`, `hx-page-handler`, and `hx-route-*` are server-side inputs;
HTMX in the browser uses the generated `hx-get`. `hx-target` passes through unchanged.

The page handler returns a partial view:

```csharp
public IActionResult OnGetDetails(int id)
{
    var product = catalog.Get(id);
    return Partial("_ProductDetails", product);
}
```

The Tag Helper generates the handler URL; HTMX performs the request and swap; ASP.NET Core renders the fragment.

This is a sketch of the interaction. [Getting started](getting-started.md) supplies the model, partial,
registration, and layout needed to run it.

## Which pieces do I need?

| Application feature | Setup |
|---|---|
| Read request headers or set ordinary response headers | Reference the package and import `Ramstack.HtmxToolkit` in C# |
| Choose a target version or configure HTMX | Call `AddHtmxToolkit()`; render `<htmx-config />` in Razor for browser configuration |
| Generate URLs or JSON attributes in Razor | Register the toolkit Tag Helpers in `_ViewImports.cshtml` |
| Attach antiforgery tokens automatically | Render configuration metadata, load the Toolkit script, and enable server validation |
| Use compatibility morph swaps or replay duplicate event names | Load the Toolkit script; follow the relevant feature's setup |
| Tell full-page and fragment requests apart with HTMX 1.9.x or 2.x | Load the Toolkit script |

For HTMX 4.x, register the target version even if you only use response events, so the toolkit chooses the
correct trigger headers. In a pure Minimal API application, HTML rendering and token metadata are supplied
by the application; see the [Minimal API example](minimal-apis.md).

## Choose the integration style

Use a shared endpoint when the full page and fragment represent the same resource. Check `Request.IsHtmxPartialRequest()`
and select the appropriate result.

Use a dedicated handler when a fragment is an independent operation, such as validation, polling, or an inline edit.

Use `[HtmxRequest]` when MVC should choose between actions on the same URL, for example a page action
and a fragment action. It can match on the request type and on boosted navigation.
This is an action constraint, so it is intended for controllers rather than Razor Page handlers or Minimal APIs.

Continue with [Getting started](getting-started.md) for a complete setup.
