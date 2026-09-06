# HTMX 4.x configuration

Select V4 explicitly when the application loads HTMX 4.x:

```csharp
using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV4(config =>
    {
        config.History = HtmxHistoryMode.Enabled;
        config.DefaultTimeout = 10_000;
        config.Mode = HtmxFetchMode.SameOrigin;
        config.Transitions = true;
        config.NoSwap = ["204", "304", "4xx", "5xx"];
    });
});
```

## Settings by purpose

| Purpose | Properties |
|---|---|
| Diagnostics and attribute prefix | `LogAll`, `Prefix` |
| History | `History` |
| Swaps and morphing | `DefaultSwap`, `AllowEmptySwapAfterOob`, `DefaultSettleDelay`, `MorphIgnore`, `MorphSkip`, `MorphSkipChildren`, `MorphScanLimit` |
| Indicators and lifecycle CSS | `IncludeIndicatorCss`, `IndicatorClass`, `RequestClass` |
| Script and extensions | `InlineScriptNonce`, `Extensions`, `ImplicitInheritance` |
| Requests | `DefaultTimeout`, `Mode` |
| Focus and transitions | `DefaultFocusScroll`, `Transitions` |
| Responses | `NoSwap` |

## Request mode

`Mode` maps to the Fetch API request mode:

| Value | Meaning |
|---|---|
| `SameOrigin` | Allow only same-origin requests |
| `Cors` | Allow cross-origin requests through CORS |
| `NoCors` | Allow restricted opaque cross-origin responses, which normally cannot be swapped |

Changing this setting does not configure CORS on the ASP.NET Core server.

## History behavior

HTMX 4 combines prior history settings into `History`:

```csharp
config.History = HtmxHistoryMode.Enabled;  // Request and swap the URL on history navigation.
config.History = HtmxHistoryMode.Disabled; // No HTMX history support.
config.History = HtmxHistoryMode.Reload;   // Perform a full page reload on history navigation.
```

Choose one value; the three assignments above illustrate alternatives.

Unlike HTMX 1.x and 2.x, HTMX 4.x does not keep local DOM snapshots in `sessionStorage` by default.
`Enabled` requests the history URL from the server and swaps the response.
The optional HTMX 4 `hx-history-cache` extension restores local snapshot behavior;
HtmxToolkit does not enable that extension automatically.

## Attribute modifiers

HTMX 4.x requires explicit attribute inheritance by default and replaces an inherited object when a child
declares the same attribute. HtmxToolkit provides Razor-friendly boolean inputs for the `inherited` and
`append` modifiers:

```razor
<section hx-request-inherited="true"
         hx-request-timeout="2000"
         hx-vals-inherited="true"
         hx-val-tenant="@Model.TenantId">
    <button hx-post="/reports/preview"
            hx-vals-append="true"
            hx-val-format="summary">
        Preview
    </button>
</section>
```

The parent inputs become `hx-config:inherited` and `hx-vals:inherited`. The button emits
`hx-vals:append`, which merges its values into the inherited object instead of replacing it.
The same inputs are available for all three generated JSON attributes:

| Razor Tag Helper input | Generated HTMX 4 attribute |
|---|---|
| `hx-request-inherited="true"` | `hx-config:inherited` |
| `hx-request-append="true"` | `hx-config:append` |
| `hx-vals-inherited="true"` | `hx-vals:inherited` |
| `hx-vals-append="true"` | `hx-vals:append` |
| `hx-headers-inherited="true"` | `hx-headers:inherited` |
| `hx-headers-append="true"` | `hx-headers:append` |

Setting both inputs for one attribute emits a combined modifier, for example
`hx-headers:inherited:append`.

Alternatively, enable the HTMX 1.x/2.x inheritance behavior globally:

```csharp
config.ImplicitInheritance = true;
```

Leave it unset or `false` when inheritance is selected per element with `hx-request-inherited`, `hx-vals-inherited`,
or `hx-headers-inherited`. Global inheritance removes the need for those `*-inherited` inputs, but child
declarations can still use `*-append` to merge with inherited objects.

## Prevent swaps for status codes

`NoSwap` accepts exact status codes and wildcard patterns:

```csharp
config.NoSwap = ["204", "304", "4xx", "5xx"];
```

The HTMX default contains 204 and 304. Assigning the property replaces that list, so preserve those entries
if the application still relies on their default behavior.

## Morphing

HTMX 4 natively supports `InnerMorph`, `OuterMorph`, and `OuterSync`. The morph settings control matching and exclusions:

```csharp
options.UseHtmxV4(config =>
{
    config.MorphSkip = "[data-preserve]";
    config.MorphSkipChildren = "[data-preserve-children]";
    config.MorphScanLimit = 20;
});
```

Use these exclusions for browser-managed or third-party widgets whose DOM state should not be reconciled.

Use the [API reference](../api/Ramstack.HtmxToolkit.Configuration.HtmxV4Config.yml) for property types and declared defaults.
