# Version compatibility

HtmxToolkit supports HTMX 1.9.x, 2.x, and 4.x. Select the version loaded by the browser so Tag Helpers
and response events use the behavior expected by that version.

## Request headers

| Information         | HTMX 1.9.x / 2.x                                     | HTMX 4.x                                                | Toolkit property                   |
|---------------------|------------------------------------------------------|---------------------------------------------------------|------------------------------------|
| HTMX request marker | `HX-Request`, by default also on history restoration | `HX-Request`, except on history restoration             | `Request`                          |
| Boosted navigation  | `HX-Boosted`                                         | `HX-Boosted`                                            | `Boosted`                          |
| Current browser URL | `HX-Current-URL`                                     | `HX-Current-URL`                                        | `CurrentUrl`                       |
| History cache miss  | `HX-History-Restore-Request`                         | `HX-History-Restore-Request`                            | `HistoryRestoreRequest`            |
| Target element      | Element ID                                           | `tag#id`                                                | `Target`                           |
| Triggering element  | `HX-Trigger` and `HX-Trigger-Name`                   | `HX-Source` in `tag#id` form                            | `Trigger`, `TriggerName`, `Source` |
| Prompt result       | `HX-Prompt`                                          | `HX-Prompt` from the `hx-prompt` extension, URI-encoded | `Prompt`                           |
| Expected response   | `HX-Request-Type`, added by the Toolkit script       | `HX-Request-Type`: `partial` or `full`                  | `RequestType`                      |

