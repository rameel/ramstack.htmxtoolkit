# Read HTMX requests

Use `HttpRequest` extensions to distinguish HTMX requests from regular navigation and to read HTMX request metadata
without comparing header strings.

## Detect a request

```csharp
public IActionResult Details(int id)
{
    var product = catalog.Get(id);

    if (Request.IsHtmxPartialRequest())
        return PartialView("_ProductDetails", product);

    return View(product);
}
```

`IsHtmxPartialRequest()` is true only when HTMX swaps the response into an element inside the page.
Everything else gets the page: direct visits, boosted navigation that replaces the `<body>`, `hx-select` requests,
and history restoration. See [Check full or partial requests](#check-full-or-partial-requests).

`IsHtmxRequest()` tells whether HTMX sent the request at all.
The overload with `out HtmxRequestHeaders` combines detection and strongly typed header access:

```csharp
if (Request.IsHtmxRequest(out var htmx))
{
    logger.LogDebug(
        "HTMX request for {Target} from {Source}",
        htmx.Target,
        htmx.Source ?? htmx.Trigger);
}
```

Call `GetHtmxHeaders()` when request detection and header access happen in separate code:

```csharp
var htmx = Request.GetHtmxHeaders();
```

## Request header properties

| Property | Meaning |
|---|---|
| `Request` | Whether the `HX-Request` header is present |
| `Boosted` | Whether a regular navigation was enhanced by `hx-boost` |
| `CurrentUrl` | Current browser URL when the request was sent |
| `HistoryRestoreRequest` | Whether HTMX marked the request for history restoration |
| `Target` | Target element; its format differs in HTMX 4 |
| `Prompt` | Result of `hx-prompt`; in HTMX 4.x, sent only by the `hx-prompt` extension |
| `Trigger`, `TriggerName` | Triggering element ID and name in HTMX 1.x and 2.x |
| `Source` | Triggering `tag#id` in HTMX 4.x |
| `RequestType` | `HtmxRequestType.Partial` or `HtmxRequestType.Full`; added by the Toolkit script in HTMX 1.x and 2.x |

Missing or version-inapplicable string headers return `null`.
`Request` is `true` whenever the `HX-Request` header is present, whatever its value.
`Boosted` and `HistoryRestoreRequest` are `true` only when their header is exactly `true`.
In HTMX 4.x, the `hx-prompt` extension sends the prompt answer URI-encoded;
decode it with `Uri.UnescapeDataString` before use.
`RequestType` returns `HtmxRequestType.Full` for a history restoration request without its header,
and `HtmxRequestType.Unspecified` for any other request without it.

See [Version compatibility](version-compatibility.md) before using version-specific metadata in shared code.

## Handle history restoration

When `HistoryRestoreRequest` is true, return the full document rather than a target fragment.
`RequestType` already accounts for it: HTMX 4.x sends `HX-Request-Type: full` with such a request,
and HTMX 1.x and 2.x send no `HX-Request-Type`, which `RequestType` reports as `HtmxRequestType.Full`.
So `IsHtmxPartialRequest()` returns `false`, and the example in [Detect a request](#detect-a-request) returns the page.

HTMX 1.x and 2.x send this request after a miss in their local snapshot cache.
HTMX 4.x requests history entries from the server by default;
see [History behavior](configuration-v4.md#history-behavior).

## Detect boosted navigation

`IsHtmxBoosted()` identifies requests originating from boosted links or forms:

```csharp
public IActionResult Navigation()
{
    if (Request.IsHtmxBoosted())
        return PartialView("_Navigation");

    return RedirectToAction("Index");
}
```

When both the classification and metadata are needed, use `IsHtmxRequest(out var htmx)` and inspect `htmx.Boosted`.
This example uses a dedicated navigation fragment. For a full navigable page, use `IsHtmxPartialRequest()`
as in [Full pages and fragments](pages-and-fragments.md).

## Check full or partial requests

The `HX-Request-Type` header tells the server what the client does with the response:

- `partial` when HTMX swaps it into an element inside the page;
- `full` when it replaces the whole `<body>`, or when `hx-select` takes part of it.

HTMX 4.x sends this header itself. HTMX 1.x and 2.x do not, so the Toolkit script adds it,
computed the same way from the final target and selector, including the `select` option of `htmx.ajax()`
and `HX-Location`. The script respects `noHeaders` in `hx-request`; a `js:` value of `hx-request`
is not evaluated, so it cannot turn the header off.

Use `Request.IsHtmxPartialRequest()` to check for a fragment, or `Request.IsHtmxFullRequest()` for a full page.
A history restoration request counts as a full page even though HTMX 1.x and 2.x send it without the header.
For any other request without the header, both methods return `false`; with HTMX 1.x and 2.x,
this means the page does not load the [Toolkit script](antiforgery.md#what-the-toolkit-script-does).

The type comes from the target, not from `HX-Boosted`: a boosted link with `hx-target` is a partial request.
`RequestType` describes the requested HTML; HtmxToolkit leaves the actual view selection to the endpoint.
See also the [HTMX 4 request headers](https://four.htmx.org/docs/#request-headers).

## Select MVC actions declaratively

`[HtmxRequest]` is an MVC action constraint: an action marked with it is selected only for HTMX requests.
When it shares a URL with an unconstrained action, MVC prefers the constrained action for the requests it accepts.
This lets one URL serve a page and its fragment from separate actions:

```csharp
[HttpGet("/profile")]
public IActionResult Profile() =>
    View();

[HttpGet("/profile")]
[HtmxRequest(RequestType = HtmxRequestType.Partial)]
public IActionResult ProfileFragment() =>
    PartialView("_Profile");
```

HTMX requests for a fragment go to `ProfileFragment`. Everything else, including direct visits,
boosted navigation that replaces the `<body>`, and history restoration, goes to `Profile`.

`RequestType = HtmxRequestType.Unspecified`, the default, does not restrict the request type.
`Full` and `Partial` require a matching `RequestType`. History restoration requests match `Full`.
Other requests without the `HX-Request-Type` header, such as HTMX 1.x and 2.x requests from a page
without the Toolkit script, match neither.

Without `RequestType`, the attribute accepts any HTMX request. That suits an endpoint that only HTMX calls,
such as a status widget:

```csharp
[HttpGet("/inventory/{sku}")]
[HtmxRequest]
public IActionResult InventoryFragment(string sku) =>
    PartialView("_Inventory", inventory.Find(sku));
```

A direct browser visit finds no matching action and gets 404 instead of a bare fragment.
Don't use the plain attribute to split a page from its fragment: boosted navigation and `hx-select` requests
come from HTMX too, but expect a full page. Set `RequestType` as in the first example.

Set `BoostedFilter` to accept only boosted or non-boosted requests:

```csharp
[HtmxRequest(BoostedFilter = HtmxBoostedFilter.Boosted)]
public IActionResult BoostedNavigation() =>
    PartialView("_Navigation");

[HtmxRequest(BoostedFilter = HtmxBoostedFilter.NonBoosted)]
public IActionResult ComponentRequest() =>
    PartialView("_Component");
```

`BoostedFilter = HtmxBoostedFilter.Any`, the default, does not restrict boosted navigation.

You can combine the two filters. For example, this action accepts only non-boosted HTMX requests for a full page:

```csharp
[HtmxRequest(RequestType = HtmxRequestType.Full, BoostedFilter = HtmxBoostedFilter.NonBoosted)]
public IActionResult FullPageRequest() =>
    View("Index");
```

Both conditions must match. These properties only select an action; the action still chooses whether to
return a view or a partial view.

If no action satisfies all routing and action constraints, MVC reports that no endpoint matched;
the attribute does not automatically fall back to a different URL.

## Use raw header names only when required

`HtmxRequestHeaderNames` contains constants for code that needs a header name, such as a CORS policy
or an [output cache policy](pages-and-fragments.md#cache-full-pages-and-fragments-separately).
Application endpoints should normally prefer `HtmxRequestHeaders` so version differences remain visible
in the type's documentation.

Next, see [Full pages and fragments](pages-and-fragments.md) or [Control HTMX responses](responses.md).
