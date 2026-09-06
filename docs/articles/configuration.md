# Application configuration

Application-wide settings are registered with `AddHtmxToolkit` and rendered into the page by `<htmx-config />`.
HtmxToolkit uses a different strongly typed configuration class for each supported HTMX major version.

## Configure HTMX

```csharp
using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.DefaultSwapStyle = HtmxSwap.OuterHtml;
        config.HistoryCacheSize = 20;
        config.Timeout = 10_000;
        config.ScrollBehavior = HtmxScrollBehavior.Smooth;
    });
});
```

Render the selected configuration once in the document head:

```html
<head>
    <htmx-config />
</head>
```

The Tag Helper produces a meta element similar to this:

```html
<meta name="htmx-config"
      content='{"historyCacheSize":20,"defaultSwapStyle":"outerHTML","timeout":10000,"scrollBehavior":"smooth"}'
      data-antiforgery-request-token="..."
      data-antiforgery-header-name="RequestVerificationToken"
      data-antiforgery-form-field-name="__RequestVerificationToken" />
```

The exact JSON property names depend on the selected HTMX version.
Antiforgery attribute names depend on the ASP.NET Core antiforgery configuration.

## Preserve HTMX defaults

Configuration properties are nullable. Leave a property `null` when the application does not need to override the HTMX default:

```csharp
options.UseHtmxV2(config =>
{
    config.Timeout = 5_000;       // Emitted.
    config.HistoryCacheSize = null; // Omitted; HTMX keeps its default.
});
```

This keeps generated markup small and avoids accidentally freezing an upstream default that the application does not depend on.

## Configure response status handling

HTMX 2.x exposes ordered `ResponseHandling` rules. HTMX 4.x exposes `NoSwap` status patterns instead.
These settings can change whether error responses are inserted into the document, so configure them together
with the application's error-response strategy.

See [HTMX 2.x configuration](configuration-v2.md) for a complete rule set and [HTMX 4.x configuration](configuration-v4.md) for `NoSwap`.

## Access the selected configuration

Code that needs the version-specific options can resolve `IOptions<HtmxToolkitOptions>` and request the expected configuration type:

```csharp
using Microsoft.Extensions.Options;
using Ramstack.HtmxToolkit.Configuration;

public sealed class HtmxDiagnostics(IOptions<HtmxToolkitOptions> options)
{
    private readonly HtmxV2Config _config =
        options.Value.GetHtmxConfig<HtmxV2Config>();
}
```

`GetHtmxConfig<TConfig>()` throws when `TConfig` does not match the selected version.
Most applications do not need to access the configuration after registration.

## Antiforgery metadata

`<htmx-config />` includes antiforgery metadata by default. Disable it only when the application attaches tokens itself:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = false;
    options.UseHtmxV2();
});
```

See [Antiforgery and Toolkit script](antiforgery.md) before disabling this behavior.

## Version-specific settings

- [HTMX 1.x configuration](configuration-v1.md)
- [HTMX 2.x configuration](configuration-v2.md)
- [HTMX 4.x configuration](configuration-v4.md)
- [Version compatibility](version-compatibility.md)
