# Troubleshooting

Start with the browser Network panel. Inspect the generated element, request headers, response body,
response headers, and console in that order.

## Tag Helper attributes appear unchanged

**Symptom:** The rendered HTML still contains `hx-page`, `hx-action`, `hx-route-*`, or `hx-request-timeout`.

**Check:** Ensure `_ViewImports.cshtml` applies to the view and contains:

```html
@addTagHelper *, Ramstack.HtmxToolkit
```

Also verify the project references `Ramstack.HtmxToolkit` and that mutually exclusive route modes were not mixed.

## A full page is inserted into a target

**Cause:** The endpoint returned a page or view with its layout rather than a partial.

Inspect the response body for `<!DOCTYPE html>`, `<head>`, or site navigation. Return `PartialView(...)`
or `Partial(...)` for the HTMX path. See [Full pages and fragments](pages-and-fragments.md).

## POST returns HTTP 400

Common causes are missing or invalid antiforgery data.

1. Inspect the document for `<meta name="htmx-config">` and its `data-antiforgery-*` attributes.
2. Confirm HTMX loads before the Toolkit script.
3. Confirm the Toolkit script endpoint returns JavaScript rather than 404 or HTML.
4. Inspect the request for the antiforgery header or form value configured by ASP.NET Core.
5. After boosted navigation, confirm the returned full document also includes `<htmx-config />`.

See [Antiforgery and Toolkit script](antiforgery.md).

## Toolkit script returns 404

Map the endpoint before the application finishes endpoint registration:

```csharp
app.MapHtmxToolkitScript();
app.MapRazorPages();
```

Use `@Html.HtmxToolkitScriptPath()` instead of copying the default hash URL. If a custom path is passed
to `MapHtmxToolkitScript`, ensure the helper is rendered after that mapping is configured during application startup.

## Response has no HX headers

`Response.Htmx(...)` intentionally invokes its callback only when the incoming request contains the HTMX request marker.
Confirm the request is actually sent by HTMX.

For infrastructure that must write a header unconditionally, use `Response.GetHtmxHeaders()`
and make the condition explicit.

## HX-Redirect does not navigate the browser

Check the response status. HTMX cannot process `HX-Redirect` or other `HX-*` headers from an intermediate 3xx response
because the browser follows that redirect internally. Return status 200 with `HX-Redirect` for the HTMX request,
and use an ordinary ASP.NET Core redirect only for the non-HTMX path.

## The wrong per-request attribute is generated

HTMX 1.x and 2.x receive `hx-request`; HTMX 4.x receives `hx-config`. Check `UseHtmxV1`, `UseHtmxV2`,
or `UseHtmxV4` in service registration and verify that the browser loads the same major version.

If no version is selected, HtmxToolkit uses V2.

## Request metadata is null

Some properties are version-specific:

- `Trigger`, `TriggerName`, and `Prompt` are for HTMX 1.x and 2.x.
- `Source` and `RequestType` are for HTMX 4.x.
- `Target` uses an ID in V1/V2 and `tag#id` in V4.

See [Version compatibility](version-compatibility.md).

## MVC reports no matching endpoint

`[HtmxRequest]` is an MVC action constraint. A normal browser request cannot select an action
that has only that constrained route. Provide a normal action for the route or handle both representations in one action.

Also check `Boosted`: `true` accepts only boosted requests, `false` only non-boosted HTMX requests, and `null` either kind.

## URL generation throws an exception

Choose exactly one routing mode:

- named route: `hx-route`;
- MVC: `hx-controller` and/or `hx-action`;
- Razor Pages: `hx-page` and/or `hx-page-handler`.

Choose at most one method from `hx-get`, `hx-post`, `hx-put`, `hx-patch`, and `hx-delete`.
If no method is present, the URL Tag Helper generates `hx-get`.

## Event timing differs after moving to HTMX 4

HTMX 1.x and 2.x have separate trigger response headers for receive, after swap, and after settle.
HTMX 4.x receives Toolkit events through `HX-Trigger` at request completion.
Review uses of `HtmxTriggerTiming.AfterSettle` during migration.

## Morphing falls back to HTML replacement

For HTMX 1.9.x or 2.x, activate `ramstack-morph` with `hx-ext`. Load Idiomorph for true `InnerMorph`
or `OuterMorph` behavior. Without it, the Toolkit script logs a warning and uses HTML replacement.
HTMX 4.x supports the morph swap styles natively.

## Configuration does not change

Ensure `<htmx-config />` is present in the full document and inspect its `content` attribute.
Only non-null properties are serialized. Confirm the code configures the same version it later
requests through `GetHtmxConfig<TConfig>()`.

Configuration JSON is cached and regenerated when a configuration property is assigned.
Treat application options as startup configuration rather than per-request mutable state.

## Cross-origin requests fail

Client settings such as `SelfRequestsOnly`, `Mode`, `WithCredentials`, and per-request credentials
do not configure server CORS. Check the browser CORS error, ASP.NET Core CORS policy,
allowed origins, cookies, and credential rules together.
