# HTMX 2.x configuration

HTMX 2.x is the default target. Calling `UseHtmxV2` is optional unless settings need to be changed:

```csharp
using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.HistoryCacheSize = 20;
        config.Timeout = 10_000;
        config.GlobalViewTransitions = true;
        config.ReportValidityOfForms = true;
    });
});
```

## Settings by purpose

| Purpose | Properties |
|---|---|
| History | `HistoryEnabled`, `HistoryCacheSize`, `RefreshOnHistoryMiss`, `HistoryRestoreAsHxRequest` |
| Swaps | `DefaultSwapStyle`, `DefaultSwapDelay`, `DefaultSettleDelay`, `AttributesToSettle`, `AllowNestedOobSwaps` |
| Indicators and lifecycle CSS | `IncludeIndicatorStyles`, `IndicatorClass`, `RequestClass`, `AddedClass`, `SwappingClass`, `SettlingClass` |
| Script and content security | `AllowEval`, `AllowScriptTags`, `InlineScriptNonce`, `InlineStyleNonce`, `DisableSelector` |
| Requests | `WithCredentials`, `DisableInheritance`, `Timeout`, `SelfRequestsOnly`, `MethodsThatUseUrlParams`, `GetCacheBusterParam` |
| Navigation and focus | `ScrollBehavior`, `DefaultFocusScroll`, `ScrollIntoViewOnBoost`, `IgnoreTitle` |
| Transitions and forms | `GlobalViewTransitions`, `TriggerSpecsCacheEnabled`, `ReportValidityOfForms` |
| Responses | `ResponseHandling` |
| WebSocket | `WsReconnectDelay`, `WsBinaryType` |

## Handle validation responses

`ResponseHandling` is an ordered collection of regular-expression rules. Assigning it replaces HTMX's response-handling configuration,
so include rules for successful and error responses as well as the special response being added.

The following example swaps a validation fragment returned with status 422, treats successful responses normally,
and leaves other client and server errors unswapped:

```csharp
options.UseHtmxV2(config =>
{
    config.ResponseHandling =
    [
        new() { Code = "204", Swap = false },
        new() { Code = "304", Swap = false },
        new() { Code = "[23]..", Swap = true },
        new() { Code = "422", Swap = true, Error = false },
        new() { Code = "[45]..", Swap = false, Error = true }
    ];
});
```

Place a more specific rule before a broader expression when both can match.

## Harden fragment processing

An application that does not return executable scripts in fragments can disable script processing:

```csharp
options.UseHtmxV2(config =>
{
    config.AllowScriptTags = false;
    config.SelfRequestsOnly = true;
});
```

These client settings complement server-side validation, authorization, CORS, and content security policy; they do not replace them.

## Morphing compatibility

HTMX 2.x supports `TextContent` natively. To use `InnerMorph`, `OuterMorph`, or `OuterSync`,
activate the `ramstack-morph` extension supplied by the Toolkit script. Load Idiomorph when true morphing is required;
otherwise the extension uses its documented fallback behavior.

```html
<main hx-ext="ramstack-morph">
    ...
</main>
```

Use the [API reference](../api/Ramstack.HtmxToolkit.Configuration.HtmxV2Config.yml) for property types and declared defaults.
