# Version compatibility

HtmxToolkit supports HTMX 1.9.x, 2.x, and 4.x. Select the version loaded by the browser so Tag Helpers
and the companion script generate the matching contract.

## Request headers

| Information | HTMX 1.x / 2.x | HTMX 4.x | Toolkit property |
|---|---|---|---|
| HTMX request marker | `HX-Request` | `HX-Request` | `Request` |
| Boosted navigation | `HX-Boosted` | `HX-Boosted` | `Boosted` |
| Current browser URL | `HX-Current-URL` | `HX-Current-URL` | `CurrentUrl` |
| History cache miss | `HX-History-Restore-Request` | `HX-History-Restore-Request` | `HistoryRestoreRequest` |
| Target element | Element ID | `tag#id` | `Target` |
| Triggering element | `HX-Trigger` and `HX-Trigger-Name` | `HX-Source` in `tag#id` form | `Trigger`, `TriggerName`, `Source` |
| Prompt result | `HX-Prompt` | Not supported | `Prompt` |
| Expected response | Not reported | `HX-Request-Type`: `partial` or `full` | `RequestType` |

In HTMX 4.x, a target value such as `div#results` includes the tag name as well as the ID.
Code shared across versions should tolerate null for version-specific properties.

```csharp
if (Request.IsHtmxRequest(out var htmx))
{
    var source = htmx.Source ?? htmx.Trigger;
    // Use source only when the endpoint actually needs it.
}
```

## Per-request options

`HtmxRequestTagHelper` uses one Razor-facing API and emits the appropriate client attribute:

| Option | HTMX 1.x / 2.x | HTMX 4.x |
|---|---|---|
| Output attribute | `hx-request` | `hx-config` |
| `hx-request-timeout` | Supported | Supported |
| `hx-request-credentials` | `true` or `false`; `Omit` is omitted | `same-origin`, `include`, or `omit` |
| `hx-request-no-headers` | Supported | Removed |
| Cache, redirect, referrer, integrity, validation | Not emitted | Supported |

## Attribute modifiers

HTMX 1.x and 2.x merge-inherit request configuration, values, and headers automatically.
For HTMX 4.x, HtmxToolkit maps Razor-friendly inputs to the explicit inheritance and append modifiers:

| Razor input | HTMX 1.x / 2.x output | HTMX 4.x output |
|---|---|---|
| `hx-request-inherited="true"` | `hx-request` | `hx-config:inherited` |
| `hx-request-append="true"` | `hx-request` | `hx-config:append` |
| `hx-vals-inherited="true"` | `hx-vals` | `hx-vals:inherited` |
| `hx-vals-append="true"` | `hx-vals` | `hx-vals:append` |
| `hx-headers-inherited="true"` | `hx-headers` | `hx-headers:inherited` |
| `hx-headers-append="true"` | `hx-headers` | `hx-headers:append` |

In V4, `append` merges a child declaration into the inherited object rather than replacing it.
Setting both inputs for the same attribute produces one combined name such as
`hx-vals:inherited:append`. In V1 and V2, both inputs leave the ordinary merge-inherited attribute name unchanged.

`HtmxV4Config.ImplicitInheritance = true` is the global alternative to per-element `*-inherited` inputs.
It does not replace `*-append` when a child declaration must merge with inherited values.

## Global configuration concepts

| Concept | HTMX 1.x | HTMX 2.x | HTMX 4.x |
|---|---|---|---|
| History | Local snapshots controlled by `HistoryEnabled`, cache size, and refresh on miss | Local snapshots, plus restore-request behavior | `History` mode; server request by default, optional local-cache extension |
| Default swap | `DefaultSwapStyle` | `DefaultSwapStyle` | `DefaultSwap` |
| Same-origin policy | `SelfRequestsOnly` | `SelfRequestsOnly` | `Mode` |
| View transitions | `GlobalViewTransitions` | `GlobalViewTransitions` | `Transitions` |
| Response statuses | HTMX defaults | `ResponseHandling` | `NoSwap` |
| Attribute inheritance | Standard behavior | `DisableInheritance` | `ImplicitInheritance` |
| Morphing | Toolkit extension | Toolkit extension | Native |

The similarly named settings are not always exact one-to-one replacements.
Review the target version's guide rather than copying a configuration delegate unchanged.

## Response events

HTMX 1.x and 2.x distinguish `HX-Trigger`, `HX-Trigger-After-Swap`, and `HX-Trigger-After-Settle`.
HTMX 4.x emits Toolkit events through `HX-Trigger` when the request completes.
An `AfterSettle` timing therefore cannot retain its separate V1/V2 timing under V4.

## Polling

Status code `286` stops polling in HTMX 1.9.x and 2.x. HTMX 4.x treats it as a regular successful response.
For a cross-version approach, return the polling element with `hx-trigger="load delay:1s"` and `hx-swap="outerHTML"`
to schedule the next request, then omit the request and trigger attributes when polling should stop.
See [Poll a background operation](recipes.md#poll-a-background-operation) for a complete example.

## Morph swaps

`HtmxSwap.InnerMorph`, `OuterMorph`, and `OuterSync` use native swap styles in HTMX 4.x.
Do not enable the `ramstack-morph` extension with that version.

With HTMX 1.9.x or 2.x, enable the extension supplied by the Toolkit script. Load the optional Idiomorph library
before the first morph swap to preserve morphing behavior:

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
    <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js" asp-append-version="true"></script>
</body>
```

The `/profile/morph` endpoint should return the replacement root, such as `<div id="profile">Updated profile</div>`.

The Toolkit script does not bundle HTMX or Idiomorph and must be loaded after HTMX. Idiomorph may be loaded
before or after the Toolkit script because the adapter resolves it when each morph swap runs.

If Idiomorph is unavailable, the adapter logs a warning and falls back from `innerMorph` to `innerHTML` and from
`outerMorph` to `outerHTML`. With HTMX 1.9.x and 2.x, `outerSync` falls back to synchronizing the target's attributes
and then replacing its children using `innerHTML`.

`HtmxSwap.TextContent` is also handled by the `ramstack-morph` extension and does not require Idiomorph.
It is supported natively by HTMX 2.x and 4.x; only HTMX 1.9.x needs the extension.

## Migration checklist

1. Update the HTMX client script.
2. Change `UseHtmxV1`, `UseHtmxV2`, or `UseHtmxV4` to match it.
3. Replace global configuration properties that do not exist in the target configuration type.
4. Inspect rendered `htmx-config` metadata and `hx-request` or `hx-config` attributes.
5. Review code that reads `Prompt`, `Trigger`, `TriggerName`, `Source`, `Target`, or `RequestType`.
6. Test history restoration, boosted navigation, response events, error responses, and morph swaps.
7. Check browser developer tools for unexpected request or response headers.

See [Choose an HTMX version](choosing-version.md) and the [V1](configuration-v1.md), [V2](configuration-v2.md),
or [V4](configuration-v4.md) configuration guide.
