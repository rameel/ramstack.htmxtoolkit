# Antiforgery and Toolkit script

The companion Toolkit script integrates ASP.NET Core antiforgery tokens with HTMX requests and supplies compatibility swaps for older HTMX versions.
It is not the HTMX library itself.

## How automatic antiforgery works

1. `<htmx-config />` asks ASP.NET Core for an antiforgery token.
2. The Tag Helper writes the request token, header name, and form-field name as data attributes on `<meta name="htmx-config">`.
3. The Toolkit script reads those attributes during page load.
4. Before each non-GET HTMX request, the script adds the token unless the form data already contains the configured antiforgery form field.
5. ASP.NET Core validates the token normally.

The token is sent as the configured antiforgery header when a header name is available.
Otherwise, it is added to the request parameters under the configured form-field name.

## Configure the layout

Enable static files in `Program.cs`:

```csharp
var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();
```

On ASP.NET Core 9 or later, use `MapStaticAssets()` and associate the asset collection with the page endpoints
instead to enable build-time compression and fingerprinted URLs:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
```

For MVC, apply `.WithStaticAssets()` to the controller endpoint builder, for example:

```csharp
app.MapStaticAssets();
app.MapDefaultControllerRoute().WithStaticAssets();
```

A hybrid Razor Pages and MVC application applies it to each endpoint set that renders views:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapControllers().WithStaticAssets();
```

`MapControllers()` covers attribute-routed controllers; with conventional or area routing, apply
`.WithStaticAssets()` to each `MapControllerRoute` or `MapAreaControllerRoute` call.

> [!NOTE]
> ASP.NET Core reads the asset collection from the current endpoint's metadata. An endpoint without it still
> renders a working URL: the resolver falls back to a `?v=...` version instead of a fingerprinted URL.

Render configuration metadata in `<head>`, then load HTMX before the Toolkit script:

```html
<head>
    <htmx-config />
</head>
<body>
    @RenderBody()

    <script src="~/js/htmx.min.js"></script>
    <script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js" asp-append-version="true"></script>
</body>
```

Both pieces are required for automatic antiforgery. The Tag Helper creates the token; the script attaches it to requests.

## Submit a protected form

Razor Pages validates non-GET handlers by default:

```html
<form hx-post
      hx-page="/Account/Profile"
      hx-page-handler="Save"
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
        return BadRequest("Display name is required.");

    return Content("Profile saved.");
}
```

No token input is required in this form because the layout metadata and Toolkit script supply it.

## Token refresh after boosted navigation

When a boosted navigation returns a new full document, the Toolkit script reads antiforgery metadata from that response
and updates the token used for later requests. Ensure the returned document contains `<htmx-config />`.

## Static web assets and caching

The NuGet package includes both script variants as ASP.NET Core static web assets:

```text
/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js
/_content/Ramstack.HtmxToolkit/htmx-toolkit.js
```

Reference the minified file from the layout with an app-relative path:

```html
<script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
        asp-append-version="true"></script>
```

This form requires `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers` in `_ViewImports.cshtml`.
ASP.NET Core resolves the `~` path and applies content-based versioning:

- On ASP.NET Core 9 or later, when the current endpoint's asset collection contains the script, the framework
  selects the fingerprinted URL, such as `htmx-toolkit.min.{fingerprint}.js`. In production, `MapStaticAssets()`
  serves fingerprinted assets with long-lived, immutable caching and supports precompressed Gzip and Brotli
  representations.
- Otherwise, `asp-append-version="true"` appends a `?v=...` version computed and cached from the file content.
  This includes ASP.NET Core 6–8 and applications using `UseStaticFiles()`.

Both forms account for the application's path base. When the file changes, its versioned URL changes. Cache
headers are managed by the application's static asset or static file configuration; adding `?v=...` does not
itself set a cache lifetime.

Static web assets work with both project references and NuGet packages. On publish, ASP.NET Core copies
the scripts into the application's `wwwroot/_content/Ramstack.HtmxToolkit` directory.

Request the readable script while diagnosing browser behavior:

```html
<script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.js"
        asp-append-version="true"></script>
```

If you use `defer`, apply it to both HTMX and the Toolkit script so their execution order is preserved.

## Disable automatic antiforgery

Disable metadata only when another integration attaches a valid token:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = false;
    options.UseHtmxV2();
});
```

The Toolkit script can still be used for morph compatibility after antiforgery metadata is disabled.

## Security considerations

- Antiforgery protects cookie-authenticated state-changing requests; it does not replace authentication or authorization.
- A custom `hx-header-*` value is client-controlled and must not be trusted as proof of identity.
- Cross-origin permissions still require correct ASP.NET Core CORS and credential configuration.
- The Toolkit static web asset works with a CSP policy that allows scripts from the application's origin.

If a protected request returns 400, see [Troubleshooting](troubleshooting.md#post-returns-http-400).
