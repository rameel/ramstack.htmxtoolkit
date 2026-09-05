# HtmxToolkit

[![NuGet](https://img.shields.io/nuget/v/Ramstack.HtmxToolkit.svg)](https://www.nuget.org/packages/Ramstack.HtmxToolkit/)
[![Build](https://github.com/rameel/ramstack.htmxtoolkit/actions/workflows/test.yml/badge.svg)](https://github.com/rameel/ramstack.htmxtoolkit/actions/workflows/test.yml)
[![License: MIT](https://img.shields.io/github/license/rameel/ramstack.htmxtoolkit)](LICENSE)

HtmxToolkit integrates [HTMX](https://htmx.org/) with ASP.NET Core. It provides strongly typed APIs for request and response headers,
MVC action filters, Razor Tag Helpers, application-wide HTMX configuration, and antiforgery support.

The package targets .NET 6 and can be used by applications running on .NET 6 or later.
It supports HTMX 1.9.x, HTMX 2.x, and HTMX 4.x. HTMX 2.x is selected by default.

## Features

- Detect HTMX requests, including boosted requests, without comparing header strings.
- Read and write all standard HTMX headers through strongly typed APIs.
- Route HTMX requests to dedicated MVC actions with `[HtmxRequest]`.
- Configure response behavior fluently or with `[HtmxResponse]`.
- Generate HTMX URLs, headers, values, and request options with Razor Tag Helpers.
- Render version-specific HTMX configuration from ASP.NET Core options.
- Add antiforgery tokens to non-GET HTMX requests with a small companion script.

## Designed for Low Overhead

HtmxToolkit is designed to minimize HTMX integration overhead in the application's request-processing path:

- `HtmxRequestHeaders` and `HtmxResponseHeaders` are `readonly` structs, each containing a single reference.
  In normal use, they incur no wrapper allocations while preserving a strongly typed API.
- Version-specific HTMX configuration is serialized only when the configuration changes; the resulting JSON is cached and reused across requests.
- Known JSON shapes use source-generated `System.Text.Json` metadata, avoiding reflection-based metadata discovery at runtime.
  Event details passed to `TriggerEvent` are the deliberate exception because their types are defined by the application.
- Work is skipped for non-HTMX requests, and overloads that accept state allow callers to use static callbacks and avoid closure allocations.

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
@addTagHelper *, Ramstack.HtmxToolkit
```

Render the configuration metadata in the document `<head>`:

```html
<head>
    <htmx-config />
</head>
```

Map the companion script endpoint in `Program.cs`:

```csharp
app.MapHtmxToolkitScript();
```

Load HTMX first, then the toolkit script in the layout:

```html
<script src="/path/to/htmx.min.js"></script>
<script src="@Html.HtmxToolkitScriptPath()"></script>
```

The default script URL contains a content hash, so the script can be cached indefinitely. When the script changes, its URL changes automatically.

You can now generate an HTMX URL from ASP.NET Core route information:

```html
<button hx-controller="Books"
        hx-action="List"
        hx-route-category="science"
        hx-target="#results">
    Browse books
</button>

<div id="results"></div>
```

If no HTTP method is specified, the URL Tag Helper emits `hx-get`. Use `hx-post`, `hx-put`, `hx-patch`, or `hx-delete` to select another method.

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

Available properties include:

- `Boosted`
- `CurrentUrl`
- `HistoryRestoreRequest`
- `Prompt`
- `Request`
- `RequestType`
- `Source`
- `Target`
- `Trigger`
- `TriggerName`

> [!NOTE]
> The request header vocabulary differs between HTMX versions.
> `HX-Trigger`, `HX-Trigger-Name`, and `HX-Prompt` are sent by HTMX 1.x and 2.x only.
> HTMX 4.x does not support `hx-prompt`, identifies the source element with `HX-Source` instead,
> and reports partial or full requests through `HX-Request-Type`.
> In HTMX 4.x, `HX-Target` also carries a `tag#id` value (for example, `div#results`)
> rather than an ID alone.

`HtmxRequestHeaderNames` exposes the corresponding header-name constants for lower-level APIs.

Use `Request.IsHtmxBoosted()` when only boosted navigation matters. An overload also provides access to the strongly typed headers.

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
- Client events with `TriggerEvent` and `TriggerEvents`.

The same API works in Minimal API handlers:

```csharp
app.MapGet("/profile", (HttpResponse response) =>
{
    response.Htmx(htmx => htmx.Retarget("#profile"));
    return TypedResults.Content("<div>Profile</div>", "text/html");
});
```

> [!TIP]
> When a callback needs state, use the generic overload to pass it explicitly and avoid a closure allocation.

```csharp
Response.Htmx(
    static (htmx, path) => htmx.TriggerEvent("content-updated", new { path }),
    Request.Path.Value);
```

Call `Response.GetHtmxHeaders()` for direct access to the strongly typed response headers, or use `HtmxResponseHeaderNames` with lower-level APIs.

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
HTMX 4.x additionally supports `hx-request-cache`, `hx-request-redirect`, `hx-request-referrer`, `hx-request-integrity`,
and `hx-request-validate`; `hx-request-no-headers` is limited to HTMX 1.9.x and 2.x.

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

Render `<htmx-config />` in the document `<head>` to produce the corresponding `<meta name="htmx-config">` element.

Select a supported HTMX version family with `UseHtmxV1`, `UseHtmxV2`, or `UseHtmxV4`:

```csharp
builder.Services.AddHtmxToolkit(options => options.UseHtmxV4());
```

Configuration property names follow the selected HTMX version.
For example, HTMX 1.9.x and 2.x use `DefaultSwapStyle` and `Timeout`,
while HTMX 4.x uses `DefaultSwap` and `DefaultTimeout`.

> [!WARNING]
> Select only one HTMX version. Attempting to select a second version in the same configuration throws an exception.

### Response Handling

The following configuration follows the
[HTMX 2.x response-handling example](https://htmx.org/docs/#response-handling-examples), allowing `422` validation
responses to swap while treating other `4xx` and `5xx` responses as errors:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.ResponseHandling =
        [
            new() { Code = "204", Swap = false },
            new() { Code = "[23]..", Swap = true },
            new() { Code = "422", Swap = true },
            new() { Code = "[45]..", Swap = false, Error = true },
            new() { Code = "...", Swap = true }
        ];
    });
});
```

HTMX 4.x replaces `responseHandling` with `noSwap` and swaps `4xx` and `5xx` responses by default. To restore the
default HTMX 2.x behavior for those errors, the
[HTMX 4.x migration guide](https://four.htmx.org/docs/#migrating-from-htmx-2x-to-4x) recommends:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV4(config =>
    {
        config.NoSwap = ["204", "304", "4xx", "5xx"];
    });
});
```

This policy also prevents `422` responses from swapping. Omit or narrow the `4xx` pattern if those responses should
continue to update the page.

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

Instead of mapping an endpoint, the companion script can be embedded directly:

```html
<script>
    @Html.HtmxToolkitScript()
</script>
```

Pass `debug: true` to `HtmxToolkitScript` or `HtmxToolkitScriptPath` to use the readable script during development.
A custom endpoint path is also supported:

```csharp
app.MapHtmxToolkitScript("/assets/htmx-toolkit.js");
```

## Compatibility Notes

### Trigger Timing

> [!IMPORTANT]
> HTMX 1.9.x and 2.x support `HX-Trigger`, `HX-Trigger-After-Swap`, and `HX-Trigger-After-Settle`.
> HTMX 4.x supports only `HX-Trigger`, which fires when the request completes (after the swap whenever one is performed).
> HtmxToolkit therefore emits events requested for any `HtmxTriggerTiming`
> value through that header rather than dropping them. The `Receive` and `AfterSettle` timings cannot be preserved exactly.

### Polling

For server-controlled polling that works with every supported HTMX version, return the polling element itself and replace it with `outerHTML`:

```html
<div id="poll-status"
     hx-get="/poll"
     hx-trigger="load delay:1s"
     hx-swap="outerHTML">
    Polling...
</div>
```

Return the same element with its request attributes to continue polling,
or return it without `hx-get` and `hx-trigger` to stop. Status code `286` stops polling in HTMX 1.9.x and 2.x,
but HTMX 4.x treats it as a regular successful response.

### Morph Swaps

`HtmxSwap.InnerMorph` and `HtmxSwap.OuterMorph` use the native `innerMorph` and `outerMorph` swap styles in HTMX 4.x.
No additional client-side dependency or configuration is required.

With HTMX 1.9.x or 2.x, enable the `ramstack-morph` extension. To preserve morphing behavior, also load the optional
Idiomorph library before the first morph swap:

```html
<body hx-ext="ramstack-morph">
    <div id="profile">Current profile</div>

    <button hx-get="/profile/morph"
            hx-target="#profile"
            hx-swap="outerMorph">
        Refresh profile
    </button>

    <script src="https://unpkg.com/htmx.org@2"></script>
    <script src="https://unpkg.com/idiomorph@0.7.4"></script>
    <script>
        @Html.HtmxToolkitScript()
    </script>
</body>
```

The `/profile/morph` endpoint should return the replacement root, such as `<div id="profile">Updated profile</div>`.

The toolkit script does not bundle HTMX or Idiomorph and must be loaded after HTMX. Idiomorph remains an optional dependency
and may be loaded before or after the toolkit script because the adapter resolves it when each morph swap runs. Do not enable
the extension with HTMX 4.x, which handles these swap styles natively.

If Idiomorph is unavailable, the adapter logs a warning and falls back from `innerMorph` to `innerHTML` and from `outerMorph`
to `outerHTML`. With HTMX 1.9.x and 2.x, `outerSync` falls back to synchronizing the target's attributes and then replacing
its children using `innerHTML`.

`HtmxSwap.TextContent` is also handled by the `ramstack-morph` extension and does not require Idiomorph. It is supported natively
by HTMX 2.x and 4.x; only HTMX 1.9.x needs the extension.

## Sample

The [`samples/Ramstack.HtmxToolkit.Demo`](samples/Ramstack.HtmxToolkit.Demo) project demonstrates request detection,
response headers, Tag Helpers, polling, boosted navigation, and antiforgery integration.

Run it with:

```console
dotnet run --project samples/Ramstack.HtmxToolkit.Demo
```

## Contributing

Bug reports and pull requests are welcome. To validate a change locally:

```console
dotnet build
dotnet test
```

## License

HtmxToolkit is available under the [MIT License](LICENSE).
