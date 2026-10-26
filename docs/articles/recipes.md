# Common recipes

These recipes combine ordinary HTMX behavior with HtmxToolkit's routing, request, response, and security APIs.
For a first application, complete [Getting started](getting-started.md) and [Forms, validation, and events](forms-and-validation.md)
first. The recipes below show variations; where a data service is application-specific, it is called out explicitly.

## Return validation errors to another target

Start with the `ProfileInput`, `ProfileStore`, and bound `Input` property from the
[forms tutorial](forms-and-validation.md). To keep the form in place and show errors separately,
use this markup in `Pages/Profile.cshtml` instead of `_ProfileForm`:

```razor
<form method="post" hx-post asp-page="/Profile" asp-page-handler="Save"
      hx-target="#save-result" novalidate>
    <div id="validation-errors" role="alert">
        <div asp-validation-summary="All"></div>
    </div>
    <label asp-for="Input.DisplayName">Display name</label>
    <input asp-for="Input.DisplayName" />
    <button type="submit">Save</button>
</form>
<div id="save-result" aria-live="polite"></div>
```

Replace the `OnPostSave` handler with:

```csharp
public IActionResult OnPostSave()
{
    if (!ModelState.IsValid)
    {
        Response.Htmx(htmx => htmx
            .Retarget("#validation-errors")
            .Reswap(HtmxSwap.InnerHtml));

        return Request.IsHtmxRequest()
            ? Partial("_ValidationSummary", ModelState)
            : Page();
    }

    _profiles.DisplayName = Input.DisplayName;
    Response.Htmx(htmx => htmx.TriggerEvent("profile-saved", HtmxTriggerTiming.AfterSwap));

    return Request.IsHtmxRequest()
        ? Partial("_ProfileResult", _profiles.DisplayName)
        : RedirectToPage();
}
```

Create `Pages/_ValidationSummary.cshtml`:

```razor
@model Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary

<ul>
    @foreach (var entry in Model.Values)
    {
        foreach (var error in entry.Errors)
        {
            <li>@error.ErrorMessage</li>
        }
    }
</ul>
```

Create `Pages/_ProfileResult.cshtml`:

```razor
@model string

<p>Profile saved for @Model.</p>
<div id="validation-errors" hx-swap-oob="innerHTML"></div>
```