`IsHtmxRequest()` returns `false` for history restoration in every version,
even when HTMX 1.9.x or 2.x sends `HX-Request` with it; see [Handle history restoration](requests.md#handle-history-restoration).
In HTMX 4.x, a target value such as `div#results` includes the tag name as well as the ID.
Code shared across versions should tolerate null for version-specific string properties.
`RequestType` is `HtmxRequestType.Unspecified` for HTMX 1.9.x and 2.x requests from a page without the Toolkit script.

```csharp
if (Request.IsHtmxRequest(out var htmx))
{
    var source = htmx.Source ?? htmx.Trigger;
    // Use source only when the endpoint actually needs it.
}
```

For HTMX 1.9.x and 2.x, the Toolkit script computes `HX-Request-Type` the same way HTMX 4.x does;
see [Check full or partial requests](requests.md#check-full-or-partial-requests).
The result can still differ between versions for the same markup, because the versions resolve attributes differently:
HTMX 4.x, for example, does not inherit `hx-select` from a parent element unless it is marked as inherited.

## Per-request options

`HtmxRequestTagHelper` uses one Razor-facing API and emits the appropriate client attribute:

| Option                                           | HTMX 1.9.x / 2.x                     | HTMX 4.x                            |
|--------------------------------------------------|--------------------------------------|-------------------------------------|
| Output attribute                                 | `hx-request`                         | `hx-config`                         |
| `hx-request-timeout`                             | Supported                            | Supported                           |
| `hx-request-credentials`                         | `true` or `false`; `Omit` is omitted | `same-origin`, `include`, or `omit` |
| `hx-request-no-headers`                          | Supported                            | Removed                             |
| Cache, redirect, referrer, integrity, validation | Not emitted                          | Supported                           |

## Attribute modifiers

HTMX 1.9.x and 2.x inherit request configuration, values, and headers automatically.
Child values override matching parent values and keep the rest.
HTMX 4.x requires explicit `inherited` and `append` modifiers. The Tag Helpers accept Razor-friendly inputs
such as `hx-vals-inherited="true"` and generate the modifiers only for HTMX 4.x, so the same markup works
with every version. See [Apply attribute modifiers](tag-helpers.md#apply-attribute-modifiers).

`HtmxV4Config.ImplicitInheritance = true` is the global alternative to per-element `*-inherited` inputs.
It does not replace `*-append` when a child declaration must merge with inherited values.

## Global configuration concepts

| Concept               | HTMX 1.9.x                                                                      | HTMX 2.x                                       | HTMX 4.x                                                                  |
|-----------------------|---------------------------------------------------------------------------------|------------------------------------------------|---------------------------------------------------------------------------|
| History               | Local snapshots controlled by `HistoryEnabled`, cache size, and refresh on miss | Local snapshots, plus restore-request behavior | `History` mode; server request by default, optional local-cache extension |
| Default swap          | `DefaultSwapStyle`                                                              | `DefaultSwapStyle`                             | `DefaultSwap`                                                             |
| Same-origin policy    | `SelfRequestsOnly`                                                              | `SelfRequestsOnly`                             | `Mode`                                                                    |
| View transitions      | `GlobalViewTransitions`                                                         | `GlobalViewTransitions`                        | `Transitions`                                                             |
| Response statuses     | HTMX defaults                                                                   | `ResponseHandling`                             | `NoSwap`                                                                  |
| Attribute inheritance | Standard behavior                                                               | `DisableInheritance`                           | `ImplicitInheritance`                                                     |
| Morphing              | Toolkit extension                                                               | Toolkit extension                              | Native                                                                    |

The similarly named settings are not always exact one-to-one replacements.
Review the target version's guide rather than copying a configuration delegate unchanged.

## Response events

HTMX 1.9.x and 2.x distinguish `HX-Trigger`, `HX-Trigger-After-Swap`, and `HX-Trigger-After-Settle`.
HTMX 4.x has only `HX-Trigger`, so the Toolkit emits events of every `HtmxTriggerTiming` through it.

| Timing        | HTMX 1.9.x and 2.x                                     | HTMX 4.x                   |
|---------------|--------------------------------------------------------|----------------------------|
| `Receive`     | When the response is received, before the swap         | When the request completes |
| `AfterSwap`   | After the swap step, only if the response is swapped   | When the request completes |
| `AfterSettle` | After the settle step, only if the response is swapped | When the request completes |

HTMX 4.x completes the request after the swap and settle steps, so `AfterSwap` and `AfterSettle` events
still fire after the DOM is updated. Two differences remain:

- `Receive` events no longer fire before the swap. To act before the swap, handle `htmx:before:swap` on the client.
- `AfterSwap` and `AfterSettle` events also fire when the response is not swapped,
  for example with status code 204 or an `HX-Location` header.

Because all timings share one header, events fire in call order regardless of timing, and a name added
with different timings counts as repeated. See [Timing and repeated events](responses.md#timing-and-repeated-events).

## Polling

Status code `286` stops polling in HTMX 1.9.x and 2.x. HTMX 4.x treats it as a regular successful response.
For a cross-version approach, return the polling element with `hx-trigger="load delay:1s"` and `hx-swap="outerHTML"`
to schedule the next request, then omit the request and trigger attributes when polling should stop.
See [Poll a background operation](recipes.md#poll-a-background-operation) for a complete example.

## Morph swaps

`HtmxSwap.InnerMorph`, `OuterMorph`, and `OuterSync` use native swap styles in HTMX 4.x.
Do not enable the `ramstack-swap` extension with that version.

With HTMX 1.9.x or 2.x, enable the extension supplied by the Toolkit script. Load the optional Idiomorph library
before the first morph swap to preserve morphing behavior:

```razor
<body hx-ext="ramstack-swap">
    <div id="profile">Current profile</div>

    <button hx-get="/profile/morph"
            hx-target="#profile"
            hx-swap="outerMorph">
        Refresh profile
    </button>

    <script src="https://cdn.jsdelivr.net/npm/htmx.org@2.0.11/dist/htmx.min.js"
            integrity="sha384-2OatzQy1H+Zd/IIrjr1TcuDGqLXeHhbooAyJY1KdQMKnr4LZ22k31GBLdYKHmVjg"
            crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/idiomorph@0.7.4/dist/idiomorph.min.js"
            integrity="sha384-YWh9vmu1Q6BptPp1emqfsNatFASVLolFKP/5f4LWR6eGMLEhBDh1zcwU2OzfVCe9"
            crossorigin="anonymous"></script>
    <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
            asp-append-version="true"></script>
</body>
```

The `/profile/morph` endpoint should return the replacement root, such as `<div id="profile">Updated profile</div>`.

The Toolkit script does not bundle HTMX or Idiomorph and must be loaded after HTMX. Idiomorph may be loaded
before or after the Toolkit script because the adapter resolves it when each morph swap runs.

If Idiomorph is unavailable, the adapter logs a warning and falls back from `innerMorph` to `innerHTML` and from
`outerMorph` to `outerHTML`. With HTMX 1.9.x and 2.x, `outerSync` falls back to synchronizing the target's attributes
and then replacing its children using `innerHTML`.

`HtmxSwap.TextContent` is also handled by the `ramstack-swap` extension and does not require Idiomorph.
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
