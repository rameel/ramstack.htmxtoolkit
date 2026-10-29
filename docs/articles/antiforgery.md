# Antiforgery and Toolkit script

The Toolkit script delivers ASP.NET Core antiforgery tokens with HTMX requests. This page explains
how that works and what the server still has to do, and lists the script's other features.

## What the Toolkit script does

The Toolkit script, `htmx-toolkit.min.js`, comes with the package as a static web asset.
It is not the HTMX library itself. Load it after HTMX on every page that uses HTMX;
see [Static assets and deployment](static-assets.md) for the URL and caching.

| Feature                                                                                                                                                                                      | HTMX versions                                               | Without the script                                                                                                         |
|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------|
| Adds the antiforgery token to non-GET HTMX requests to the page's origin and refreshes it after boosted navigation                                                                           | All                                                         | Non-GET requests without a token field fail server-side antiforgery validation, usually with HTTP 400                      |
| Sends `HX-Request-Type`; see [Check full or partial requests](requests.md#check-full-or-partial-requests)                                                                                    | 1.9.x and 2.x; HTMX 4.x sends it itself                     | `IsHtmxPartialRequest()` and `IsHtmxFullRequest()` return `false`, and `[HtmxRequest]` with a `RequestType` does not match |
| Replays events whose name repeats in one response header; see [Timing and repeated events](responses.md#timing-and-repeated-events)                                                          | All                                                         | None of the events in that header fire                                                                                     |
| Adds the `ramstack-swap` extension with `innerMorph`, `outerMorph`, and a simplified `outerSync`, plus `textContent` for HTMX 1.9.x; see [Morph swaps](version-compatibility.md#morph-swaps) | 1.9.x and 2.x; HTMX 4.x supports these swap styles natively | These swap styles are not available                                                                                        |

## How automatic antiforgery works

1. `<htmx-config />` asks ASP.NET Core for an antiforgery token.
2. The Tag Helper writes the request token, header name, and form-field name as data attributes on `<meta name="htmx-config">`.
3. The Toolkit script reads those attributes during page load.
4. Before each non-GET HTMX request to the page's origin, the script adds the token unless the form data already contains the configured antiforgery form field.
5. The application validates the token through Razor Pages, an MVC filter, or its Minimal API validation setup.

The token is sent as the configured antiforgery header when a header name is available.
Otherwise, it is added to the request parameters under the configured form-field name.

## Configure the layout

Use the registration and layout from [Getting started](getting-started.md). Render `<htmx-config />`
in the document head and load HTMX before the Toolkit script. Both pieces are needed for automatic token delivery.
See [Static assets and deployment](static-assets.md) for script URLs, caching, and `MapStaticAssets()`.

## Enable server-side validation

Sending a token and validating it are separate steps. `AddHtmxToolkit()` configures toolkit services;
it does not add an antiforgery validation filter or middleware to your application.

### Razor Pages

Razor Pages validates unsafe requests by default. Keep that behavior enabled. The
[forms tutorial](forms-and-validation.md) shows model binding, validation errors, and a successful save.

### MVC

Enable validation for unsafe controller actions, for example with a global filter:

```csharp
using Microsoft.AspNetCore.Mvc;

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
```

You can instead apply `[ValidateAntiForgeryToken]` to individual POST actions, but then every relevant action
needs that attribute. `[HtmxRequest]` only affects action selection; it does not validate a token or authorize a user.

### Minimal APIs

MVC antiforgery attributes do not protect Minimal API delegates. Register `AddAntiforgery()` and use an
appropriate validation path for your endpoints. The [Minimal API example](minimal-apis.md) explicitly calls
`IAntiforgery.IsRequestValidAsync()` before processing the form and returns 400 when validation fails.

For endpoints that bind form parameters through ASP.NET Core's built-in antiforgery support, configure
`UseAntiforgery()` after authentication and authorization middleware when those are present. Follow the
[ASP.NET Core antiforgery guide](https://learn.microsoft.com/aspnet/core/security/anti-request-forgery)
for the target framework and endpoint style. Loading the Toolkit script alone does not enable server validation.

## Submit a protected form

For a Razor Page with a `Save` POST handler:

```razor
<form hx-post
      asp-page="/Account/Profile"
      asp-page-handler="Save"
      hx-target="#save-result">
    <label>
        Display name
        <input name="displayName" required />
    </label>
    <button type="submit">Save</button>
</form>

<div id="save-result"></div>
```

```csharp
public IActionResult OnPostSave(string? displayName)
{
    if (string.IsNullOrWhiteSpace(displayName))
        return Content("Display name is required.");

    return Content("Profile saved.");
}
```

The standard Form Tag Helper renders the form `action` and a hidden token input from the same `asp-*` attributes.
The Toolkit script detects that input and does not add a second token. For HTMX requests without a token input,
such as a button outside a form, the layout metadata and Toolkit script supply the token.
The validation message returns 200 so HTMX 2.x displays it with its default response handling.
To make the form also work without JavaScript, return a full page for normal requests, as shown in the
[forms tutorial](forms-and-validation.md).

## Token refresh after boosted navigation

After a boosted navigation, the Toolkit script looks for antiforgery metadata in the response. If it contains
a non-empty request token, the script updates the saved token and associated names. A fragment without token
metadata leaves the existing values in place.

Keep `<htmx-config />` in full document layouts. After signing in, signing out, or changing identity,
obtain a token for the new identity. A full navigation with `HX-Redirect` is a straightforward way to do that;
see [Redirect after authentication](recipes.md#redirect-after-authentication). Keeping an old token does not refresh it.

## Requests to other origins

The script adds the token only to requests to the page's origin. Requests to another origin, including another
subdomain of the same site, do not receive it, so the token cannot leak to a third party.
If another origin of the application validates the same antiforgery tokens, send the token explicitly:

```razor
@inject Microsoft.AspNetCore.Antiforgery.IAntiforgery Antiforgery

<button hx-post="https://orders.example.com/orders"
        hx-header-RequestVerificationToken="@Antiforgery.GetAndStoreTokens(Context).RequestToken">
    Place order
</button>
```

Such a request also needs CORS with credentials on the other origin, and the antiforgery cookie must be
available to it.

## Static web assets and caching

Script loading, versioned URLs, and deployment are covered in [Static assets and deployment](static-assets.md).
Do not put HTML containing user-specific antiforgery tokens in a shared response cache.
See [Caching full pages and fragments](pages-and-fragments.md#cache-full-pages-and-fragments-separately)
when one URL can return different HTML.

## Change the header or field name

Configure names through ASP.NET Core's antiforgery options:

```csharp
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.FormFieldName = "__CsrfToken";
});
```

The configuration Tag Helper reads these options. You do not need to repeat the names in client-side code.
A form field already present under the configured name takes precedence over automatic token injection.

## Disable automatic antiforgery

Disable metadata only when another integration attaches a valid token:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = false;
    options.UseHtmxV2();
});
```

Keep loading the Toolkit script: its [other features](#what-the-toolkit-script-does) do not depend on antiforgery metadata.

## Security considerations

- Antiforgery protects cookie-authenticated state-changing requests; it does not replace authentication or authorization.
- A custom `hx-header-*` value is client-controlled and must not be trusted as proof of identity.
- Cross-origin permissions still require correct ASP.NET Core CORS and credential configuration.
- The Toolkit static web asset works with a CSP policy that allows scripts from the application's origin.

If a protected request returns 400, see [Troubleshooting](troubleshooting.md#post-returns-http-400).
