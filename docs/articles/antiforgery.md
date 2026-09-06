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

Map the script endpoint in `Program.cs`:

```csharp
using Ramstack.HtmxToolkit.Hosting;

var app = builder.Build();

app.MapHtmxToolkitScript();
app.MapRazorPages();
```

Render configuration metadata in `<head>`, then load HTMX before the Toolkit script:

```html
<head>
    <htmx-config />
</head>
<body>
    @RenderBody()

    <script src="~/js/htmx.min.js"></script>
    <script src="@Html.HtmxToolkitScriptPath()"></script>
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

## Script endpoint and caching

The default endpoint path contains a content hash:

```text
/htmxtoolkit/{content-hash}
```

It returns the minified script with `Cache-Control: public,max-age=31536000`. A new embedded script receives a new default URL.

Pass a custom path when routing conventions require one:

```csharp
app.MapHtmxToolkitScript("/assets/htmx-toolkit.js");
```

When using a stable custom path, account for cache invalidation in deployment or proxy configuration.

Request the readable script while diagnosing browser behavior:

```html
<script src="@Html.HtmxToolkitScriptPath(debug: true)"></script>
```

The debug URL adds `?debug` and the endpoint returns the unminified asset.

## Inline the script

Applications that cannot map the endpoint can render the embedded asset inside a script element:

```html
<script>@Html.HtmxToolkitScript()</script>
```

Inlining removes a request but changes the content security policy and repeats the script in every full document.
Prefer the cacheable endpoint for most applications.

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
- An inline Toolkit script may require a CSP nonce or hash. The endpoint form works naturally with a policy that allows scripts from the application's origin.

If a protected request returns 400, see [Troubleshooting](troubleshooting.md#post-returns-http-400).
