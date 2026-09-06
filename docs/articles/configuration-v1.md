# HTMX 1.x configuration

Select HTMX 1.x when the application loads an HTMX 1.9.x release:

```csharp
using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV1(config =>
    {
        config.HistoryCacheSize = 20;
        config.Timeout = 10_000;
        config.DefaultSwapStyle = HtmxSwap.InnerHtml;
        config.ScrollBehavior = HtmxScrollBehavior.Smooth;
    });
});
```

Only non-null settings are written to `<meta name="htmx-config">`.

## Settings by purpose

| Purpose | Properties |
|---|---|
| History | `HistoryEnabled`, `HistoryCacheSize`, `RefreshOnHistoryMiss` |
| Swaps | `DefaultSwapStyle`, `DefaultSwapDelay`, `DefaultSettleDelay`, `AttributesToSettle` |
| Indicators and lifecycle CSS | `IncludeIndicatorStyles`, `IndicatorClass`, `RequestClass`, `AddedClass`, `SwappingClass`, `SettlingClass` |
| Script and content security | `AllowEval`, `AllowScriptTags`, `InlineScriptNonce`, `DisableSelector` |
| Requests | `WithCredentials`, `Timeout`, `SelfRequestsOnly`, `MethodsThatUseUrlParams`, `GetCacheBusterParam` |
| Navigation and focus | `ScrollBehavior`, `DefaultFocusScroll`, `ScrollIntoViewOnBoost`, `IgnoreTitle` |
| Transitions and parsing | `GlobalViewTransitions`, `UseTemplateFragments`, `TriggerSpecsCacheEnabled` |
| WebSocket | `WsReconnectDelay`, `WsBinaryType` |

## Common profiles

Restrict requests to the current origin and prevent scripts returned in fragments from running:

```csharp
options.UseHtmxV1(config =>
{
    config.SelfRequestsOnly = true;
    config.AllowScriptTags = false;
});
```

Configure a finite request timeout and explicit history-miss behavior:

```csharp
options.UseHtmxV1(config =>
{
    config.Timeout = 15_000;
    config.RefreshOnHistoryMiss = true;
});
```

## Morphing compatibility

`HtmxSwap.InnerMorph`, `OuterMorph`, and `OuterSync` are native HTMX 4.x swap styles. For HTMX 1.9.x,
activate the `ramstack-morph` extension supplied by the Toolkit script. `InnerMorph` and `OuterMorph` can use Idiomorph
when the application loads it; otherwise they fall back to ordinary HTML replacement. `TextContent` also uses
the compatibility extension on HTMX 1.9.x.

```html
<body hx-ext="ramstack-morph">
    ...
</body>
```

Use the [API reference](../api/Ramstack.HtmxToolkit.Configuration.HtmxV1Config.yml) for property types and declared defaults,
and review [Version compatibility](version-compatibility.md) before migrating.
