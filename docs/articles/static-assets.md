# Static assets and deployment

HtmxToolkit includes the Toolkit script as an ASP.NET Core static web asset. HTMX itself is a separate
dependency. This page explains how to serve the Toolkit script, version its URL, and diagnose deployment issues.
For what the script does, see [What the Toolkit script does](antiforgery.md#what-the-toolkit-script-does).

## Load the scripts

In a Razor layout, load HTMX before the Toolkit script:

```razor
<script src="~/js/htmx.min.js"></script>
<script src="~/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"
        asp-append-version="true"></script>
```

This example assumes you have downloaded HTMX into `wwwroot/js/htmx.min.js`.
Alternatively, use the pinned CDN URL from [Getting started](getting-started.md#4-load-the-browser-scripts).

Register the standard ASP.NET Core Tag Helpers in the applicable `_ViewImports.cshtml`:

```razor
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

They resolve `~/` against the application's path base and process `asp-append-version`.
These are Razor features: a string returned by a Minimal API does not process Tag Helpers.

The package supplies both files:

```text
/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js
/_content/Ramstack.HtmxToolkit/htmx-toolkit.js
```

Use the readable file when debugging. If you use `defer`, apply it to both HTMX and the Toolkit script.
Do not use `async` for this pair: the Toolkit script needs HTMX to have loaded first.

## Serve assets with UseStaticFiles

The following setup works on ASP.NET Core 6 or later:

```csharp
app.UseStaticFiles();
app.MapRazorPages();
```

For MVC, keep `UseStaticFiles()` and map the application's controller routes instead.

With this setup, `asp-append-version="true"` appends a `?v=...` value computed from the file content.
Changing the file changes its URL. The query parameter does not itself set a cache lifetime;
cache headers come from the application's static file configuration.

## Use MapStaticAssets on ASP.NET Core 9 or later

For build-time compression and fingerprinted asset URLs, replace the preceding setup with:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
```

For MVC with conventional routing:

```csharp
app.MapStaticAssets();
app.MapDefaultControllerRoute().WithStaticAssets();
```

For a hybrid application with attribute-routed controllers:

```csharp
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapControllers().WithStaticAssets();
```

Apply `.WithStaticAssets()` to every endpoint set that renders views, including each relevant
`MapControllerRoute` or `MapAreaControllerRoute` call. ASP.NET Core reads the asset collection from
the current endpoint's metadata.

When the collection contains the script, the Razor script helper selects a fingerprinted URL such as
`htmx-toolkit.min.{fingerprint}.js`. Otherwise it falls back to `?v=...` versioning.
In production, `MapStaticAssets()` serves fingerprinted assets with long-lived, immutable caching
and supports precompressed Gzip and Brotli representations.

## Publish the application

Static web assets work with both NuGet packages and project references. Publish the application normally:

```console
dotnet publish -c Release
```

Deploy the complete publish output, including `wwwroot` and the generated static asset manifests.
The package's scripts are published under `wwwroot/_content/Ramstack.HtmxToolkit`.
Use app-relative URLs in Razor so an application hosted under a path such as `/shop` requests
`/shop/_content/...` instead of `/_content/...` at the domain root.

When running from build output outside `Development`, enable static web asset discovery before building the app:

```csharp
builder.WebHost.UseStaticWebAssets();
```

That extra call is unnecessary for published output.

## Check a deployment

Open the rendered script URL directly. It should return JavaScript with HTTP 200, not an HTML error or login page.
Then check that the loaded HTMX major version matches `UseHtmxV1`, `UseHtmxV2`, or `UseHtmxV4`.
The [troubleshooting guide](troubleshooting.md#toolkit-script-returns-404) covers missing assets.

For automatic token delivery, also render `<htmx-config />` in the document head and configure
server-side validation as described in [Antiforgery](antiforgery.md).
