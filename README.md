# HtmxToolkit
[![NuGet](https://img.shields.io/nuget/v/Ramstack.HtmxToolkit.svg)](https://nuget.org/packages/Ramstack.HtmxToolkit)
[![MIT](https://img.shields.io/github/license/rameel/ramstack.htmxtoolkit)](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/LICENSE)

Provides HTMX integration for ASP.NET Core applications.

<!-- TOC -->
* [HtmxToolkit](#htmxtoolkit)
  * [Getting Started](#getting-started)
  * [HttpRequest](#httprequest)
    * [HtmxRequestAttribute](#htmxrequestattribute)
  * [HttpResponse](#httpresponse)
    * [The declarative way of setting response headers](#the-declarative-way-of-setting-response-headers)
  * [Tag Helpers](#tag-helpers)
    * [HtmxUrlTagHelper](#htmxurltaghelper)
    * [HtmxHeaderTagHelper](#htmxheadertaghelper)
    * [HtmxValsTagHelper](#htmxvalstaghelper)
    * [HtmxRequestTagHelper](#htmxrequesttaghelper)
    * [HtmxConfigTagHelper](#htmxconfigtaghelper)
      * [Response Handling Configuration](#response-handling-configuration)
  * [Toolkit Script](#toolkit-script)
  * [Supported Versions](#supported-versions)
  * [Contributions](#contributions)
  * [License](#license)
<!-- TOC -->

## Getting Started

Add the [`Ramstack.HtmxToolkit` NuGet package](https://www.nuget.org/packages/Ramstack.HtmxToolkit/)
to your project with the following command:

```console
dotnet add package Ramstack.HtmxToolkit
```

Register the toolkit and select the HTMX version used by the application:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.DefaultSwapStyle = HtmxSwap.OuterHtml;
        config.Timeout = 5000;
        config.GlobalViewTransitions = true;
    });
});
```

HTMX 2.x is used by default. Calling `UseHtmxV2` is optional when no version-specific
settings are required:

```csharp
builder.Services.AddHtmxToolkit();
```

## HttpRequest

The library provides the `HttpRequestExtensions` class for working with `HttpRequest`.

```csharp
/// <summary>
/// Provides extension methods for the <see cref="HttpRequest"/> class.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Determines whether the specified HTTP request is an HTMX request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is an HTMX request;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request);

    /// <summary>
    /// Determines whether the specified HTTP request is an HTMX request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this method returns, contains the <see cref="HtmxRequestHeaders"/>
    /// that provides access to well-known HTMX headers.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is an HTMX request; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request, out HtmxRequestHeaders headers);

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is boosted; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request);

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this method returns, contains the <see cref="HtmxRequestHeaders"/>
    /// that provides access to well-known HTMX headers.</param>
    /// <returns>
    /// <see langword="true" /> if the specified HTTP request is boosted; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request, out HtmxRequestHeaders headers);

    /// <summary>
    /// Returns a strongly typed view of the HTMX request headers.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// The <see cref="HtmxRequestHeaders" />.
    /// </returns>
    public static HtmxRequestHeaders GetHtmxHeaders(this HttpRequest request);
}
```

Use `IsHtmxRequest` to determine whether the current request was issued by HTMX.

```csharp
HttpContext.Request.IsHtmxRequest()
```

You can then handle HTMX and regular requests differently, for example:

```csharp
if (Request.IsHtmxRequest())
    return PartialView();

return View();
```

The overloads with an `out` parameter also provide access to strongly typed headers set by HTMX:

```csharp
if (Request.IsHtmxRequest(out var headers))
{
    if (headers.HistoryRestoreRequest)
    {
        ...
    }
}
```

You can also access strongly typed headers by calling `GetHtmxHeaders`:

```csharp
var headers = Request.GetHtmxHeaders();
```

The complete set of request header properties is shown below:

```csharp
/// <summary>
/// Represents strongly typed HTMX request headers.
/// </summary>
public readonly struct HtmxRequestHeaders
{
    /// <summary>
    /// Gets a value indicating whether the request was made using AJAX instead of a normal navigation.
    /// </summary>
    public bool Boosted { get; }

    /// <summary>
    /// Gets the current URL of the browser.
    /// </summary>
    public string? CurrentUrl { get; }

    /// <summary>
    /// Gets a value indicating whether the request restores history after a miss in the local history cache.
    /// </summary>
    public bool HistoryRestoreRequest { get; }

    /// <summary>
    /// Gets the user's response to an <c>hx-prompt</c> on the client.
    /// </summary>
    public string? Prompt { get; }

    /// <summary>
    /// Gets a value indicating whether the current request is an HTMX request.
    /// </summary>
    public bool Request { get; }

    /// <summary>
    /// Gets the ID of the target element, if present.
    /// </summary>
    public string? Target { get; }

    /// <summary>
    /// Gets the name of the triggered element, if present.
    /// </summary>
    public string? TriggerName { get; }

    /// <summary>
    /// Gets the ID of the triggered element, if present.
    /// </summary>
    public string? Trigger { get; }
}
```

For example:

```csharp
if (Request.GetHtmxHeaders().HistoryRestoreRequest)
{
    ...
}
```

The `HtmxRequestHeaderNames` class also provides constants for well-known request header names,
so you do not have to remember their exact spelling.

```csharp
/// <summary>
/// Defines constants for the well-known names of HTMX request headers.
/// </summary>
/// <remarks>
/// For more information, see <see href="https://htmx.org/reference/#request_headers">HTMX Request Headers Reference</see>.
/// </remarks>
public static class HtmxRequestHeaderNames
{
    /// <summary>
    /// The <c>HX-Boosted</c> header indicates whether the request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    public const string Boosted = "HX-Boosted";

    /// <summary>
    /// The <c>HX-Current-URL</c> header contains the current URL of the browser.
    /// </summary>
    public const string CurrentUrl = "HX-Current-URL";

    ...
    // The list of other constants is omitted for brevity
}
```

### HtmxRequestAttribute

To route HTMX requests to a specific controller action, apply the `HtmxRequestAttribute`
action constraint to that action:

```csharp
public class UserController : ControllerBase
{
    [HtmxRequest]
    public IActionResult UpdateProfile(UserProfile profile)
    {
        ...
    }
}
```

To match only boosted requests, set the `Boosted` property to `true`:

```csharp
public class UserController : ControllerBase
{
    ...
    [HtmxRequest(Boosted = true)]
    public IActionResult UpdateProfile(UserProfile profile)
    {
        ...
    }
}
```

## HttpResponse

For working with response headers, the library provides the `HttpResponseExtensions` class:

```csharp
/// <summary>
/// Provides extension methods for the <see cref="HttpResponse"/> class.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Returns a strongly typed view of the HTMX response headers.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>
    /// The <see cref="HtmxResponseHeaders" />.
    /// </returns>
    public static HtmxResponseHeaders GetHtmxHeaders(this HttpResponse response);

    /// <summary>
    /// Configures the HTMX response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The delegate that configures the HTMX response headers.</param>
    public static void Htmx(this HttpResponse response, Action<HtmxResponse> configure);

    /// <summary>
    /// Configures the HTMX response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The delegate that configures the HTMX response headers
    /// using <paramref name="state" />.</param>
    /// <param name="state">The state passed to <paramref name="configure" />.</param>
    public static void Htmx<TState>(this HttpResponse response, Action<HtmxResponse, TState> configure, TState state);
}
```

The `GetHtmxHeaders` method provides access to strongly typed response headers
that control HTMX behavior.

```csharp
/// <summary>
/// Represents strongly typed HTMX response headers.
/// </summary>
public readonly struct HtmxResponseHeaders
{
    /// <summary>
    /// Gets or sets the value of the <c>HX-Location</c> header, which performs
    /// a client-side redirect without a full-page reload.
    /// </summary>
    [MaybeNull]
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the value of the <c>HX-Push-Url</c> header, which pushes a new URL
    /// onto the browser's history stack.
    /// </summary>
    [MaybeNull]
    public string PushUrl { get; set; }

    ...
    // The remaining properties are omitted for brevity
}
```

Just as `HtmxRequestHeaderNames` defines constants for HTMX request headers,
`HtmxResponseHeaderNames` defines constants for HTMX response headers.

```csharp
/// <summary>
/// Defines constants for the well-known names of HTMX response headers.
/// </summary>
/// <remarks>
/// For more information, see <see href="https://htmx.org/reference/#response_headers">HTMX Response Headers Reference</see>.
/// </remarks>
public static class HtmxResponseHeaderNames
{
    /// <summary>
    /// The <c>HX-Location</c> header performs a client-side redirect without a full-page reload.
    /// </summary>
    public const string Location = "HX-Location";

    /// <summary>
    /// The <c>HX-Push-Url</c> header pushes a new URL onto the browser's history stack.
    /// </summary>
    public const string PushUrl = "HX-Push-Url";

    /// <summary>
    /// The <c>HX-Redirect</c> header performs a client-side redirect to a new location.
    /// </summary>
    public const string Redirect = "HX-Redirect";

    ...
    // The list of other constants is omitted for brevity
}
```

The most convenient approach is to use one of the `Htmx` extension methods.
Its callback receives an `HtmxResponse`, allowing you to configure response headers in a fluent style:

```csharp
Response.Htmx(h => h
    .TriggerEvent(
        eventName: "process",
        detail: new { Value = ... })
    .StopPolling(ShouldStopPolling));
```

:bulb: The generic overload accepts an additional state parameter to avoid closure allocations:

```csharp
Response.Htmx(
    static (h, stop) => h
        .TriggerEvent(
            eventName: "process",
            detail: new { Value = ... })
        .StopPolling(stop),
    ShouldStopPolling);
```

:bulb: The `Htmx` extension methods are also available for `IActionResult`, allowing you to write:

```csharp
return Json(profile).Htmx(h => h.StopPolling(ShouldStopPolling));
```

In all these examples, headers are set only for an HTMX request. For a regular request,
the callback passed to `Htmx` is not executed, avoiding unnecessary work.

### The declarative way of setting response headers

Some response headers can be set declaratively by applying `HtmxResponseAttribute`
to a controller or action:

```csharp
public class UserController : ControllerBase
{
    [HtmxRequest]
    [HtmxResponse(
        StopPolling = true,
        Reswap = HtmxSwap.OuterHtml)]
    public IActionResult UpdateProfile(UserProfile profile)
    {
        ...
    }
}
```

:bulb: For a more complex swap expression, such as `innerHTML show:#result:top`,
use the `Reswap` overload that accepts a string.

```csharp
/// <summary>
/// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
/// </summary>
/// <param name="value">The swap style to assign to the header.</param>
/// <returns>
/// The current <see cref="HtmxResponse"/> instance.
/// </returns>
public HtmxResponse Reswap(HtmxSwap value);

/// <summary>
/// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
/// </summary>
/// <param name="value">The header value to set.</param>
/// <returns>
/// The current <see cref="HtmxResponse"/> instance.
/// </returns>
public HtmxResponse Reswap(string value);
```

For declarative configuration, `HtmxResponseAttribute` provides the `ReswapExpression` property:

```csharp
/// <summary>
/// Gets or sets the complete <c>HX-Reswap</c> header value, including any swap modifiers.
/// </summary>
[MaybeNull]
public string ReswapExpression { get; set; }

/// <summary>
/// Gets or sets the swap style to specify in the <c>HX-Reswap</c> header.
/// </summary>
public HtmxSwap Reswap { get; set; }
```

Use `ReswapExpression` when the strongly typed `Reswap` property is not flexible enough.

## Tag Helpers

The library provides five tag helpers:

* `HtmxUrlTagHelper`
* `HtmxHeaderTagHelper`
* `HtmxValsTagHelper`
* `HtmxRequestTagHelper`
* `HtmxConfigTagHelper`

To make them available in your project, add the `@addTagHelper` directive to a Razor view:

```razor
@addTagHelper *, Ramstack.HtmxToolkit
```

To make the tag helpers available throughout the application, add this line to
`_ViewImports.cshtml`, which is inherited by Razor views by default.

Import the toolkit namespace there as well if a view refers to toolkit types:

```razor
@using Ramstack.HtmxToolkit
```

### HtmxUrlTagHelper

The `HtmxUrlTagHelper` generates URLs for HTMX requests in much the same way that
the built-in ASP.NET Core tag helpers generate links. In most cases, replace the `asp-` prefix
with `hx-`:

```razor
<div hx-target="this">
    <button hx-area="Sessions"
            hx-controller="Speaker"
            hx-action="Detail"
            hx-route-id="@Model.SpeakerId">Show Info</button>
</div>
```

The following code will be generated:

```html
<div hx-target="this">
    <button hx-get="/Sessions/Speaker/Detail/1">Show Info</button>
</div>
```

If no HTMX method is specified, the tag helper uses `hx-get`. You can select a method with
`hx-get`, `hx-post`, `hx-put`, `hx-delete`, or `hx-patch`.

For instance, in the following example, we use `hx-post`:

```razor
<div hx-target="this">
    <button hx-post
            hx-area="Sessions"
            hx-controller="Speaker"
            hx-action="Detail"
            hx-route-id="@Model.SpeakerId">Show Info</button>
</div>
```

In this case, the following code will be generated:

```html
<div hx-target="this">
    <button hx-post="/Sessions/Speaker/Detail/1">Show Info</button>
</div>
```

Use `hx-page` and `hx-page-handler` to generate a URL for a Razor Page handler:

```razor
<div hx-target="this">
    <button hx-page="/Attendee"
            hx-page-handler="Profile"
            hx-route-attendeeid="1">Attendee Profile</button>
</div>
```

The following code will be generated:

```html
<div hx-target="this">
    <button hx-get="/Attendee?handler=Profile&amp;attendeeid=1">Attendee Profile</button>
</div>
```

The `hx-all-route-data` attribute accepts an `IDictionary<string, string>` containing
additional route values:

```razor
@{
    var parameters = new Dictionary<string, string>
    {
        ["category"] = "science",
        ["pdf"] = "true"
    };
}

<button hx-target="#result"
        hx-action="List"
        hx-all-route-data="parameters">Books</button>
```

The following code will be generated:

```html
<button hx-target="#result" hx-get="/Books/List?category=science&amp;pdf=true">Books</button>
```

The following URL-generation attributes are also available:

* `hx-host`
* `hx-protocol`
* `hx-fragment`

### HtmxHeaderTagHelper

HTMX lets you add custom request headers through a JSON-valued attribute. Because writing and
escaping that JSON manually can be inconvenient, `HtmxHeaderTagHelper` provides a clearer format:

```razor
<div hx-action="Example"
     hx-header-Key-1="Value-1"
     hx-header-Key-2="Value-2">
    Get some HTML and include custom headers in the request
</div>
```

The following code will be generated:

```html
<div hx-get="/Home/Example"
     hx-headers='{"Key-1":"Value-1","Key-2":"Value-2"}'>
    Get some HTML and include custom headers in the request
</div>
```

You can also assign an `IDictionary<string, string>` to `hx-all-headers`:

```razor
@{
    var headers = new Dictionary<string, string>
    {
        ["Key-1"] = "Value-1",
        ["Key-2"] = "Value-2"
    };
}

<div hx-action="Example"
     hx-all-headers="headers">
    Get some HTML and include custom headers in the request
</div>
```

`HtmxHeaderTagHelper` handles JSON serialization and escaping.

### HtmxValsTagHelper

The `HtmxValsTagHelper` adds values that HTMX includes with a request. Use `hx-val-*`
attributes instead of writing JSON manually:

```razor
<button hx-get="/books"
        hx-val-category="science"
        hx-val-format="summary">
    Browse books
</button>
```

The following HTML will be generated:

```html
<button hx-get="/books"
        hx-vals='{"category":"science","format":"summary"}'>
    Browse books
</button>
```

You can also assign an `IDictionary<string, string>` to `hx-all-vals`.

### HtmxRequestTagHelper

`HtmxRequestTagHelper` configures request options for the selected HTMX version.
For HTMX 1.9.x and 2.x, use typed `hx-request-*` attributes instead of writing `hx-request`
JSON manually:

```razor
<button hx-get="/reports"
        hx-request-timeout="5000"
        hx-request-credentials="@HtmxRequestCredentials.Include"
        hx-request-no-headers="false">
    Load report
</button>
```

The following HTML will be generated:

```html
<button hx-get="/reports"
        hx-request='{"timeout":5000,"credentials":true,"noHeaders":false}'>
    Load report
</button>
```

For HTMX 4.x, the tag helper generates `hx-config`. In addition to `timeout` and `credentials`,
HTMX 4.x supports `cache`, `redirect`, `referrer`, `integrity`, and `validate`. The `noHeaders`
option is available only in HTMX 1.9.x and 2.x.

```razor
<button hx-get="/reports"
        hx-request-timeout="5000"
        hx-request-credentials="@HtmxRequestCredentials.Omit"
        hx-request-cache="no-cache"
        hx-request-validate="true">
    Load report
</button>
```

With HTMX 4.x selected, the following HTML will be generated:

```html
<button hx-get="/reports"
        hx-config='{"timeout":5000,"credentials":"omit","cache":"no-cache","validate":true}'>
    Load report
</button>
```

### HtmxConfigTagHelper

HTMX configuration is defined at application startup through `AddHtmxToolkit`.
The version-specific callback exposes only settings supported by the selected HTMX version:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.DefaultSwapStyle = HtmxSwap.OuterHtml;
        config.Timeout = 5000;
        config.GlobalViewTransitions = true;
    });
});
```

Use the tag helper as a marker where the configuration meta element should be rendered:

```html
<head>
    <htmx-config />
</head>
```

The following markup will be generated:

```html
<head>
    <meta name="htmx-config"
          content='{"defaultSwapStyle":"outerHTML","timeout":5000,"globalViewTransitions":true}'
          data-antiforgery-request-token="..."
          data-antiforgery-header-name="RequestVerificationToken"
          data-antiforgery-form-field-name="__RequestVerificationToken" />
</head>
```

The marker can also be written as a `meta` element:

```html
<meta htmx-config />
```

HTMX 2.x is selected by default. Use `UseHtmxV1`, `UseHtmxV2`, or `UseHtmxV4` to select a
version explicitly. Each configuration type follows the names used by that HTMX version, so HTMX 1.9.x
and 2.x expose `DefaultSwapStyle` and `Timeout`, while HTMX 4.x exposes `DefaultSwap` and
`DefaultTimeout`. Selecting different versions in the same configuration throws an exception.

HTMX 4.x is currently in beta. To target it, select it explicitly and use its version-specific
settings:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV4(config =>
    {
        config.DefaultSwap = HtmxSwap.OuterHtml;
        config.DefaultTimeout = 5000;
        config.Transitions = true;
        config.NoSwap = ["204", "304", "4xx", "5xx"];
    });
});
```

The configured values remain available through dependency injection:

```csharp
public sealed class ConfigurationInspector(IOptions<HtmxToolkitOptions> options)
{
    public HtmxV2Config HtmxConfig =>
        options.Value.GetHtmxConfig<HtmxV2Config>();
}
```

#### Response Handling Configuration

HTMX 2.x introduces the [`responseHandling`](https://htmx.org/docs/#response-handling) configuration option,
allowing you to define how HTMX should handle responses based on HTTP status codes. Rules are
configured in order through `HtmxV2Config`:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.ResponseHandling =
        [
            new() { Code = "204", Swap = false },
            new() { Code = "[23]..", Swap = true },
            new() { Code = "422", Swap = true },
            new() { Code = "[45]..", Swap = false, Error = true },
            new() { Code = "...", Swap = true }
        ];
    });
});
```

HTMX 4.x removes `responseHandling`. To retain HTMX 2.x behavior that does not swap error
responses, configure `NoSwap` as shown in the HTMX 4.x example above.

## Toolkit Script

The toolkit script provides antiforgery support and HTMX compatibility behavior.
Antiforgery metadata generation is enabled by default. Include the script to ensure the
token is added to non-GET request form parameters or headers and refreshed in a timely manner.

Sending the token does not enable server-side validation by itself. Configure antiforgery
validation for the corresponding ASP.NET Core endpoints as appropriate.

To disable antiforgery metadata generation—for example, when the application handles
antiforgery separately or does not issue unsafe HTMX requests—set the option to `false`:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = false;
});
```

You can embed the minified script directly in a Razor view:

```razor
<script>
  @Html.HtmxToolkitScript()
</script>
```

Pass `true` to embed the debug version instead:

```razor
<script>
  @Html.HtmxToolkitScript(debug: true)
</script>
```

The minified version is used by default and is less than 1 KB.

The method returns a cached `HtmlString`, avoiding repeated conversions and allocations.

Alternatively, register an endpoint that serves the script:

```csharp
app.UseAuthorization();
...
app.MapHtmxToolkitScript();
app.MapControllers();
```

By default, the registered path is mapped to `/htmxtoolkit/[sha1-hash]`,
where **[sha1-hash]** represents a precomputed hash of the script content.
The hash changes whenever the script changes, providing automatic cache invalidation.

To use a custom path, pass it to `MapHtmxToolkitScript`:

```csharp
app.MapHtmxToolkitScript("/my-path");
```

Then include the mapped script in a Razor view:

```razor
<script src="@Html.HtmxToolkitScriptPath()"></script>
```

Pass `true` to generate a path with the `?debug` query string and load the debug version:

```razor
<script src="@Html.HtmxToolkitScriptPath(debug: true)"></script>
```

Without the `debug` argument, the endpoint serves the minified version.

## Supported Versions

The following .NET and HTMX versions are supported:

|      | Version                          |
|------|----------------------------------|
| .NET | 6, 7, 8, 9, 10, 11               |
| HTMX | 1.9.x, 2.x (default), 4.x (beta) |

## Contributions

Bug reports and contributions are welcome.

## License

This package is released as open source under the **MIT License**.
See the [LICENSE](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/LICENSE) file for more details.
