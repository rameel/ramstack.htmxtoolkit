# HtmxToolkit

[![NuGet](https://img.shields.io/nuget/v/Ramstack.HtmxToolkit.svg)](https://www.nuget.org/packages/Ramstack.HtmxToolkit/)
[![Build](https://github.com/rameel/ramstack.htmxtoolkit/actions/workflows/test.yml/badge.svg)](https://github.com/rameel/ramstack.htmxtoolkit/actions/workflows/test.yml)
[![License: MIT](https://img.shields.io/github/license/rameel/ramstack.htmxtoolkit)](LICENSE)

HtmxToolkit helps you build [HTMX](https://htmx.org/) applications with ASP.NET Core.

HTMX and the server talk through `HX-*` headers. Requests carry details such as the target element
and the current URL, and your response can tell HTMX where to put the HTML, whether to change the URL,
or which events to raise.
HtmxToolkit lets you do all of this from C# and Razor, without header strings or hand-written JSON:

- Find out whether HTMX expects a fragment or a full page, and read its headers as typed properties.
- Tell HTMX what to do next with a fluent API: retarget, reswap, redirect, push a URL, or trigger an event.
- Send HTMX requests to dedicated MVC actions with `[HtmxRequest]`.
- Build `hx-get` and `hx-post` URLs from controllers, actions, and Razor Pages, just like the `asp-*` Tag Helpers do.
- Configure HTMX once in `Program.cs` and send antiforgery tokens with HTMX requests automatically.

HtmxToolkit runs on .NET 6 or later and supports HTMX 1.9.x, 2.x, and 4.x. HTMX 2.x is the default.

## Getting started

**1. Install the package:**

```console
dotnet add package Ramstack.HtmxToolkit
```

The package doesn't include HTMX itself, so add HTMX to your application as usual,
from a CDN or as a local file.

**2. Register the toolkit in `Program.cs`:**

```csharp
using Ramstack.HtmxToolkit.Hosting;

builder.Services.AddHtmxToolkit();
```

By default, the toolkit targets HTMX 2.x. If you use HTMX 1.9.x or 4.x, select it as shown in [Configuration](#configuration).

**3. Enable the Tag Helpers in `_ViewImports.cshtml`:**

```html
@using Ramstack.HtmxToolkit
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Ramstack.HtmxToolkit
```

**4. Update the layout.** Add `<htmx-config />` to `<head>` and load the Toolkit script after HTMX:

```html
<head>
    ...
    <htmx-config />
</head>
<body>
    ...
    <script src="~/js/htmx.min.js"></script>
    <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
            asp-append-version="true"></script>
</body>
```

`<htmx-config />` renders your HTMX settings and the antiforgery token. The Toolkit script comes
with the package. It adds the token to HTMX requests and, with HTMX 1.x and 2.x, tells the server
whether a request expects a fragment or a full page (see [Handling requests](#handling-requests)).
The script is served like any other static file, so make sure the application calls `UseStaticFiles()`
or `MapStaticAssets()`.
See [Static assets](docs/articles/static-assets.md) for details.

**5. Send your first request.** In a view, describe where the request goes in ASP.NET Core terms:

```html
<button hx-get
        hx-controller="Books"
        hx-action="List"
        hx-route-category="science"
        hx-target="#results">
    Show science books
</button>

<div id="results"></div>
```

The toolkit turns these attributes into a regular URL, and the browser receives
`hx-get="/Books/List?category=science"`. The action returns only a piece of HTML:

```csharp
public IActionResult List(string category)
{
    var books = _catalog.FindByCategory(category);
    return PartialView("_BookList", books);
}
```

Click the button, and the list appears in `#results` without a page reload.

For a step-by-step tutorial, see [Getting started](docs/articles/getting-started.md).
If you use Minimal APIs, see [Minimal API setup](docs/articles/minimal-apis.md).

## Handling requests

A page and its fragment can share one URL. HTMX gets just the fragment,
while a direct visit or a page refresh gets the full page:

```csharp
public IActionResult Help() =>
    Request.IsHtmxPartialRequest()
        ? PartialView("_Help")
        : View();
```

Checking only whether the request came from HTMX isn't enough here. A boosted link that replaces the whole `<body>`,
or a request with `hx-select`, comes from HTMX too, but expects a full page.
`IsHtmxPartialRequest()` reads the `HX-Request-Type` header instead. The header is `partial` when HTMX swaps
the response into an element inside the page, and `full` when the response replaces the whole `<body>`
or goes through `hx-select`. A history restore always counts as a full page.

HTMX 4 sends this header itself. HTMX 1.x and 2.x don't, so the Toolkit script adds it the same way.
That's why the example above works with every supported version, as long as the script is on the page.
If you cache such responses, add a `Vary` header so that the page and the fragment are cached separately.
See [Full pages and fragments](docs/articles/pages-and-fragments.md).

To read other HTMX headers, use `IsHtmxRequest`. It gives you the headers as typed properties:

```csharp
if (Request.IsHtmxRequest(out var htmx))
{
    var target = htmx.Target;  // the element to update: "results", or "div#results" in HTMX 4
    var url = htmx.CurrentUrl; // the page the user is on
}
```

In MVC, you can also give HTMX requests their own action. An action marked with `[HtmxRequest]`
is selected only for HTMX requests, so two actions can share one URL:

```csharp
[HttpGet("/profile")]
public IActionResult Profile() => View();

[HttpGet("/profile")]
[HtmxRequest(RequestType = HtmxRequestType.Partial)]
public IActionResult ProfileFragment() => PartialView("_Profile");
```

HTMX requests for a fragment go to `ProfileFragment`. Everything else, including direct visits
and boosted navigation, goes to `Profile`. The attribute can also filter by boosted navigation.
See [Read HTMX requests](docs/articles/requests.md) for all headers and `[HtmxRequest]` options.

## Controlling responses

Use `Response.Htmx(...)` to tell HTMX what to do with your response:

```csharp
Response.Htmx(htmx => htmx
    .Retarget("#profile")
    .Reswap(HtmxSwap.OuterHtml)
    .TriggerEvent("profile-updated"));
```

The callback runs only for HTMX requests, so it is safe to call in any action.
Besides changing the target and the swap style, you can navigate to another page (`Location`, `Redirect`),
update the address bar (`PushUrl`, `ReplaceUrl`), reload the page (`Refresh`), and raise client-side events
with optional data (`TriggerEvent`). This works with any `HttpResponse`, including Minimal API handlers.

In MVC controllers, common headers can also be set with an attribute:

```csharp
[HtmxResponse(Retarget = "#results", Reswap = HtmxSwap.BeforeEnd)]
public IActionResult LoadMore() => PartialView("_MoreResults");
```

See [Control HTMX responses](docs/articles/responses.md) for all options,
including Native AOT support and overloads that avoid closures in hot paths.

## Tag Helpers

The URL attributes from the example above work with Razor Pages too:

```html
<button hx-post
        hx-page="/Orders"
        hx-page-handler="Archive"
        hx-route-id="@Model.Id">
    Archive
</button>
```

The request method comes from `hx-get`, `hx-post`, `hx-put`, `hx-patch`, or `hx-delete`.
If you leave it out, `hx-get` is used.

Other Tag Helpers build HTMX attributes that otherwise need hand-written JSON:

```html
<button hx-get="/reports"
        hx-val-format="summary"
        hx-header-X-Time-Zone="UTC"
        hx-request-timeout="5000">
    Load report
</button>
```

| You write      | You get                                                                                 |
|----------------|-----------------------------------------------------------------------------------------|
| `hx-val-*`     | `hx-vals`, extra values sent with the request                                           |
| `hx-header-*`  | `hx-headers`, extra request headers                                                     |
| `hx-request-*` | `hx-request` (HTMX 1.x, 2.x) or `hx-config` (HTMX 4), request options such as a timeout |

The toolkit builds and escapes the JSON for you.
See [Tag Helpers](docs/articles/tag-helpers.md) for the full list of attributes.

## Configuration

Choose the HTMX version you load in the browser and set its options in one place:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.ReportValidityOfForms = true;
        config.DefaultFocusScroll = true;
    });
});
```

Use `UseHtmxV1`, `UseHtmxV2`, or `UseHtmxV4`, depending on the version you use.
Each version has its own set of options. `<htmx-config />` renders only the options you set,
so everything else keeps the HTMX defaults.

See [Application configuration](docs/articles/configuration.md) for the available settings.

## Antiforgery

Antiforgery support is on by default. The Toolkit script adds the token to every HTMX request
except GET and refreshes it after boosted navigation.

The script only sends the token; ASP.NET Core still has to check it. Razor Pages does this automatically.
In MVC, enable validation yourself, for example globally:

```csharp
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
```

If antiforgery is handled some other way, turn the feature off with `options.IncludeAntiforgeryToken = false`.
See [Antiforgery](docs/articles/antiforgery.md) for more.

## HTMX versions

Most of your code doesn't depend on the HTMX version: the toolkit adapts its output to the version you select.
Where possible, it also smooths out the differences between versions:

- **HTMX 4 features in HTMX 1.9.x and 2.x.** The Toolkit script sends the `HX-Request-Type` header,
  and its `ramstack-swap` extension adds the `innerMorph`, `outerMorph`, and `textContent` swap styles,
  plus a simplified `outerSync`. To use them, enable the extension with `hx-ext` and load
  [Idiomorph](https://github.com/bigskysoftware/idiomorph) for morphing:

  ```html
  <body hx-ext="ramstack-swap">
      ...

      <script src="~/js/htmx.min.js"></script>
      <script src="https://cdn.jsdelivr.net/npm/idiomorph@0.7.4/dist/idiomorph.min.js"
              integrity="sha384-YWh9vmu1Q6BptPp1emqfsNatFASVLolFKP/5f4LWR6eGMLEhBDh1zcwU2OzfVCe9"
              crossorigin="anonymous"></script>
      <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
              asp-append-version="true"></script>
  </body>
  ```

  Then use the styles like any other: `hx-swap="outerMorph"` in markup or `Reswap(HtmxSwap.OuterMorph)`
  in a response. Idiomorph is optional: without it, `innerMorph` and `outerMorph` fall back to
  `innerHTML` and `outerHTML`, and `textContent` doesn't need it at all. HTMX 4 supports all of
  these natively, so skip the extension there.
- **Features that HTMX 4 dropped.** Where HTMX 4 has a close equivalent, the toolkit uses it.
  For example, HTMX 4 has no `HX-Trigger-After-Swap` and `HX-Trigger-After-Settle` headers,
  so events with these timings are sent in `HX-Trigger` and fire when the request completes.

Some differences can't be hidden. For example, in HTMX 4 an event can't wait for the settle step.

If you aren't sure which version to pick, see [Choose an HTMX version](docs/articles/choosing-version.md).
For all the differences, see [Version compatibility](docs/articles/version-compatibility.md).

## Demo

The [demo application](samples/Ramstack.HtmxToolkit.Demo) shows the toolkit in action: requests and responses,
MVC attributes, Tag Helpers, polling, boosted navigation, and antiforgery. It uses HTMX 4 and needs the .NET 10 SDK.
Run it from the repository root:

```console
dotnet run --project samples/Ramstack.HtmxToolkit.Demo
```

Then open <https://localhost:5001>.

## Documentation

The [guides](docs/articles/index.md) cover everything from a first tutorial to recipes and troubleshooting.

## Contributing

Bug reports and pull requests are welcome. See [Contributing](CONTRIBUTING.md) to learn how to build the project
and run the tests.

## License

HtmxToolkit is available under the [MIT License](LICENSE).
