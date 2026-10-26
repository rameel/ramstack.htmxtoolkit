# Use HtmxToolkit with MVC

This example uses ASP.NET Core MVC on .NET 10 and the default HTMX 2.x target.
It loads product details into a partial view, then edits the product name with server validation.
The request and response APIs are the same as in Razor Pages;
MVC uses controller actions and `PartialView()` instead of page handlers and `Partial()`.

## Create and configure the application

```console
dotnet new mvc -n HtmxMvc -f net10.0
cd HtmxMvc
dotnet add package Ramstack.HtmxToolkit
```

Replace `Program.cs` with:

```csharp
using HtmxMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddHtmxToolkit();
builder.Services.AddSingleton<ProductStore>();

var app = builder.Build();

app.UseStaticFiles();
app.MapDefaultControllerRoute();

app.Run();
```

The antiforgery filter validates the POST action used below.
`AddHtmxToolkit()` does not install that filter for you.

Add these lines to the existing `Views/_ViewImports.cshtml`:

```razor
@using Ramstack.HtmxToolkit
@addTagHelper *, Ramstack.HtmxToolkit
```

Keep the template's `@using HtmxMvc.Models` and standard MVC Tag Helper registration.
Replace `Views/Shared/_Layout.cshtml` with the layout from
[Getting started](getting-started.md#4-load-the-browser-scripts). `Views/_ViewStart.cshtml` already selects it.

## Add the model and controller

Create `Models/Product.cs`:

```csharp
namespace HtmxMvc.Models;

public sealed record Product(int Id, string Name);
```

Create `Models/ProductStore.cs`:

```csharp
namespace HtmxMvc.Models;

public sealed class ProductStore
{
    public Product Current { get; set; } = new(42, "Notebook");
}
```

The store holds one shared product in memory and resets when the application restarts.
Use your application's persistence and access rules when adapting this example.

Create `Models/ProductInput.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace HtmxMvc.Models;

public sealed class ProductInput
{
    private string _name = "";

    [Required(ErrorMessage = "Enter a product name.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Use 2 to 80 characters.")]
    public string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? "";
    }
}
```

The setter trims the name before model validation. A padded one-character value such as `" A "`
still fails the minimum length check.

Create `Controllers/ProductsController.cs`:

```csharp
using HtmxMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Ramstack.HtmxToolkit;

namespace HtmxMvc.Controllers;

public sealed class ProductsController : Controller
{
    private readonly ProductStore _products;

    public ProductsController(ProductStore products) => _products = products;

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Details(int id)
    {
        if (id != 42)
            return NotFound();

        return PartialView("_ProductDetails", _products.Current);
    }

    [HttpGet]
    public IActionResult Edit() =>
        View(new ProductInput { Name = _products.Current.Name });

    [HttpPost]
    public IActionResult Save(ProductInput input)
    {
        if (!ModelState.IsValid)
            return Request.IsHtmxRequest()
                ? PartialView("_ProductForm", input)
                : View("Edit", input);

        _products.Current = new Product(42, input.Name);
        ModelState.Clear();
        ViewData["Saved"] = true;

        return Request.IsHtmxRequest()
            ? PartialView("_ProductForm", input)
            : RedirectToAction(nameof(Edit));
    }
}
```

`Details` is a dedicated fragment action. If the same URL must also work as a complete browser page,
use the approach in [Full pages and fragments](pages-and-fragments.md).

## Add the views

Create `Views/Products/Index.cshtml`:

```razor
<h1>Products</h1>

<button hx-get
        asp-controller="Products"
        asp-action="Details"
        asp-route-id="42"
        hx-target="#product-details">
    View product
</button>

<div id="product-details" aria-live="polite">Select a product.</div>

<p><a asp-action="Edit">Edit the product name</a></p>
```

Create `Views/Products/_ProductDetails.cshtml`:

```razor
@model Product

<article>
    <h2>@Model.Name</h2>
    <p>Product #@Model.Id is in stock.</p>
</article>
```

## Add the edit form

Create `Views/Products/Edit.cshtml`:

```razor
@model ProductInput

<h1>Edit product</h1>

@await Html.PartialAsync("_ProductForm", Model)

<p><a asp-action="Index">Back to products</a></p>
```

Create `Views/Products/_ProductForm.cshtml`:

```razor
@model ProductInput

<form id="product-editor"
      method="post" hx-post asp-controller="Products" asp-action="Save"
      hx-target="this" hx-swap="outerHTML" novalidate>
    <div asp-validation-summary="All" role="alert"></div>

    <label asp-for="Name">Product name</label>
    <input asp-for="Name" />
    <button type="submit">Save</button>

    @if (ViewData["Saved"] is true)
    {
        <p role="status">Product saved.</p>
    }
</form>
```

`asp-for` produces a named input that MVC binds to `ProductInput.Name`.
The standard Form Tag Helper generates a hidden antiforgery token, which the global filter validates
before the action runs. Keep the layout and Tag Helper registrations from the setup above.

An invalid POST returns the form with its `ModelState` errors and does not update the store.
A valid POST stores the name, clears the submitted values from `ModelState`, and returns a fresh form
showing the normalized name. Both HTMX responses use HTTP 200, so HTMX 2.x displays them with its defaults.
Every returned form keeps its ID and request attributes so it can be submitted again.

Without JavaScript, the same form returns the full Edit view on error and redirects back to Edit after a save.
`novalidate` lets you exercise server validation without the browser blocking an empty submission first.

## Run it

```console
dotnet run --no-launch-profile -- --urls http://localhost:5081 --environment Development
```

Open <http://localhost:5081/Products> and select **View product**. Inspect the generated `hx-get` URL
and the fragment returned by the controller. Then follow **Edit the product name**:

1. Submit an empty name. The form should show “Enter a product name.”
2. Submit `" A "`. The form should show the length error.
3. Submit `" Sketchbook "`. The input should show “Sketchbook” and the form should show “Product saved.”
4. Save a second name, return to Products, and load the details again. They should show the saved name.
5. Disable JavaScript and reload Edit. Invalid and valid submissions should still work through normal navigation.

The Network panel should show a POST to `/Products/Save` and a response containing only the form for HTMX.
See [Forms, validation, and events](forms-and-validation.md) to refresh a separate component after saving,
or [Loading states and request errors](loading-and-errors.md) to add browser feedback during requests.

`[HtmxRequest]` can select actions based on HTMX headers, but it is not an authorization or antiforgery check.
Use it when action selection helps the application; it is not required for every fragment endpoint.
