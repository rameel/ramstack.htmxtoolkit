# Troubleshooting

Start with the browser Network panel. Inspect the generated element, request headers, response body,
response headers, and console in that order.

## Clicking does not send a request

If the Network panel shows no request, check the browser setup before debugging the handler:

1. Confirm that the HTMX script returns JavaScript with HTTP 200. A NuGet reference does not install HTMX.
2. Look for console errors, including `htmx is not defined` when the Toolkit script loads too early.
3. Inspect the rendered element. It should have a real `hx-get` or `hx-post` URL, not only `hx-page` or `hx-action`.
4. Check whether HTML form validation is blocking submission. An invalid required input can prevent a request.
5. Check that the element's trigger matches the interaction, especially if you changed `hx-trigger`.

Use the explicit script setup in [Getting started](getting-started.md#4-load-the-browser-scripts).

## Tag Helper attributes appear unchanged

**Symptom:** The rendered HTML still contains `hx-page`, `hx-action`, `hx-route-*`, or `hx-request-timeout`.

**Check:** Ensure `_ViewImports.cshtml` applies to the view and contains:

```razor
@addTagHelper *, Ramstack.HtmxToolkit
```

Also verify the project references `Ramstack.HtmxToolkit` and uses only one routing mode per element.

## A full page is inserted into a target

**Cause:** The endpoint returned a page or view with its layout rather than a partial.

Inspect the response body for `<!DOCTYPE html>`, `<head>`, or site navigation, then check the request:

- With HTMX 1.9.x or 2.x, look for the `HX-Request-Type: partial` header. If it is missing, the page does not load
  the [Toolkit script](antiforgery.md#what-the-toolkit-script-does), so `IsHtmxPartialRequest()` returns `false`
  and the endpoint returns the full page. Load the script after HTMX on every page that uses HTMX.
- If the header is present, make the endpoint return `PartialView(...)` or `Partial(...)` for the HTMX path.

See [Full pages and fragments](pages-and-fragments.md).

## A login page appears inside a component

Inspect the request's redirect chain and final response URL. An expired authentication cookie can cause a
protected endpoint to redirect to the login page. The browser follows that redirect, and HTMX receives
the login page as the response to the original component request.

Handle this in the authentication challenge or cookie handler that creates the redirect. For the HTMX path,
return a non-redirect response, such as HTTP 200 with `HX-Redirect` pointing to the local login page.
Keep the normal redirect for ordinary browser requests. Adding an HTMX header to the intermediate 302
will not work because HTMX cannot read that response.

If the endpoint returns 401 or 403 instead, decide whether the browser should navigate to sign-in or show
an access message. A 403 can mean that a signed-in user lacks permission; signing in again may not help.
See [Redirect after authentication](recipes.md#redirect-after-authentication) for the response pattern.

## A request fails, but the page shows no error

Check the Network panel for an HTTP error, a timeout, or a failed connection. HTMX 1.9.x and 2.x leave most
4xx and 5xx responses unswapped by default. An error may therefore appear in the console while the page stays unchanged.
A connection failure has no response HTML to insert at all.

Show a separate message through lifecycle events and keep it outside the target being replaced.
The [loading and errors tutorial](loading-and-errors.md) has a complete HTMX 2.x example with retry behavior.
Its [HTMX 4 notes](loading-and-errors.md#adapting-to-htmx-4) cover the different event names and error-swap defaults.
Use the server log to diagnose the failure; display a short message to the user instead of an exception page.

## POST returns HTTP 400

Common causes are missing or invalid antiforgery data.

1. Inspect the document for `<meta name="htmx-config">` and its `data-antiforgery-*` attributes.
2. Confirm HTMX loads before the Toolkit script.
3. Confirm the Toolkit script URL returns JavaScript rather than 404 or HTML.
4. Inspect the request for the antiforgery header or form value configured by ASP.NET Core.
5. Check the server log for the actual failure. HTTP 400 can also come from model binding or application code.

See [Antiforgery and Toolkit script](antiforgery.md).

## POST works until navigation or sign-in

After boosted navigation, the Toolkit script takes the token from the new page's `<htmx-config />`;
a fragment without token metadata keeps the previous token. Compare a POST before and after navigation
in the Network panel.

If the antiforgery header disappears, check for a hidden antiforgery input first: the script does not add
the token when the form already sends that field. If neither a header nor a form token is sent, the browser
may be running an outdated copy of the script. Keep `asp-append-version="true"` on the script tag,
disable the cache in the Network panel, reload, and check that the script comes from the deployed package version.
See [Static assets and deployment](static-assets.md).

If the token is sent but validation fails after a sign-in or sign-out, the token belongs to the previous identity.
A full navigation through `HX-Redirect` reloads the document and its metadata; see
[Token refresh after boosted navigation](antiforgery.md#token-refresh-after-boosted-navigation).

## Validation HTML is returned but no errors appear

Inspect the response status as well as its body. HTMX 1.9.x and 2.x do not swap most 4xx or 5xx responses by default.
`Retarget` and `Reswap` set the target and swap style; they do not by themselves enable swapping a 422 response.

The [forms tutorial](forms-and-validation.md) returns validation HTML with status 200. To use 422 with HTMX 2.x,
add a matching [ResponseHandling rule](configuration-v2.md#handle-validation-responses).
For HTMX 4.x, check whether your `NoSwap` patterns exclude the response status.

If the response is 200, verify that the target exists and that the returned partial actually contains validation
messages. Check input names against bound properties, such as `Input.DisplayName`, and preserve `ModelState`
when returning an invalid form.

## An update works once and then stops

Inspect the DOM after the first swap. With `outerHTML`, the returned fragment replaces the target element itself.
Return its ID and any request attributes needed for the next interaction. An empty response with an outer swap
can remove the target entirely.

With `innerHTML`, return only the target's contents. Returning another copy of the same container can introduce
duplicate IDs. For repeated list requests, update the next cursor too; see [Load more items](recipes.md#load-more-items).

## New content appears, but its JavaScript widget does not work

A script that only runs on `DOMContentLoaded` initializes the first document. That event does not run again
when HTMX inserts a partial, and listeners attached directly to replaced elements are removed with those elements.

For simple click or input behavior, register a listener once on a stable ancestor such as `document` and
use `event.target.closest(...)` to find the relevant control. The [event-data example](responses.md#read-event-data-in-javascript)
shows a listener that survives swaps.

For a widget that needs explicit initialization, initialize the newly inserted elements after each update.
HTMX 2.x provides `htmx.onLoad(...)` for initial and newly loaded content; HTMX 4 provides lifecycle hooks
such as `htmx:after:swap`. Initialize the first document too when using a swap-only hook.
Use the widget library's own instance check or a marker to avoid initializing the same element twice,
and call its cleanup API before removing elements when the widget requires it.

Keep shared scripts in the layout and scope initialization to the new content. Repeating script tags in
every partial can register duplicate handlers. Check the [HTMX 2 events](https://htmx.org/events/) or
[HTMX 4 events](https://four.htmx.org/reference/events) for the client version you load.

## Back or Forward shows only a fragment

Inspect the request sent during history restoration. Return a full document when `HistoryRestoreRequest` is true;
`IsHtmxPartialRequest()` already returns `false` for such requests. A URL pushed into history should be able to render
a full page when opened directly.

If behavior depends on which request happened first, inspect HTTP and output cache keys. A shared URL must not
reuse a fragment as the full-page response. See [Full pages and fragments](pages-and-fragments.md).

## Toolkit script returns 404

Enable static files and reference the script from the layout:

```csharp
app.UseStaticFiles();
app.MapRazorPages();
```

For ASP.NET Core 9 or later with `MapStaticAssets()`, configure the page endpoints with the asset collection:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
```

```razor
<script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
        asp-append-version="true"></script>
```

The resolved URL points to `/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js` or its fingerprinted variant
under the application's path base. Ensure `_ViewImports.cshtml` registers the standard MVC Tag Helpers
(`@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`) and that deployment includes the published
static assets and manifests.

When running from build output in an environment other than `Development`, enable discovery of static web assets
with `builder.WebHost.UseStaticWebAssets()` before building the app. This extra call is unnecessary when running
from published output.

See [Static assets and deployment](static-assets.md) for the complete setup and path-base handling.

## Response has no HX headers

`Response.Htmx(...)` intentionally invokes its callback only when `Request.IsHtmxRequest()` is `true`:
the incoming request contains the HTMX request marker and does not restore history.
Confirm the request is actually sent by HTMX and is not a history restoration request.

For infrastructure that must write a header unconditionally, use `Response.GetHtmxHeaders()`
and make the condition explicit.

## Setting an HX header throws InvalidOperationException

Response headers, including the events added by `TriggerEvent()`, can be set only before the response starts.
The response starts when the first part of the body is sent, for example when a view calls `FlushAsync()`.
By the time `OnResultExecuted` of a filter runs, the result has usually written the body. Set the headers
before that point, for example in `OnResultExecuting`.

## HX-Redirect does not navigate the browser

Check the response status. HTMX cannot process `HX-Redirect` or other `HX-*` headers from an intermediate 3xx response
because the browser follows that redirect internally. Return status 200 with `HX-Redirect` for the HTMX request,
and use an ordinary ASP.NET Core redirect only for the non-HTMX path.

## The wrong per-request attribute is generated

HTMX 1.9.x and 2.x receive `hx-request`; HTMX 4.x receives `hx-config`. Check `UseHtmxV1`, `UseHtmxV2`,
or `UseHtmxV4` in service registration and verify that the browser loads the same major version.

If no version is selected, HtmxToolkit uses V2.

## Request metadata is null

Some properties are version-specific:

- `Trigger` and `TriggerName` are for HTMX 1.9.x and 2.x.
- `Source` is for HTMX 4.x.
- `Prompt` needs the `hx-prompt` extension in HTMX 4.x.
- `Target` uses an ID in V1/V2 and `tag#id` in V4.

`RequestType` returns `HtmxRequestType.Unspecified` when its header is absent, except for history restoration
requests, which it reports as `Full`. HTMX 1.9.x and 2.x send this header only when the page loads the Toolkit script:
if `RequestType` is always `Unspecified` and `IsHtmxPartialRequest()` is always `false`, check that the script is loaded.

See [Version compatibility](version-compatibility.md).

## MVC reports no matching endpoint

`[HtmxRequest]` is an MVC action constraint. A normal browser request cannot select an action
that has only that constrained route. Provide a normal action for the route or return a page or fragment from one action.

Also check `BoostedFilter`: `HtmxBoostedFilter.Boosted` accepts only boosted HTMX requests,
`HtmxBoostedFilter.NonBoosted` only non-boosted HTMX requests, and `HtmxBoostedFilter.Any` (the default)
does not restrict boosted navigation. A `RequestType` of `Full` or `Partial` additionally requires a matching
`HX-Request-Type` header, which HTMX 1.9.x and 2.x send only when the page loads the Toolkit script.
Leave it `Unspecified` to accept requests without that header.

## URL generation throws an exception

Choose exactly one routing mode:

- named route: `hx-route`;
- MVC: `hx-controller` and/or `hx-action`;
- Razor Pages: `hx-page` and/or `hx-page-handler`.

Choose at most one method from `hx-get`, `hx-post`, `hx-put`, `hx-patch`, and `hx-delete`.
If no method is present, the URL Tag Helper generates `hx-get`.

## Event timing differs after moving to HTMX 4

HTMX 1.9.x and 2.x have separate trigger response headers for receive, after swap, and after settle.
HTMX 4.x receives all Toolkit events through `HX-Trigger` when the request completes, after the swap and settle steps.
During migration, review:

- `HtmxTriggerTiming.Receive` events whose handlers must run before the swap.
  Move that logic to an `htmx:before:swap` listener.
- `AfterSwap` and `AfterSettle` events that must not fire when the response is not swapped,
  for example with status code 204.

See [Response events](version-compatibility.md#response-events).

## A response event does not refresh another component

First check the actual response in the Network panel for `HX-Trigger`, `HX-Trigger-After-Swap`, or
`HX-Trigger-After-Settle`. `TriggerEvent()` sets the header in `Response.Headers` right away, so server code
can also inspect it after the call.

If the header is present:

- Match the event name exactly in `hx-trigger`.
- Use `from:body` when another element sends the event.
- Keep the listening component outside the content being replaced.
- Give the listener a request URL as well as a trigger.
- Keep the Toolkit script loaded when an event name repeats in one header; otherwise none of that header's events fire.

For a complete working pair of handlers and markup, see [Forms, validation, and events](forms-and-validation.md).

## Morphing falls back to HTML replacement

For HTMX 1.9.x or 2.x, activate `ramstack-swap` with `hx-ext`. Load Idiomorph for true `InnerMorph`
or `OuterMorph` behavior. Without it, the Toolkit script logs a warning and uses HTML replacement.
HTMX 4.x supports the morph swap styles natively.

## Configuration does not change

Ensure `<htmx-config />` is present in the full document and inspect its `content` attribute.
Only non-null properties are serialized. Confirm the code configures the same version it later
requests through `GetHtmxConfig<TConfig>()`.

Configuration JSON is cached and regenerated when a configuration property is assigned.
Configure application options at startup. Do not change these options while handling requests.

## Cross-origin requests fail

Client settings such as `SelfRequestsOnly`, `Mode`, `WithCredentials`, and per-request credentials
do not configure server CORS. Check the browser CORS error, ASP.NET Core CORS policy,
allowed origins, cookies, and credential rules together.
