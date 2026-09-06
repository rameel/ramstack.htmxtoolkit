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

Register Razor Pages and the toolkit in `Program.cs`, then map the companion script endpoint:

```csharp
using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHtmxToolkit();

var app = builder.Build();

app.UseStaticFiles();
app.MapHtmxToolkitScript();
app.MapRazorPages();

app.Run();
```

No configuration delegate is required for HTMX 2.x. To use another major version,
see [Choose an HTMX version](choosing-version.md).

## 3. Enable the Razor helpers

Add the namespace and Tag Helpers to `Pages/_ViewImports.cshtml`:

```html
@using Ramstack.HtmxToolkit
@addTagHelper *, Ramstack.HtmxToolkit
```

The `@using` directive makes `Html.HtmxToolkitScriptPath()` and toolkit types available.
The `@addTagHelper` directive enables attributes such as `hx-page`, `hx-route-*`, and the `<htmx-config />` element.

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
    <script src="@Html.HtmxToolkitScriptPath()"></script>
</body>
</html>
```

The default Toolkit script URL includes a content hash and is served with a one-year cache lifetime.
When the embedded script changes, its default URL changes too.

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
