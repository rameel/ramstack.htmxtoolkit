# HTMX requests

Use `HttpRequest` extensions to distinguish HTMX requests from regular navigation and to read HTMX request metadata
without comparing header strings.

## Detect a request

```csharp
public IActionResult Details(int id)
{
    var product = catalog.Get(id);

    if (Request.IsHtmxRequest())
        return PartialView("_ProductDetails", product);

    return View(product);
}
```

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
| `Request` | Whether `HX-Request` is `true` |
| `Boosted` | Whether a regular navigation was enhanced by `hx-boost` |
| `CurrentUrl` | Current browser URL when the request was sent |
| `HistoryRestoreRequest` | Whether HTMX marked the request for history restoration |
| `Target` | Target element; its format differs in HTMX 4 |
| `Prompt` | Result of `hx-prompt` in HTMX 1.x and 2.x |
| `Trigger`, `TriggerName` | Triggering element ID and name in HTMX 1.x and 2.x |
| `Source` | Triggering `tag#id` in HTMX 4.x |
| `RequestType` | `partial` or `full` in HTMX 4.x |

Missing or version-inapplicable string headers return `null`.
Boolean properties return `false` when the header is absent or is not exactly `true`.

See [Version compatibility](version-compatibility.md) before using version-specific metadata in shared code.

## Handle history restoration

When `HX-History-Restore-Request` is present, return the full document rather than a target fragment:

```csharp
if (Request.IsHtmxRequest(out var htmx) && !htmx.HistoryRestoreRequest)
    return PartialView("_ProductDetails", product);

return View(product);
```

This prevents a fragment from being treated as the document during history recovery.
HTMX 1.x and 2.x use local snapshots and can send this request after a cache miss.
HTMX 4.x normally requests history URLs from the server and does not keep local DOM snapshots
unless its optional history-cache extension is enabled.

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

An overload also returns `HtmxRequestHeaders` when both the classification and metadata are needed.

## Select MVC actions declaratively

Apply `[HtmxRequest]` to reserve a controller action for HTMX requests:

```csharp
[HttpGet("/inventory/{sku}")]
[HtmxRequest]
public IActionResult InventoryFragment(string sku) =>
    PartialView("_Inventory", inventory.Find(sku));
```

Set `Boosted` to accept only boosted or non-boosted requests:

```csharp
[HtmxRequest(Boosted = true)]
public IActionResult BoostedNavigation() =>
    PartialView("_Navigation");

[HtmxRequest(Boosted = false)]
public IActionResult ComponentRequest() =>
    PartialView("_Component");
```

`Boosted = null`, the default, accepts either kind of HTMX request. If no action satisfies all routing
and action constraints, MVC reports that no endpoint matched; the attribute does not automatically fall back
to a different URL.

## Use raw header names only when required

`HtmxRequestHeaderNames` contains constants for integration with middleware or APIs that require a header name.
Application endpoints should normally prefer `HtmxRequestHeaders` so version differences remain visible
in the type's documentation.

Next, see [Full pages and fragments](pages-and-fragments.md) or [Control HTMX responses](responses.md).