The out-of-band element clears old errors after a successful save. Both results use HTTP 200 so default
HTMX 2.x response handling swaps them. If you use 422, configure [validation response handling](configuration-v2.md#handle-validation-responses).
The full page's `asp-validation-summary` also displays errors after a normal browser POST.

## Refresh another component after saving

The [forms tutorial](forms-and-validation.md) includes a complete example: a save handler sends `profile-saved`,
and an independent summary requests its latest HTML. Listen on `body` when the source is another component:

```razor
<aside hx-get
       asp-page="/Profile"
       asp-page-handler="Summary"
       hx-trigger="profile-saved from:body">
</aside>
```

If a JavaScript listener also needs details about the change, replace the tutorial's event call with:

```csharp
Response.Htmx(htmx => htmx.TriggerEvent(
    "profile-saved",
    new { DisplayName = Input.DisplayName },
    HtmxTriggerTiming.AfterSwap));
```

Add this to a JavaScript file loaded once by the layout, after HTMX:

```javascript
document.addEventListener("profile-saved", event => {
    console.log("Saved display name:", event.detail.displayName);
});
```

The detail property becomes `displayName` in JSON. Listening on `document` survives replacement of the form.
The HTMX listener above only needs the event name;
it reloads the saved data from the server. Keep it outside the content replaced by the save response.
See [Responses](responses.md#trigger-client-events) for event timings and source-generated serialization.

## Poll a background operation

For polling that works with every supported HTMX version, return the polling element itself and replace it with
`outerHTML`. Each replacement element schedules the next request with `load delay:1s` while work is incomplete.
This example simulates progress by adding ten on each request; it does not run a background job.

Add this type to your models namespace:

```csharp
public sealed record ProgressState(int Percent)
{
    public bool Completed => Percent >= 100;
}
```

Create `Pages/Jobs/_Progress.cshtml` (and import the model's namespace in `_ViewImports.cshtml`):

```razor
@model ProgressState

@if (Model.Completed)
{
    <div id="job-progress">Complete</div>
}
else
{
    <div id="job-progress"
         hx-get
         asp-page="/Jobs/Status"
         asp-page-handler="Progress"
         asp-route-progress="@Model.Percent"
         hx-trigger="load delay:1s"
         hx-target="this"
         hx-swap="outerHTML">
        @Model.Percent%
    </div>
}
```

```csharp
public IActionResult OnGetProgress(int progress)
{
    var next = Math.Clamp(progress + 10, 0, 100);
    return Partial("_Progress", new ProgressState(next));
}
```

Put the handler in `Pages/Jobs/Status.cshtml.cs` and render the initial partial in `Pages/Jobs/Status.cshtml`:

```razor
@await Html.PartialAsync("_Progress", new ProgressState(0))
```

Polling stops when the server returns the completed element without the request and trigger attributes.
For a real job, send a job ID and read its progress from a server-side store. Do not treat the client's
`progress` parameter as actual job state.
This pattern does not depend on status code `286`, whose behavior [differs between HTMX versions](version-compatibility.md#polling).

## Handle boosted navigation progressively

Start with a real link so navigation works without JavaScript:

```razor
<a asp-page="/Catalog"
   asp-route-category="books"
   hx-boost="true"
   hx-target="#main">
    Books
</a>
```

Return a fragment for the boosted request and a page otherwise:

```csharp
public IActionResult OnGet(string category)
{
    Products = catalog.List(category);

    if (Request.IsHtmxPartialRequest())
        return Partial("_Catalog", Products);

    return Page();
}
```

Keep the `#main` target in the layout and let `_Catalog` contain its contents. `catalog` is your data service.
The link targets `#main`, so its request is partial; history restoration and direct visits get the full page.
Include `<htmx-config />` in
full documents; partial responses can retain the token already held by the Toolkit script.
See [Full pages and fragments](pages-and-fragments.md).

## Redirect after authentication

HTMX does not automatically turn an ordinary server redirect into a full browser navigation in every workflow.
Send `HX-Redirect` for the HTMX path and preserve a normal redirect fallback:

```csharp
[HttpPost]
public IActionResult SignIn(LoginInput input)
{
    if (!auth.TrySignIn(input))
    {
        ModelState.AddModelError("", "Invalid email or password.");
        return Request.IsHtmxRequest()
            ? PartialView("_SignInForm", input)
            : View(input);
    }

    var url = Url.Action("Index", "Dashboard")!;

    if (Request.IsHtmxRequest())
    {
        Response.Htmx(htmx => htmx.Redirect(url));
        return Ok();
    }

    return Redirect(url);
}
```

`auth` stands for your sign-in service. A failed sign-in returns the form with an error and status 200,
so HTMX 2.x swaps it with its default response handling; a 401 response would leave the page unchanged.
After a successful sign-in, the HTMX response uses status 200 so HTMX can process `HX-Redirect`.
A 3xx response would be followed by the browser internally, hiding the intermediate `HX-Redirect` header from HTMX.
The normal request still uses the conventional ASP.NET Core redirect.

## Load more items

A load-more response must update both the list and the next cursor. The following example uses 25 generated
orders so it can run without a database. Add these types to `HtmxStarter.Models`:

```csharp
public sealed record OrderSummary(int Id, string Name);
public sealed record OrderBatch(OrderSummary[] Items, int? NextCursor);
```

Create `Pages/Orders.cshtml`:

```razor
@page
@model OrdersModel

<h1>Orders</h1>
<ul id="orders"></ul>
<div id="load-more">
    <button hx-get asp-page="/Orders" asp-page-handler="More" asp-route-after="0"
            hx-target="#orders" hx-swap="beforeend">
        Load more
    </button>
</div>
```

Create `Pages/Orders.cshtml.cs`:

```csharp
using HtmxStarter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxStarter.Pages;

public sealed class OrdersModel : PageModel
{
    public IActionResult OnGetMore(int after)
    {
        var items = Enumerable.Range(1, 25)
            .Where(id => id > after)
            .Take(5)
            .Select(id => new OrderSummary(id, $"Order {id}"))
            .ToArray();

        int? next = items.Length > 0 && items[^1].Id < 25 ? items[^1].Id : null;
        return Partial("_OrderBatch", new OrderBatch(items, next));
    }
}
```

Create `Pages/_OrderBatch.cshtml`:

```razor
@model OrderBatch

@foreach (var item in Model.Items)
{
    <li>@item.Name</li>
}
<div id="load-more" hx-swap-oob="outerHTML">
    @if (Model.NextCursor is { } cursor)
    {
        <button hx-get asp-page="/Orders" asp-page-handler="More" asp-route-after="@cursor"
                hx-target="#orders" hx-swap="beforeend">
            Load more
        </button>
    }
    else
    {
        <p>All orders loaded.</p>
    }
</div>
```

The list items are appended to `#orders`. HTMX processes the out-of-band `#load-more` element separately,
replacing the old button after every batch. The next request therefore sends the new cursor.
After five clicks, the list contains 25 orders and the button is replaced by “All orders loaded.”
In a real query, use a stable ordering and a cursor tied to that ordering.

## Search while typing

Use an input event with a delay so the server receives a request after the user pauses:

```razor
<label for="catalog-search">Search products</label>
<input id="catalog-search"
       name="query"
       type="search"
       hx-get
       asp-page="/Catalog"
       asp-page-handler="Search"
       hx-trigger="input changed delay:300ms, search"
       hx-target="#search-results"
       hx-request-timeout="5000" />

<div id="search-results" aria-live="polite"></div>
```

```csharp
public IActionResult OnGetSearch(string? query)
{
    IReadOnlyList<Product> matches = string.IsNullOrWhiteSpace(query)
        ? []
        : catalog.Search(query);

    return Partial("_SearchResults", matches);
}
```

The URL and timeout are generated by HtmxToolkit; the trigger and request synchronization behavior belong to HTMX.
For expensive searches, add cancellation and query limits on the server even when the client uses a delay.

## Edit a table row inline

This recipe uses the `Product` record from [Getting started](getting-started.md) and assumes an application
data service named `catalog` with `List`, `Get`, and `Update` methods.

Create `Pages/_ProductRow.cshtml` for a display row:

```razor
@model Product

<tr id="product-@Model.Id">
    <td>@Model.Name</td>
    <td>@Model.Price</td>
    <td>
        <button hx-get
                asp-page="/Products"
                asp-page-handler="Edit"
                asp-route-id="@Model.Id"
                hx-target="closest tr"
                hx-swap="outerHTML">
            Edit
        </button>
    </td>
</tr>
```

Render the rows in `Pages/Products.cshtml`. Here the page model exposes a `Products` property
loaded with `catalog.List()`:

```razor
<table>
    @foreach (var product in Model.Products)
    {
        <partial name="_ProductRow" model="product" />
    }
</table>
```

Add `Models/ProductInput.cs` for the edit form:

```csharp
using System.ComponentModel.DataAnnotations;

namespace HtmxStarter.Models;

public sealed class ProductInput
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }
}
```

Create `Pages/_ProductEdit.cshtml`. It posts back to another handler with the same target:

```razor
@model ProductInput

<tr id="product-@Model.Id">
    <td colspan="3">
        <form hx-post
              asp-page="/Products"
              asp-page-handler="Save"
              hx-target="closest tr"
              hx-swap="outerHTML">
            <div asp-validation-summary="All" role="alert"></div>
            <input asp-for="Id" type="hidden" />
            <input asp-for="Name" />
            <input asp-for="Price" />
            <button type="submit">Save</button>
        </form>
    </td>
</tr>
```

Add the handlers to the page model:

```csharp
public IActionResult OnGetEdit(int id)
{
    var product = catalog.Get(id);
    if (product is null)
        return NotFound();

    return Partial("_ProductEdit", new ProductInput
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price
    });
}

public IActionResult OnPostSave(ProductInput input)
{
    if (!ModelState.IsValid)
        return Partial("_ProductEdit", input);

    var product = catalog.Update(input.Id, input.Name, input.Price);
    return Partial("_ProductRow", product);
}
```

Both partials return a complete `<tr>` with the same ID. The invalid response retains the entered values and
validation errors; the valid response returns the display row. These fragments use status 200.
Add authorization, missing-record handling, and concurrency checks to your data flow; route values and
hidden inputs are client-controlled. Keep the [antiforgery setup](antiforgery.md) from the layout.

## Update related elements out of band

An endpoint can return its normal target plus another element marked for an out-of-band swap:

```razor
@model AddToCartResult

<div id="product-actions-@Model.ProductId">
    Added to cart.
</div>

<span id="cart-count" hx-swap-oob="true">
    @Model.CartCount
</span>
```

This is useful when one operation changes a row and a page-level count. For HTMX 2.x, `AllowNestedOobSwaps` controls
whether nested out-of-band elements are processed. HTMX 4.x also has `AllowEmptySwapAfterOob` for responses
that contain only out-of-band content.

## Return an error without replacing content

For a working form that shows a request error and lets the user try again, follow
[Loading states and request errors](loading-and-errors.md). It covers a loading indicator, a disabled save
button, and messages for HTTP errors, connection failures, and timeouts.

Choose the behavior at the HTMX configuration level when it should apply to the whole application.
HTMX 2.x uses `ResponseHandling`; HTMX 4.x uses `NoSwap`. For an endpoint-specific validation response, use `Retarget`
and `Reswap` to choose where and how the HTML is inserted, together with a status the client is configured to swap.
For a successful operation with no content update, return 204 or use `Reswap(HtmxSwap.None)` with a 200 response.

See the [HTMX 2.x](configuration-v2.md) and [HTMX 4.x](configuration-v4.md) configuration guides.
