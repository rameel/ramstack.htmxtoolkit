# HtmxToolkit

[![NuGet](https://img.shields.io/nuget/v/Ramstack.HtmxToolkit.svg)](https://www.nuget.org/packages/Ramstack.HtmxToolkit/)
[![Build](https://github.com/rameel/ramstack.htmxtoolkit/actions/workflows/test.yml/badge.svg)](https://github.com/rameel/ramstack.htmxtoolkit/actions/workflows/test.yml)
[![License: MIT](https://img.shields.io/github/license/rameel/ramstack.htmxtoolkit)](LICENSE)

HtmxToolkit integrates [HTMX](https://htmx.org/) with ASP.NET Core. It provides strongly typed APIs for request and response headers,
MVC action filters, Razor Tag Helpers, application-wide HTMX configuration, and antiforgery support.

- Supports .NET 6 or later.
- Supports HTMX 1.9.x, HTMX 2.x, and HTMX 4.x. HTMX 2.x is selected by default.

See the [documentation](docs/articles/index.md) for full guides and recipes.

## Features

- Detect HTMX requests, including boosted requests, without comparing header strings.
- Read and write all standard HTMX headers through strongly typed APIs.
- Route HTMX requests to dedicated MVC actions with `[HtmxRequest]`.
- Configure response behavior fluently or with `[HtmxResponse]`.
- Generate HTMX URLs, headers, values, and request options with Razor Tag Helpers.
- Render version-specific HTMX configuration from ASP.NET Core options.
- Add antiforgery tokens to non-GET HTMX requests with a small companion script.

## Designed for Low Overhead

`HtmxRequestHeaders` and `HtmxResponseHeaders` are `readonly` structs, each containing a single reference.
This avoids allocating wrapper objects and allows the JIT to optimize away the wrapper overhead in inlined code.

Version-specific configuration JSON is cached and reused until the configuration changes.
Known JSON shapes use source-generated `System.Text.Json` metadata, avoiding reflection-based metadata discovery at runtime.

## Installation

```console
dotnet add package Ramstack.HtmxToolkit
```

Register HtmxToolkit in `Program.cs`:

```csharp
using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHtmxToolkit();
```

> [!IMPORTANT]
> HtmxToolkit does not bundle HTMX itself. Add a supported HTMX release to the application separately.

## Quick Start

Make the Tag Helpers and toolkit types available to Razor views in `_ViewImports.cshtml`:

```html
@using Ramstack.HtmxToolkit
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Ramstack.HtmxToolkit
```

Render the configuration metadata in the document `<head>`:

```html
<head>
    <htmx-config />
</head>
```

On ASP.NET Core 9 or later, enable static assets and associate them with the endpoints that render views in `Program.cs`:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
```

> [!NOTE]
> For MVC, apply `.WithStaticAssets()` to each controller endpoint builder that renders views

On ASP.NET Core 6–8, enable static files:

```csharp
app.UseStaticFiles();
```

The NuGet package includes the toolkit script as a static web asset. Load it after HTMX in the layout:

```html
<script src="/path/to/htmx.min.js"></script>
<script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
        asp-append-version="true"></script>
```

You can now generate an HTMX URL from ASP.NET Core route information:

```html
<button hx-get
        hx-controller="Books"
        hx-action="List"
        hx-route-category="science"
        hx-target="#results">
    Browse books
</button>

<div id="results"></div>
```

- Use `hx-post`, `hx-put`, `hx-patch`, or `hx-delete` instead of `hx-get` to select another HTTP method.
- If the method attribute is omitted, the URL Tag Helper defaults to `hx-get`.

## Requests

Use `IsHtmxRequest` when an endpoint should return a partial response for an HTMX request and a full-page response otherwise:

```csharp
public IActionResult Help()
{
    if (Request.IsHtmxRequest(out var htmx) && !htmx.HistoryRestoreRequest)
        return PartialView("_Help");

    return View();
}
```

Checking `HistoryRestoreRequest` ensures that an HTMX history cache miss receives the full page it expects.
The `out` parameter provides strongly typed access to the request headers.

Call `Request.GetHtmxHeaders()` to access the same headers separately from request detection.

Available headers and their formats vary by HTMX version; see the
[request header comparison](docs/articles/version-compatibility.md#request-headers).

`HtmxRequestHeaderNames` exposes the corresponding header-name constants for lower-level APIs.

Use `Request.IsHtmxBoosted()` when only boosted navigation matters.
An overload also provides access to the strongly typed headers.

### MVC Action Selection

Apply `[HtmxRequest]` to reserve an action for HTMX requests:

```csharp
[HttpGet("/profile/fragment")]
[HtmxRequest]
public IActionResult ProfileFragment()
{
    return PartialView("_Profile");
}
```

Set `Boosted` to restrict action selection to boosted or non-boosted HTMX requests:

```csharp
[HtmxRequest(Boosted = true)]
public IActionResult BoostedNavigation()
{
    return PartialView("_Navigation");
}
```

## Responses

Configure HTMX response headers through `Response.Htmx(...)`:

```csharp
Response.Htmx(htmx => htmx
    .Retarget("#profile")
    .Reswap(HtmxSwap.OuterHtml)
    .TriggerEvent("profile-updated"));
```

> [!NOTE]
> The callback runs only for HTMX requests, so non-HTMX requests avoid unnecessary response work.

The fluent API supports:

- Client navigation with `Location`, `Redirect`, `PushUrl`, and `ReplaceUrl`.
- Swap control with `Reswap`, `Retarget`, and `Reselect`.
- Page refresh with `Refresh`.
- Client events with `TriggerEvent`.

The same API works with any `HttpResponse`, including
[Minimal API handlers](docs/articles/responses.md#configure-a-response-fluently).

Use [state-passing overloads](docs/articles/responses.md#avoid-closures-in-a-hot-path) to avoid closure allocations in hot paths.

For trimming and Native AOT, pass source-generated `JsonTypeInfo<T>` metadata to `TriggerEvent`;
see the [complete example](docs/articles/responses.md#trigger-client-events).

Call `Response.GetHtmxHeaders()` for direct access to the strongly typed response headers,
or use `HtmxResponseHeaderNames` with lower-level APIs.

### Declarative Responses

Controllers can set common response headers declaratively:

```csharp
[HtmxRequest]
[HtmxResponse(
    Retarget = "#results",
    Reswap = HtmxSwap.BeforeEnd)]
public IActionResult LoadMore()
{
    return PartialView("_MoreResults");
}
```

`HtmxResponseAttribute` supports `Refresh`, `Reswap`, `ReswapExpression`, `Retarget`, and `Reselect`.
Use `ReswapExpression` for a complete expression with swap modifiers, such as `innerHTML show:#result:top`.

## Tag Helpers

HtmxToolkit includes five Tag Helpers:

| Tag Helper             | Purpose                                                                     |
|------------------------|-----------------------------------------------------------------------------|
| `HtmxUrlTagHelper`     | Builds HTMX request URLs from routes, controllers, actions, or Razor Pages. |
| `HtmxHeaderTagHelper`  | Serializes custom `hx-headers` values.                                      |
| `HtmxValsTagHelper`    | Serializes additional `hx-vals` request values.                             |
| `HtmxRequestTagHelper` | Generates version-specific `hx-request` or `hx-config` options.             |
| `HtmxConfigTagHelper`  | Renders application configuration and antiforgery metadata.                 |

### URL Generation

Controller and action:

```html
<button hx-post
        hx-area="Admin"
        hx-controller="Users"
        hx-action="Disable"
        hx-route-id="@Model.Id">
    Disable user
</button>
```

Razor Page handler:

```html
<button hx-page="/Attendee"
        hx-page-handler="Profile"
        hx-route-attendeeid="@Model.Id">
    Show profile
</button>
```

Use `hx-all-route-data` for an `IDictionary<string, string>` of route values.
The helper also supports `hx-route`, `hx-host`, `hx-protocol`, and `hx-fragment`.

### Headers and Values

Create `hx-headers` without manually escaping JSON:

```html
<button hx-get="/reports"
        hx-header-X-View="compact"
        hx-header-X-Time-Zone="UTC">
    Load report
</button>
```

Add request values in the same way:

```html
<button hx-get="/books"
        hx-val-category="science"
        hx-val-format="summary">
    Browse books
</button>
```

Use `hx-all-headers` or `hx-all-vals` to supply an `IDictionary<string, string>`.

### Request Options

For HTMX 1.9.x and 2.x, typed `hx-request-*` attributes generate `hx-request` JSON:

```html
<button hx-get="/reports"
        hx-request-timeout="5000"
        hx-request-credentials="@HtmxRequestCredentials.Include"
        hx-request-no-headers="false">
    Load report
</button>
```

With HTMX 4.x selected, `HtmxRequestTagHelper` generates `hx-config` instead.
See [Version compatibility](docs/articles/version-compatibility.md#per-request-options) for supported options.

### Attribute Modifiers

HTMX 1.9.x and 2.x merge-inherit `hx-request`, `hx-vals`, and `hx-headers` automatically.
With HTMX 4.x, use the Tag Helper's `inherited` input on a parent and `append` on a child to merge their values:

```html
<div hx-vals-inherited="true"
     hx-val-category="books">
    <button hx-post="/search"
            hx-vals-append="true"
            hx-val-sort="title">
        Search
    </button>
</div>
```

The Tag Helper emits `hx-vals:inherited` on the parent and `hx-vals:append` on the child.
See [Attribute modifiers](docs/articles/version-compatibility.md#attribute-modifiers) for all supported inputs and version differences.

## Configuration

Configure HTMX once during service registration. Only explicitly configured values are emitted,
so HTMX defaults remain in effect:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.ReportValidityOfForms = true;
        config.DefaultFocusScroll = true;
    });
});
```

Select the version loaded by the browser with `UseHtmxV1`, `UseHtmxV2`, or `UseHtmxV4`:

```csharp
builder.Services.AddHtmxToolkit(options => options.UseHtmxV4());
```

> [!WARNING]
> Select only one HTMX version. Attempting to select a second version in the same configuration throws an exception.

See the [configuration guides](docs/articles/configuration.md) for version-specific settings and response handling.

## Antiforgery

Antiforgery metadata is enabled by default. `<htmx-config />` renders the current token and field or header names;
the companion script attaches the token to non-GET HTMX requests and refreshes it after boosted navigation.

> [!WARNING]
> The companion script sends the token but does not validate it. Razor Pages validates unsafe HTTP methods automatically.
> MVC applications must enable server-side antiforgery validation for the relevant actions.

For example, MVC applications can validate all unsafe actions globally:

```csharp
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
```

Disable the metadata when antiforgery is handled elsewhere:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = false;
});
```

See [Antiforgery and Toolkit script](docs/articles/antiforgery.md) for setup and script-loading details.

## Compatibility Notes

### Trigger Timing

> [!IMPORTANT]
> HTMX 1.9.x and 2.x support `HX-Trigger`, `HX-Trigger-After-Swap`, and `HX-Trigger-After-Settle`.
> HTMX 4.x supports only `HX-Trigger`, which fires when the request completes (after the swap whenever one is performed).
> HtmxToolkit therefore emits events requested for any `HtmxTriggerTiming`
> value through that header rather than dropping them. The `Receive` and `AfterSettle` timings cannot be preserved exactly.

### Morph Swaps

`HtmxSwap.InnerMorph` and `HtmxSwap.OuterMorph` use the native `innerMorph` and `outerMorph` swap styles in HTMX 4.x.
No additional client-side dependency or configuration is required.

With HTMX 1.9.x or 2.x, enable the toolkit's `ramstack-morph` extension and load Idiomorph for morphing:

```html
<body hx-ext="ramstack-morph">
    ...
</body>
```

Without Idiomorph, the extension falls back to HTML replacement.
See [Morph swaps](docs/articles/version-compatibility.md#morph-swaps) for script setup, supported styles, and fallback behavior.

## Running Locally

### Demo

The [`samples/Ramstack.HtmxToolkit.Demo`](samples/Ramstack.HtmxToolkit.Demo) project demonstrates request detection,
response headers and events, MVC attributes, Tag Helpers, polling, boosted navigation, and antiforgery integration.

Run it from the repository root:

```console
dotnet run --project samples/Ramstack.HtmxToolkit.Demo
```

The application is available at <https://localhost:5001> and <http://localhost:5000>.

### Documentation

See [Building the documentation locally](docs/README.md) for build and preview commands.

## Contributing

Bug reports and pull requests are welcome. See [Contributing](CONTRIBUTING.md) for development setup,
validation commands, and pull request and commit guidelines.

## Supported versions

|      | Version            |
|------|--------------------|
| .NET | 6, 7, 8, 9, 10, 11 |
| HTMX | 1.9.x, 2.x, 4.x    |


## License

HtmxToolkit is available under the [MIT License](LICENSE).
