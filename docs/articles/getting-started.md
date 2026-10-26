# Getting started

Build a Razor Pages application that loads product details without leaving the page. You will create the
page, its handler, and a partial view, then inspect the request in the browser.

This tutorial uses the .NET 10 SDK and HTMX 2.x. The library itself supports .NET 6 or later.
You only need basic Razor Pages knowledge; no JavaScript code is required.
If HTMX is new to you, read [How HTMX updates a page](overview.md#how-htmx-updates-a-page) first.
For other application styles, see [MVC](mvc.md) or [Minimal APIs](minimal-apis.md).

## 1. Create the application

Run these commands in the directory where you keep your projects:

```console
dotnet new webapp -n HtmxStarter -f net10.0
cd HtmxStarter
dotnet add package Ramstack.HtmxToolkit
```

If you already have a Razor Pages application, install the package there and use your own namespace
in the examples below.

## 2. Register the toolkit

Replace `Program.cs` with:

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

`AddHtmxToolkit()` selects HTMX 2.x by default. It registers toolkit options; it does not download HTMX.
`UseStaticFiles()` serves the Toolkit script included in the package.
See [Static assets and deployment](static-assets.md) for `MapStaticAssets()` and production caching.

## 3. Enable the Tag Helpers

Replace `Pages/_ViewImports.cshtml` with:

```razor
@using HtmxStarter
@using HtmxStarter.Models
@using Ramstack.HtmxToolkit
@namespace HtmxStarter.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Ramstack.HtmxToolkit
```

The standard ASP.NET Core helpers resolve `~/` paths and generate versioned script URLs.
The toolkit helpers turn standard routing attributes such as `asp-page` and `asp-route-id` into HTMX request URLs.
You will add the `HtmxStarter.Models` namespace in step 5.

## 4. Load the browser scripts

Replace `Pages/Shared/_Layout.cshtml` with:

```razor
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>HtmxStarter</title>
    <htmx-config />
</head>
<body>
    <main>@RenderBody()</main>

    <script src="https://cdn.jsdelivr.net/npm/htmx.org@2.0.11/dist/htmx.min.js"
            integrity="sha384-2OatzQy1H+Zd/IIrjr1TcuDGqLXeHhbooAyJY1KdQMKnr4LZ22k31GBLdYKHmVjg"
            crossorigin="anonymous"></script>
    <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
            asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

The first script is HTMX, loaded from a CDN. The second is the Toolkit script, served by your application.
Keep that order. The template's `Pages/_ViewStart.cshtml` already selects this layout.
For everything the Toolkit script does, see [What the Toolkit script does](antiforgery.md#what-the-toolkit-script-does).

`<htmx-config />` renders configuration and antiforgery metadata in the document head.
The Toolkit script uses that metadata when a form later sends a POST request.
For local hosting of HTMX, download the same file into `wwwroot/js/htmx.min.js` and change its script URL to
`~/js/htmx.min.js`. See the [HTMX installation guide](https://htmx.org/docs/#installing).

## 5. Add a model and partial view

Create `Models/Product.cs`:

```csharp
namespace HtmxStarter.Models;

public sealed record Product(int Id, string Name, decimal Price);
```

Create `Pages/_ProductDetails.cshtml`:

```razor
@model Product

<article>
    <h2>@Model.Name</h2>
    <p>Product #@Model.Id · Price: @Model.Price</p>
    <p>In stock.</p>
</article>
```

A partial contains only the HTML needed for an update. It has no `@page` directive and does not render
the layout. Razor encodes model values when it writes them into HTML.

## 6. Add the page and handler

Create `Pages/Products.cshtml`:

```razor
@page
@model ProductsModel

<h1>Products</h1>

<button hx-get
        asp-page="/Products"
        asp-page-handler="Details"
        asp-route-id="42"
        hx-target="#product-details">
    View product
</button>

<div id="product-details" aria-live="polite">
    Select a product.
</div>
```

Create `Pages/Products.cshtml.cs`:

```csharp
using HtmxStarter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxStarter.Pages;

public sealed class ProductsModel : PageModel
{
    public IActionResult OnGetDetails(int id)
    {
        if (id != 42)
            return NotFound();

        var product = new Product(42, "Notebook", 12.50m);
        return Partial("_ProductDetails", product);
    }
}
```

The data is created in the handler to keep this example self-contained. In an application, load it from
your data service before returning the partial.

## 7. Run it

```console
dotnet run --no-launch-profile -- --urls http://localhost:5080 --environment Development
```

Open <http://localhost:5080/Products> and select **View product**. The product details should replace
“Select a product.” without a full page load.

Inspect the rendered button. Its routing attributes have become a URL similar to:

```html
<button hx-get="/Products?id=42&amp;handler=Details" hx-target="#product-details"
        formaction="/Products?id=42&amp;handler=Details">
    View product
</button>
```

The standard Form Action Tag Helper renders `formaction` from the same attributes; it matters only
when the button submits a form. Query parameter order may differ. The important parts are the page path, `id=42`, and `handler=Details`.

In the browser Network panel, the request should have `HX-Request: true`, return HTTP 200,
and contain only the `<article>` in its response. HTMX's default swap replaces the target's contents,
so the `#product-details` container remains available for another click.
The request should also have `HX-Request-Type: partial`. HTMX 2.x doesn't send this header itself;
the Toolkit script adds it, so seeing it confirms that the script is loaded.

If nothing happens, start with [Clicking does not send a request](troubleshooting.md#clicking-does-not-send-a-request).

## Next steps

Continue with [Forms, validation, and events](forms-and-validation.md). It adds a complete POST flow
and refreshes a second component after a successful save. Then add
[loading states and request errors](loading-and-errors.md) to handle slow or failed requests.

- [Full pages and fragments](pages-and-fragments.md) explains when to return each kind of response.
- [Tag Helpers](tag-helpers.md) covers URL generation, headers, values, and request options.
- [Run the examples](samples.md) points to the larger demo and its source files.
