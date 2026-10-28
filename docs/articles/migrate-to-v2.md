# Migrate from 1.x to 2.0

HtmxToolkit 2.0 adds HTMX 2.x and 4.x support next to HTMX 1.9.x. Supporting three major versions meant
redesigning the parts of 1.x that assumed one: global configuration became version-specific, the Toolkit script
became a static web asset that all versions use, and several response APIs were renamed or removed
because they could not behave the same way across versions.

Most of the changes show up as build errors. Some do not: the old Razor attributes are rendered unchanged,
and some runtime behavior changed. These changes are marked as **silent** below. Check them even when
the application builds.

This guide covers upgrading the library. Moving the application to another HTMX major version is a separate
step; see [Version compatibility](version-compatibility.md) for that.

## Summary

| Area                                                                | Change                                                                              | Detected by       |
|---------------------------------------------------------------------|-------------------------------------------------------------------------------------|-------------------|
| [Registration](#register-the-toolkit-and-select-the-htmx-version)   | Call `AddHtmxToolkit()` and select the HTMX version; HTMX 2.x is the default        | **Silent**        |
| [Global configuration](#move-global-configuration-to-code)          | `<htmx-config>` attributes move to `AddHtmxToolkit()`                               | **Silent**        |
| [Antiforgery](#antiforgery-metadata-is-on-by-default)               | Metadata is included by default and no longer stored in `htmx.config`               | **Silent**        |
| [Toolkit script](#load-the-toolkit-script-as-a-static-asset)        | Inline script helpers and `MapHtmxAntiforgeryScript` replaced by a static web asset | Build             |
| [URL Tag Helper](#rename-the-url-tag-helper-attributes)             | `hx-*` URL attributes become `asp-*`; `hx-host` and `hx-protocol` are removed       | **Silent**        |
| [Requests](#update-request-handling)                                | `IsHtmxRequest()` is `false` for history restoration; `Boosted` filter renamed      | Build, **silent** |
| [Action result wrappers](#configure-responses-through-responsehtmx) | `IActionResult.Htmx(...)` removed                                                   | Build             |
| [Polling](#replace-stoppolling)                                     | `StopPolling` removed                                                               | Build             |
| [History](#update-history-calls)                                    | `PreventPushUrl` and `PreventReplaceUrl` become `PreventHistoryUpdate`              | Build             |
| [HX-Location](#replace-ajaxcontext-with-htmxlocationoptions)        | `AjaxContext` becomes `HtmxLocationOptions`                                         | Build             |
| [Client events](#update-client-events)                              | Event names and detail dictionary keys keep their case; repeated names are kept     | Build, **silent** |
| [Typed response headers](#update-typed-response-headers)            | `HtmxResponseHeaders` is a struct; event properties are read-only                   | Build             |
| [`[HtmxResponse]`](#update-htmxresponse)                            | Restricted to classes and methods, one per target, inherited                        | Build, **silent** |

## Update the package

```console
dotnet add package Ramstack.HtmxToolkit --version 2.0.0
```

The package still targets .NET 6 and runs on .NET 6 or later.

Add these namespaces where needed:

| Namespace                            | Contains                                                                                                                                                     |
|--------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Ramstack.HtmxToolkit.Hosting`       | `AddHtmxToolkit`                                                                                                                                             |
| `Ramstack.HtmxToolkit.Configuration` | `HtmxToolkitOptions`, `HtmxV1Config`, `HtmxV2Config`, `HtmxV4Config`, and configuration enums such as `HtmxScrollBehavior`, `HtmxBinaryType`, and `HttpVerb` |

`HtmxScrollBehavior` and `HtmxBinaryType` moved from `Ramstack.HtmxToolkit` to `Ramstack.HtmxToolkit.Configuration`.

## Register the toolkit and select the HTMX version

Version 1.x had no registration and was written for HTMX 1.9.x. Version 2.0 registers its options with `AddHtmxToolkit()`
and generates markup for the selected HTMX version. **The default is HTMX 2.x.**

If the application still loads HTMX 1.9.x, select it explicitly:

```csharp
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit(options => options.UseHtmxV1());
```

For HTMX 2.x, `builder.Services.AddHtmxToolkit()` is enough. The selected version controls the configuration
properties available in code and the attributes generated by the Tag Helpers.
See [Choose an HTMX version](choosing-version.md).

## Move global configuration to code

In 1.x, `<meta htmx-config>` and `<htmx-config>` accepted HTMX settings as attributes.
In 2.0, the settings are strongly typed properties of the version-specific configuration, and
the Tag Helper takes no settings.

1.x:

```razor
<htmx-config default-swap-style="HtmxSwap.OuterHtml"
             scroll-behavior="HtmxScrollBehavior.Smooth"
             history-cache-size="20"
             include-antiforgery-token="true" />
```

2.0:

```csharp
using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.DefaultSwapStyle = HtmxSwap.OuterHtml;
        config.ScrollBehavior = HtmxScrollBehavior.Smooth;
        config.HistoryCacheSize = 20;
    });
});
```

```razor
<head>
    <htmx-config />
</head>
```

> [!WARNING]
> The old attributes do not cause build errors. The Tag Helper no longer binds them, so Razor renders them
> unchanged on the `<meta>` element and HTMX ignores them. Search the layouts for `htmx-config` and move every
> attribute to code.

Each attribute maps to the property with the same name in PascalCase, such as `history-cache-size` to `HistoryCacheSize`,
on `HtmxV1Config` or `HtmxV2Config`. The exceptions are:

| 1.x attribute                 | 2.0                                                                                                                |
|-------------------------------|--------------------------------------------------------------------------------------------------------------------|
| `include-antiforgery-token`   | `HtmxToolkitOptions.IncludeAntiforgeryToken`, now `true` by default; see the next section                          |
| `use-template-fragments`      | `HtmxV1Config.UseTemplateFragments` only; HTMX 2.x always parses responses with templates                          |
| `trigger-specs-cache`         | `TriggerSpecsCacheEnabled` (`bool?`)                                                                               |
| `methods-that-use-url-params` | `MethodsThatUseUrlParams` (`HttpVerb[]` instead of `string[]`)                                                     |
| `ws-reconnect-delay`          | Removed: HTMX accepts only a function or the default `"full-jitter"`, and a function cannot be passed through JSON |

For every property, see [HTMX 1.9.x](configuration-v1.md), [HTMX 2.x](configuration-v2.md),
or [HTMX 4.x](configuration-v4.md) configuration.

### Antiforgery metadata is on by default

`<htmx-config />` now includes antiforgery metadata unless you opt out. In 1.x, it did so only with
`include-antiforgery-token="true"`.

> [!WARNING]
> To include the metadata, ASP.NET Core generates antiforgery tokens for every page that renders `<htmx-config />`.
> The response then sets the antiforgery cookie, `Cache-Control: no-cache, no-store`, and `X-Frame-Options: SAMEORIGIN`.
> If such a page was publicly cacheable in 1.x and the application does not send HTMX requests that need a token,
> disable the metadata:
>
> ```csharp
> builder.Services.AddHtmxToolkit(options => options.IncludeAntiforgeryToken = false);
> ```

The metadata also moved. In 1.x it was an `antiForgery` object inside the configuration JSON, and the script
copied it to `htmx.config.antiForgery`. In 2.0 it is written as data attributes, and `htmx.config.antiForgery`
is no longer set:

```html
<meta name="htmx-config"
      content='{...}'
      data-antiforgery-request-token="..."
      data-antiforgery-header-name="RequestVerificationToken"
      data-antiforgery-form-field-name="__RequestVerificationToken" />
```

The Toolkit script reads these attributes itself. Update only custom JavaScript that read `htmx.config.antiForgery`:

```js
const { antiforgeryRequestToken, antiforgeryHeaderName } =
    document.querySelector("meta[name='htmx-config']").dataset;
```

See [Antiforgery and Toolkit script](antiforgery.md).

## Load the Toolkit script as a static asset

These 1.x APIs were removed:

| 1.x                                           | 2.0                                                   |
|-----------------------------------------------|-------------------------------------------------------|
| `@Html.HtmxAntiforgeryScript()` inline script | Static web asset loaded with `<script src>`           |
| `@Html.HtmxAntiforgeryScriptPath()`           | `~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js` |
| `app.MapHtmxAntiforgeryScript()`              | `app.UseStaticFiles()` or `app.MapStaticAssets()`     |
| `HtmxAssets.Script`, `DebugScript`, `Hash`    | Removed                                               |
| `debug: true`                                 | `htmx-toolkit.js` instead of `htmx-toolkit.min.js`    |

1.x:

```razor
<script src="~/js/htmx.min.js"></script>
<script>
    @Html.HtmxAntiforgeryScript()
</script>
```

2.0:

```razor
<script src="~/js/htmx.min.js"></script>
<script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
        asp-append-version="true"></script>
```

Remove the `MapHtmxAntiforgeryScript()` call and make sure the application serves static files:

```csharp
app.UseStaticFiles();
```

On ASP.NET Core 9 or later, you can use `app.MapStaticAssets()` with `.WithStaticAssets()` instead.
`asp-append-version` and `~/` require the standard Tag Helpers:
`@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`.

Because the script is no longer inline, a Content Security Policy no longer needs `'unsafe-inline'` or a nonce for it.

Load the script on every page that uses HTMX, even when the application does not use automatic antiforgery.
In 2.0 it also sends `HX-Request-Type` for HTMX 1.9.x and 2.x, replays events whose name repeats,
and provides the `ramstack-swap` extension. See [What the Toolkit script does](antiforgery.md#what-the-toolkit-script-does)
and [Static assets and deployment](static-assets.md).

## Rename the URL Tag Helper attributes

The URL Tag Helper now reads the standard ASP.NET Core `asp-*` routing attributes instead of its own `hx-*` copies.
HTMX 4.x defines a native `hx-action` attribute, which made the old naming conflict with HTMX itself.

| 1.x                 | 2.0                  |
|---------------------|----------------------|
| `hx-controller`     | `asp-controller`     |
| `hx-action`         | `asp-action`         |
| `hx-area`           | `asp-area`           |
| `hx-page`           | `asp-page`           |
| `hx-page-handler`   | `asp-page-handler`   |
| `hx-route`          | `asp-route`          |
| `hx-route-*`        | `asp-route-*`        |
| `hx-all-route-data` | `asp-all-route-data` |
| `hx-host`           | Removed              |
| `hx-protocol`       | Removed              |
| `hx-fragment`       | `asp-fragment`       |

The HTMX request attribute is now required and must be written without a value. In 1.x, `hx-get` was used
when no request attribute was present.

1.x:

```razor
<button hx-controller="Speaker"
        hx-action="Detail"
        hx-route-id="@Model.SpeakerId">Show info</button>

<button hx-post
        hx-page="/Attendee"
        hx-page-handler="Profile">Save</button>
```

2.0:

```razor
<button hx-get
        asp-controller="Speaker"
        asp-action="Detail"
        asp-route-id="@Model.SpeakerId">Show info</button>

<button hx-post
        asp-page="/Attendee"
        asp-page-handler="Profile">Save</button>
```

> [!WARNING]
> Old attributes do not cause build errors. Razor renders them unchanged, and HTMX 1.9.x and 2.x ignore them,
> so the element sends no request. HTMX 4.x reads a leftover `hx-action="Detail"` as the request URL `Detail`.

Find the old attributes from the project directory. On Linux and macOS:

```console
grep -rniE '\bhx-(controller|action|area|page|route|all-route-data|host|protocol|fragment)\b' --include='*.cshtml' .
```

In PowerShell:

```powershell
Get-ChildItem -Recurse -Filter *.cshtml |
    Select-String -Pattern '\bhx-(controller|action|area|page|route|all-route-data|host|protocol|fragment)\b'
```

In HTMX 4.x markup, `hx-action` can be legitimate; review those matches.

Check the following while migrating:

- **Route values are strings.** In 1.x, `hx-route-*` values were typed as `object`, so Razor compiled them as C# code.
  `asp-route-*` values are strings, as in the built-in Tag Helpers: `hx-route-id="Model.Id"` must become
  `asp-route-id="@Model.Id"`. Without `@`, the value is the literal text `Model.Id`.
- **`asp-all-route-data` takes `IDictionary<string, string>`**, like the built-in Tag Helpers,
  instead of `RouteValueDictionary`. Convert other values to strings.
- **Built-in Tag Helpers process the same attributes** on `<a>`, `<form>`, `<button>`, and submit or image `<input>`
  elements and also render `href`, `action`, or `formaction`. Add `method="get"` to a `<form hx-get>`;
  otherwise the Form Tag Helper renders `method="post"` and an antiforgery field.
  See [Links, forms, and buttons](tag-helpers.md#links-forms-and-buttons).
- **An HTMX request attribute with a value throws** when `asp-*` attributes are present, even an empty one such as `hx-get=""`.
  An `hx-get="/url"` without `asp-*` attributes is not affected.
- **`hx-host` and `hx-protocol` have no replacement.** They only made the URL absolute, and HTMX 2.x and 4.x
  block cross-origin requests by default. For a request to another origin, allow it in the HTMX configuration
  and write the URL directly, for example
  `hx-get="@Url.Action("Detail", "Speaker", new { id = Model.SpeakerId }, "https", "api.example.com")"`.
- **The Tag Helper order** changed from `-1000` to `-950`, so it runs after the built-in Tag Helpers.
  This matters only to custom Tag Helpers that depend on that order.

## Update request handling

### History restoration is not an HTMX request

> [!WARNING]
> `IsHtmxRequest()` now returns `false` when `HX-History-Restore-Request` is `true`. This is a silent behavior change.

HTMX treats history restoration as a full page load: HTMX 4.x omits `HX-Request` for it, but HTMX 1.9.x always sends it,
and HTMX 2.x does by default.
With the common `IsHtmxRequest() ? Partial(...) : View()` pattern, HTMX 1.9.x and 2.x received a fragment
instead of the page after Back or Forward. In 2.0, the same code returns the full page with every HTMX version.

`IsHtmxRequest(out headers)`, `[HtmxRequest]`, `Response.Htmx(...)`, and `[HtmxResponse]` follow the same rule.
HTMX 1.9.x and 2.x ignore response headers when restoring history, so skipping them loses nothing.
To read the raw header, use `Request.GetHtmxHeaders().Request`. See [Handle history restoration](requests.md#handle-history-restoration).

### Boosted requests

| 1.x                                      | 2.0                                                           |
|------------------------------------------|---------------------------------------------------------------|
| `[HtmxRequest(Boosted = true)]`          | `[HtmxRequest(BoostedFilter = HtmxBoostedFilter.Boosted)]`    |
| `[HtmxRequest(Boosted = false)]`         | `[HtmxRequest(BoostedFilter = HtmxBoostedFilter.NonBoosted)]` |
| `Request.IsHtmxBoosted(out var headers)` | `Request.IsHtmxBoosted()` and `Request.GetHtmxHeaders()`      |

In 1.1 through 1.2.2, `Boosted` was a `bool?`, which C# cannot set in an attribute, so `Boosted = true` did not compile.
The `HtmxBoostedFilter` enum keeps the three choices: `Any`, `Boosted`, and `NonBoosted`.

`[HtmxRequest]` can also select actions by `RequestType`. See [Select MVC actions declaratively](requests.md#select-mvc-actions-declaratively).

## Update response code

### Configure responses through Response.Htmx

The `Htmx(...)` extension methods for `IActionResult`, `HtmxResult`, and `HtmxResult<TState>` were removed.
`Response.Htmx(...)` works the same way in MVC, Razor Pages, and Minimal APIs, while wrapping results would have needed
a separate family of `IResult` wrappers that preserves endpoint and OpenAPI metadata.

1.x:

```csharp
return PartialView("_Row", item).Htmx(htmx => htmx.Retarget("#rows"));
```

2.0:

```csharp
Response.Htmx(htmx => htmx.Retarget("#rows"));
return PartialView("_Row", item);
```

Like the removed wrappers, `Response.Htmx(...)` runs the callback only for HTMX requests.

### Replace StopPolling

`HtmxResponse.StopPolling()`, `StopPolling(bool)`, `HtmxResponse.StopPollingStatusCode`, and
`HtmxResponseAttribute.StopPolling` were removed. They relied on status code `286`, which HTMX 4.x
treats as an ordinary successful response.

For polling that works with every HTMX version, return the polling element without its request and trigger
attributes when polling should stop. See [Poll a background operation](recipes.md#poll-a-background-operation).
An application that stays on HTMX 1.9.x or 2.x can still set the status code itself:

```csharp
Response.StatusCode = 286;
```

### Update history calls

| 1.x                      | 2.0                      |
|--------------------------|--------------------------|
| `PreventPushUrl()`       | `PreventHistoryUpdate()` |
| `PreventReplaceUrl()`    | `PreventHistoryUpdate()` |
| `PushUrl(value: ...)`    | `PushUrl(url: ...)`      |
| `ReplaceUrl(value: ...)` | `ReplaceUrl(url: ...)`   |

In 1.x, `PushUrl` and `ReplaceUrl` set their headers independently, and HTMX versions resolved the combination
differently. In 2.0, each history method removes the headers set by the others, so the last call takes effect.
`PushUrl` and `ReplaceUrl` throw `ArgumentException` for a `null` or empty URL.
See [History updates](responses.md#history-updates).

### Replace AjaxContext with HtmxLocationOptions

`Location(string path, AjaxContext context)` became `Location(string path, HtmxLocationOptions options)`.

| `AjaxContext` (1.x)  | `HtmxLocationOptions` (2.0)                                                                   |
|----------------------|-----------------------------------------------------------------------------------------------|
| `Source`             | `Source`                                                                                      |
| `Target`             | `Target`                                                                                      |
| `Swap`               | `Swap` for a swap style; `SwapExpression` for a style with modifiers                          |
| `Select`             | `Select`                                                                                      |
| `Values` (`object`)  | `Values` (`IDictionary<string, HtmxFieldValues>`)                                             |
| `Headers` (`object`) | `Headers` (`IDictionary<string, string>`)                                                     |
| `Event`, `Handler`   | Removed: HTMX expects a live browser event and a JavaScript function, which JSON cannot carry |
| —                    | New: `SelectOob`, `HistoryUrl`, `HistoryAction`                                               |

1.x:

```csharp
Response.Htmx(htmx => htmx.Location("/orders", new AjaxContext
{
    Target = "#main",
    Values = new { status = "open" }
}));
```

2.0:

```csharp
Response.Htmx(htmx => htmx.Location("/orders", new HtmxLocationOptions
{
    Target = "#main",
    Values = new Dictionary<string, HtmxFieldValues>
    {
        ["status"] = "open"
    }
}));
```

`HtmxFieldValues` converts implicitly from `string` and `string[]`.
See [Location or redirect](responses.md#location-or-redirect).

### Update client events

**Event names are sent as written.**

> [!WARNING]
> In 1.x, event names were serialized with a camel-case policy, so `TriggerEvent("ItemSaved")` sent `itemSaved`.
> In 2.0 the browser receives `ItemSaved`. A listener for `itemSaved` or `hx-trigger="itemSaved from:body"`
> silently stops firing. Use the same name on the server and in the client.

**Dictionary keys and `null` values in details are kept.** In 1.x, the detail object was serialized with
camel-cased dictionary keys and without `null` properties, so a detail with `Errors["Email"]` and `Note = null`
sent `{"errors":{"email":[...]}}`. In 2.0 it is serialized the way ASP.NET Core serializes JSON responses:
property names are still camel-cased, but the browser receives `{"errors":{"Email":[...]},"note":null}`.
Update listeners that read camel-cased dictionary keys.

**Repeated names are kept.** In 1.x, a second event with the same name in the same header was dropped.
In 2.0, all events are sent in call order. HTMX cannot deliver a repeated name from one header by itself,
so the [Toolkit script](antiforgery.md#what-the-toolkit-script-does) delivers such events;
without the script, none of the events in that header fire.

Other changes:

- `TriggerEvents(IReadOnlyDictionary<string, object>, HtmxTriggerTiming)` was removed. Call `TriggerEvent` once per event.
- The timing parameter of `TriggerEvent(string, HtmxTriggerTiming)` was renamed from `trigger` to `timing`.
- An event without a detail now receives an empty object; in 1.x, `event.detail.value` was an empty string.
- `TriggerEvent` throws `ArgumentException` for an empty or whitespace event name.
- `TriggerEvent(string, object, HtmxTriggerTiming)` is annotated as incompatible with trimming and Native AOT.
  Trimmed and Native AOT projects get warnings IL2026 and IL3050 for it; use the overload that accepts
  `JsonTypeInfo<T>`. See [Serialize details with source-generated metadata](responses.md#serialize-details-with-source-generated-metadata).
- `HtmxTriggerTiming` members are ordered as HTMX processes them: `Receive`, `AfterSwap`, `AfterSettle`.
  The numeric values of `AfterSwap` and `AfterSettle` swapped; this matters only if the values are stored or cast.

### Update typed response headers

`HtmxResponseHeaders`, returned by `Response.GetHtmxHeaders()`, changed from a class to a `readonly struct`:

- `Trigger`, `TriggerAfterSwap`, and `TriggerAfterSettle` are read-only and return
  `IReadOnlyList<KeyValuePair<string, string>>?` with event names and detail JSON. Add events with `TriggerEvent`.
- Assigning `null` or an empty string removes the header. In 1.x, `null` was ignored and an empty string
  produced an empty header. `Reswap` also accepts `null`.

### Update `[HtmxResponse]`

`HtmxResponseAttribute` now applies only to classes and methods, at most once per target, and is inherited:

- Merge several `[HtmxResponse]` attributes on one target into one. Duplicates and other targets are now build errors.
- An attribute on a base controller or page model now applies to derived classes. In 1.x, it was ignored there.
  An attribute on a derived class replaces the inherited one.
- `StopPolling` was removed; see [Replace StopPolling](#replace-stoppolling).

## Other changes

These changes affect fewer applications:

- `HtmxSwap` gained `InnerMorph`, `OuterMorph`, `OuterSync`, and `TextContent` after `OuterHtml`.
  The numeric values of the members that follow them changed; this matters only if the values are stored or cast.
- Typed swap getters, such as `HtmxResponseHeaders.Reswap`, parse the style the way HTMX does:
  case-sensitively, and ignoring leading whitespace. A value such as `OUTERHTML` now returns `null`.
- `HtmxHeaderTagHelper.Headers` changed from `Dictionary<string, string>` to `IDictionary<string, string>`,
  and header names are compared without case.
- `HtmxScrollBehavior` gained `Instant`.
- `Location`, `Redirect`, `Reswap(string)`, `Retarget`, and `Reselect` throw `ArgumentException` for a `null`
  or empty value, and `Location(string, HtmxLocationOptions)` also throws for `null` options.
  In 1.x, an empty value produced an empty header.
- Generated JSON in attributes keeps non-ASCII characters, such as Cyrillic, instead of escaping them as `\uXXXX`.
  Update snapshot tests that compare it as text.

## New in 2.0

After the migration, consider these additions:

- HTMX 2.x and 4.x support, with configuration types for each version. See [Choose an HTMX version](choosing-version.md).
- `hx-val-*` and `hx-request-*` Tag Helpers for `hx-vals` and per-request options. See [Tag Helpers](tag-helpers.md).
- `HX-Request-Type` support: `IsHtmxPartialRequest()`, `IsHtmxFullRequest()`, and `[HtmxRequest(RequestType = ...)]`.
  See [Check full or partial requests](requests.md#check-full-or-partial-requests).
- Morph swaps for HTMX 1.9.x and 2.x through the `ramstack-swap` extension. See [Morph swaps](version-compatibility.md#morph-swaps).
- Native AOT support for event details through `JsonTypeInfo<T>`.
