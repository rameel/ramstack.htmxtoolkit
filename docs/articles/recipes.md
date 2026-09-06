# Common recipes

These recipes combine ordinary HTMX behavior with HtmxToolkit's routing, request, response, and security APIs.
Adapt partial names and persistence code to the application.

## Return validation errors to another target

Post a form whose normal target is the saved profile:

```html
<form hx-post
      hx-page="/Profile"
      hx-page-handler="Save"
      hx-target="#profile">
    <div id="validation-errors"></div>
    <input asp-for="Input.DisplayName" />
    <button type="submit">Save</button>
</form>

<section id="profile"></section>
```

Retarget only the invalid response:

```csharp
public IActionResult OnPostSave()
{
    if (!ModelState.IsValid)
    {
        Response.Htmx(htmx => htmx
            .Retarget("#validation-errors")
            .Reswap(HtmxSwap.InnerHtml));

        return Partial("_ValidationSummary", ModelState);
    }

    profiles.Save(Input);
    return Partial("_Profile", Input);
}
```

Automatic antiforgery requires the layout setup from [Antiforgery and Toolkit script](antiforgery.md).

## Refresh another component after saving

The server can dispatch a named event without custom JavaScript:

```csharp
public IActionResult OnPostSave(ProfileInput input)
{
    profiles.Save(input);

    Response.Htmx(htmx => htmx.TriggerEvent(
        "profile-saved",
        new { input.Id },
        HtmxTriggerTiming.AfterSwap));

    return Partial("_SaveResult", input);
}
```

Another element listens for the event and reloads itself:

```html
<aside id="profile-summary"
       hx-page="/Profile"
       hx-page-handler="Summary"
       hx-trigger="profile-saved from:body">
    @await Html.PartialAsync("_ProfileSummary", Model.Profile)
</aside>
```

This keeps the server response in control while allowing independently targeted components to stay synchronized.

## Poll a background operation

Return markup that contains the next poll while work is incomplete:

```html
@model ProgressState

@if (Model.Completed)
{
    <div id="job-progress">Complete</div>
}
else
{
    <div id="job-progress"
         hx-page="/Jobs/Status"
         hx-page-handler="Progress"
         hx-route-progress="@Model.Percent"
         hx-trigger="every 500ms"
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

Polling stops naturally because completed markup no longer contains `hx-trigger="every ..."`.

## Handle boosted navigation progressively

Start with a real link so navigation works without JavaScript:

```html
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

    if (Request.IsHtmxBoosted())
        return Partial("_Catalog", Products);

    return Page();
}
```

If the same URL participates in history restoration, use the fuller check from [Full pages and fragments](pages-and-fragments.md).

## Redirect after authentication

HTMX does not automatically turn an ordinary server redirect into a full browser navigation in every workflow.
Send `HX-Redirect` for the HTMX path and preserve a normal redirect fallback:

```csharp
public IActionResult SignIn(LoginInput input)
{
    if (!auth.TrySignIn(input))
        return Unauthorized();

    if (Request.IsHtmxRequest())
    {
        Response.Htmx(htmx => htmx.Redirect("/dashboard"));
        return Ok();
    }

    return Redirect("/dashboard");
}
```

The HTMX response uses status 200 so HTMX can process `HX-Redirect`.
A 3xx response would be followed by the browser internally, hiding the intermediate `HX-Redirect` header from HTMX.
The normal request still uses the conventional ASP.NET Core redirect.

## Load more items

Append a fragment and let the returned markup contain the next cursor:

```html
<button hx-page="/Orders"
        hx-page-handler="More"
        hx-route-after="@Model.NextCursor"
        hx-target="#orders"
        hx-swap="beforeend">
    Load more
</button>
```

The returned partial should contain only new rows. Replace or remove the button separately,
for example with an out-of-band element, when there is no next page.

## Search while typing

Use an input event with a delay so the server receives a request after the user pauses:

```html
<label for="catalog-search">Search products</label>
<input id="catalog-search"
       name="query"
       type="search"
       hx-page="/Catalog"
       hx-page-handler="Search"
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

Load an edit partial into the row itself:

```html
<tr id="product-@product.Id">
    <td>@product.Name</td>
    <td>@product.Price</td>
    <td>
        <button hx-page="/Products"
                hx-page-handler="Edit"
                hx-route-id="@product.Id"
                hx-target="closest tr"
                hx-swap="outerHTML">
            Edit
        </button>
    </td>
</tr>
```

The edit partial posts back to another handler with the same target:

```html
@model ProductInput

<tr id="product-@Model.Id">
    <td colspan="3">
        <form hx-post
              hx-page="/Products"
              hx-page-handler="Save"
              hx-target="closest tr"
              hx-swap="outerHTML">
            <input asp-for="Id" type="hidden" />
            <input asp-for="Name" />
            <input asp-for="Price" />
            <button type="submit">Save</button>
        </form>
    </td>
</tr>
```

On invalid input, return the edit partial with validation messages. On success, return the display-row partial.
Keep authorization and concurrency checks in both handlers; route values and hidden inputs are client-controlled.

## Update related elements out of band

An endpoint can return its normal target plus another element marked for an out-of-band swap:

```html
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

Choose the behavior at the HTMX configuration level when it should apply to the whole application.
HTMX 2.x uses `ResponseHandling`; HTMX 4.x uses `NoSwap`. For an endpoint-specific validation response, `Retarget`
and `Reswap` usually make the intent clearer.

See the [HTMX 2.x](configuration-v2.md) and [HTMX 4.x](configuration-v4.md) configuration guides.
