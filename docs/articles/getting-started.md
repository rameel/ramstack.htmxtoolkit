# Getting started

This tutorial adds HtmxToolkit to an ASP.NET Core Razor Pages application and loads a product fragment
without navigating away from the page. The same registration and layout setup applies to MVC applications.

## 1. Install the package

```console
dotnet add package Ramstack.HtmxToolkit
```

Add HTMX 1.9.x, 2.x, or 4.x to the application separately. HtmxToolkit does not bundle HTMX.
This example assumes HTMX 2.x, which is the toolkit default.

## 2. Register HtmxToolkit

Register Razor Pages and the toolkit in `Program.cs`, and enable static files:

```csharp
using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHtmxToolkit();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();
```

No configuration delegate is required for HTMX 2.x. To use another major version,
see [Choose an HTMX version](choosing-version.md).

The setup above works on ASP.NET Core 6 or later. On ASP.NET Core 9 or later, use the following instead of
`UseStaticFiles()` and `MapRazorPages()` to enable build-time compression and fingerprinted asset URLs:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
```

> [!NOTE]
> In a hybrid Razor Pages and MVC application, call `.WithStaticAssets()` on every endpoint set that renders
> views, for example `app.MapControllers().WithStaticAssets()` as well.

## 3. Enable the Razor helpers

Add the namespace and Tag Helpers to `Pages/_ViewImports.cshtml`:

```html
@using Ramstack.HtmxToolkit
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Ramstack.HtmxToolkit
```

The `@using` directive makes toolkit types available.
The first `@addTagHelper` directive enables the standard ASP.NET Core Tag Helpers used by the layout snippet
(`~` path resolution and `asp-append-version`). The second enables attributes such as `hx-page` and `hx-route-*`,
as well as the `<htmx-config />` element.

## 4. Configure the layout

Render the configuration metadata in `<head>`. Load HTMX first and the Toolkit script second:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <htmx-config />
</head>
<body>
    @RenderBody()

    <script src="~/js/htmx.min.js"></script>
    <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js" asp-append-version="true"></script>
</body>
</html>
```

The Toolkit script is a static web asset supplied by the NuGet package. ASP.NET Core resolves the `~` path, and
`asp-append-version="true"` makes the URL content-based: on ASP.NET Core 9 or later the framework selects a
fingerprinted URL when available, otherwise it appends a `?v=...` version. Both forms account for the application's
path base. Cache headers are managed by the application's static asset or static file configuration.

> [!IMPORTANT]
> `<htmx-config />` and the Toolkit script work together to add ASP.NET Core antiforgery data to unsafe HTMX requests. Omitting either one disables that automatic behavior.

## 5. Add an HTMX interaction

Create `Pages/Products.cshtml`:

```html
@page
@model ProductsModel

<h1>Products</h1>

<button hx-page="/Products"
        hx-page-handler="Details"
        hx-route-id="42"
        hx-target="#product-details">
    View product
</button>

<div id="product-details" aria-live="polite">
    Select a product.
</div>
```

Because an `hx-page` is present and no method was specified, `HtmxUrlTagHelper` generates an `hx-get` URL
for the Razor Page handler.

Add the handler to `Pages/Products.cshtml.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public sealed class ProductsModel : PageModel
{
    public IActionResult OnGetDetails(int id) =>
        Content($"<article><h2>Product #{id}</h2><p>In stock.</p></article>", "text/html");
}
```

Run the application and select **View product**. In browser developer tools, the request contains HTMX headers
and the returned `<article>` is inserted into `#product-details`.

For a real application, prefer a partial view over assembling HTML in a string:

```csharp
public IActionResult OnGetDetails(int id) =>
    Partial("_ProductDetails", catalog.Get(id));
```

## Next steps

- [Full pages and fragments](pages-and-fragments.md) makes one URL work with and without HTMX.
- [Read HTMX requests](requests.md) explains request detection and request headers.
- [Control HTMX responses](responses.md) lets the server change the target, swap, URL, or client behavior.
- [Antiforgery and Toolkit script](antiforgery.md) explains protected POST requests.
