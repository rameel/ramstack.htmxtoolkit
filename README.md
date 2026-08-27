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
  * [TagHelpers](#taghelpers)
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

Install `Ramstack.HtmxToolkit` [NuGet package](https://www.nuget.org/packages/Ramstack.HtmxToolkit/) to your project,
use the following command

```console
dotnet add package Ramstack.HtmxToolkit
```

Register the toolkit and select the HTMX version used by the application:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = true;
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

The library provides a set of classes for working with `HttpRequest`.
First off, there's the `HttpRequestExtensions` class.

```csharp
/// <summary>
/// Provides extension methods for the <see cref="HttpRequest"/> class.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Determines whether the specified HTTP request is htmx request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <c>true</c> if the specified HTTP request is htmx request; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request);

    /// <summary>
    /// Determines whether the specified HTTP request is htmx request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this methods returns, contains the <see cref="HtmxRequestHeaders"/>
    /// that provides well-known htmx headers.</param>
    /// <returns>
    /// <c>true</c> if the specified HTTP request is htmx request; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHtmxRequest(this HttpRequest request, out HtmxRequestHeaders headers);

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// <c>true</c> if the specified HTTP request is boosted; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request);

    /// <summary>
    /// Determines whether the specified HTTP request was made using AJAX
    /// instead of a normal navigation.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="headers">When this methods returns, contains the <see cref="HtmxRequestHeaders"/>
    /// that provides well-known htmx headers.</param>
    /// <returns>
    /// <c>true</c> if the specified HTTP request is boosted; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHtmxBoosted(this HttpRequest request, out HtmxRequestHeaders headers);

    /// <summary>
    /// Returns the <see cref="HtmxRequestHeaders"/> that provides well-known htmx headers.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>
    /// The <see cref="HtmxRequestHeaders"/>.
    /// </returns>
    public static HtmxRequestHeaders GetHtmxHeaders(this HttpRequest request);
}
```

The `IsHtmxRequest` method allows you to determine whether the current request is initiated by HTMX.

```csharp
HttpContext.Request.IsHtmxRequest()
```

And thus, you can define different scenarios depending on the result, for example:

```csharp
if (Request.IsHtmxRequest())
    return PartialView();

return View();
```

The overloads of these methods provide access to strongly typed headers set by HTMX:

```csharp
if (Request.IsHtmxRequest(out var headers))
{
    if (headers.HistoryRestoreRequest)
    {
        ...
    }
}
```

In addition, access to strongly typed headers can be obtained by calling `GetHtmxHeaders`:

```csharp
var headers = Request.GetHtmxHeaders();
```

The full list of headers is presented below:

```csharp
/// <summary>
/// Represents strongly typed HTMX request headers.
/// </summary>
public readonly struct HtmxRequestHeaders
{
    /// <summary>
    /// Gets a value indicating whether the request
    /// was made using AJAX instead of a normal navigation.
    /// </summary>
    public bool Boosted { get; }

    /// <summary>
    /// Gets the current URL of the browser.
    /// </summary>
    public string? CurrentUrl { get; }

    /// <summary>
    /// Gets a value indicating whether the request
    /// is for history restoration after a miss in the local history cache.
    /// </summary>
    public bool HistoryRestoreRequest { get; }

    /// <summary>
    /// Gets the user response to an hx-prompt on the client.
    /// </summary>
    public string? Prompt { get; }

    /// <summary>
    /// Gets a value indicating whether the current request is htmx request.
    /// </summary>
    public bool Request { get; }

    /// <summary>
    /// Gets the ID of the target element if it exists.
    /// </summary>
    public string? Target { get; }

    /// <summary>
    /// Gets the name of the triggered element if it exists.
    /// </summary>
    public string? TriggerName { get; }

    /// <summary>
    /// Gets the ID of the triggered element if it exists
    /// as indicated by the <c>HX-Trigger</c> header.
    /// </summary>
    public string? Trigger { get; }
}
```

Example of usage:

```csharp
if (Request.GetHtmxHeaders().HistoryRestoreRequests)
{
    ...
}
```

The library also provides a set of predefined string constants for request headers,
so you don't have to remember them each time and risk making mistakes in spelling.
You can find them in the `HtmxRequestHeaderNames` class.

```csharp
/// <summary>
/// Defines constants for the well-known names of htmx request headers.
/// For more information, see https://htmx.org/reference/#request_headers
/// </summary>
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

To simplify some manual checks, the library provides an ASP.NET Core result filter
that can be applied to an action controller, page handler, or the entire controller:

```csharp
public class UserController : ControlleBase
{
    [HtmxRequest]
    public IActionResult UpdateProfile(UserProfile profile)
    {
        ...
    }
}
```

If you are processing boosted requests in a special way, add the `Boosted` parameter.

```csharp
public class UserController : ControlleBase
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

For working with response headers, the library also provides a set of classes.
The first one is the `HttpResponseExtension` class with extension methods:

```csharp
/// <summary>
/// Provides extension methods for the <see cref="HttpResponse"/> class.
/// </summary>
public static class HttpResponseExtensions
{
    /// <summary>
    /// Returns the <see cref="HtmxResponseHeaders"/> that provides well-known htmx headers.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>
    /// The <see cref="HtmxResponseHeaders"/>.
    /// </returns>
    public static HtmxResponseHeaders GetHtmxHeaders(this HttpResponse response);

    /// <summary>
    /// Configures the htmx response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The function to configure the htmx response headers.</param>
    public static void Htmx(this HttpResponse response, Action<HtmxResponse> configure);

    /// <summary>
    /// Configures the htmx response headers.
    /// </summary>
    /// <param name="response">The HTTP response to configure.</param>
    /// <param name="configure">The function to configure the htmx response headers.</param>
    /// <param name="state">The value to pass to the <paramref name="configure"/>.</param>
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
    /// Gets or sets the <c>HX-Location</c> header to a client-side redirect
    /// that does not do a full page reload.
    /// </summary>
    [MaybeNull]
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the <c>HX-Push-Url</c> header to push a new URL into the history stack.
    /// </summary>
    [MaybeNull]
    public string PushUrl { get; set; }

    ...
    // The remaining properties are omitted for brevity
}
```

Just like `HtmxRequestHeaderNames`, which consists of predefined string constants for HTMX request headers,
there is a corresponding `HtmxResponseHeaderNames` class containing
a list of string constants for HTMX response headers.

```csharp
/// <summary>
/// Defines constants for the well-known names of htmx response headers.
/// For more information, see https://htmx.org/reference/#response_headers
/// </summary>
public static class HtmxResponseHeaderNames
{
    /// <summary>
    /// The <c>HX-Location</c> header is used to a client-side redirect that does not do a full page reload.
    /// </summary>
    public const string Location = "HX-Location";

    /// <summary>
    /// The <c>HX-Push-Url</c> header is used to push a new URL into the history stack.
    /// </summary>
    public const string PushUrl = "HX-Push-Url";

    /// <summary>
    /// The <c>HX-Redirect</c> header is used to client-side redirect to a new location.
    /// </summary>
    public const string Redirect = "HX-Redirect";

    ...
    // The list of other constants is omitted for brevity
}
```

However, the most convenient and efficient way is by using one of the provided `Htmx` methods
with a callback that accepts `HtmxResponse`, allowing you to specify response headers
in a fluent style:

```csharp
Response.Htmx(h => h
    .TriggerEvent(
        eventName: "process",
        detail: new { Value = ... })
    .StopPolling(ShouldStopPolling));
```

:bulb: The second Htmx method accepts an additional parameter to avoid unnecessary allocations due to closures:

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

In both cases, the headers will be set only in the case of an `htmx` request.
In the case of a regular request, the callback passed to the `Htmx(this)` method will not be executed,
which allows avoiding unnecessary work.

### The declarative way of setting response headers

Some of the response headers can be set declaratively using the `HtmxResponseAttribute`,
which is applied to the controller, action, or page.

```csharp
public class UserController : ControlleBase
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

:bulb: If a more complex expression is needed for `swap`, for example, `innerHTML show:#result:top`,
you can use the `Reswap` method in the `HtmxResponse` class, which accepts a string.

```csharp
/// <summary>
/// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
/// </summary>
/// <param name="value">The header value to set.</param>
/// <returns>
/// The current <see cref="HtmxResponse"/> instance.
/// </returns>
public HtmxResponse Reswap(string value);

/// <summary>
/// Sets the <c>HX-Reswap</c> header to specify how the response will be swapped.
/// </summary>
/// <param name="value">The header value to set.</param>
/// <returns>
/// The current <see cref="HtmxResponse"/> instance.
/// </returns>
public HtmxResponse Reswap(HtmxSwap value);
```

And for the `HtmxResponseAttribute`, there is the `ReswapExpression` property.

```csharp
/// <summary>
/// Gets or sets the <c>HX-Reswap</c> header that allows to specify how the response will be swapped.
/// </summary>
[MaybeNull]
public string ReswapExpression { get; set; }

/// <summary>
/// Gets or sets the <c>HX-Reswap</c> header that allows to specify how the response will be swapped.
/// </summary>
public HtmxSwap Reswap { get; set; }
```

allowing you to flexibly configure the `swap` header you need.

## TagHelpers

The library provides 4 tag helpers:

* `HtmxUrlTagHelper`
* `HtmxHeaderTagHelper`
* `HtmxConfigTagHelper`
* `HtmxRequestTagHelper`

To make them available in your project, add the `@addTagHelper` directive in the Razor view.

```razor
@addTagHelper *, Ramstack.HtmxToolkit
```

To make the tag helpers available globally for the entire application, you should add this line
to the `_ViewImports.cshtml` file, which is inherited by all view files by default.

### HtmxUrlTagHelper

The `HtmxUrlTagHelper` allows generating links for HTMX methods similar to how it's done in ASP.NET Core
for generating links, just replace the `asp-` prefix with the `hx-` prefix.

```html
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

By default, if no HTMX method is specified, `hx-get` is used. To specify a particular method,
you can choose one from the following attributes: `hx-get`, `hx-post`, `hx-put`, `hx-delete`, or `hx-patch`.

For instance, in the following example, we use `hx-post`:
```html
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

The following attributes are also available for obtaining links to a page and a page handler:

```html
<div hx-target="this">
    <button hx-page="/Attendee"
            hx-page-handler="Profile"
            hx-route-attendeeid="1">Attendee Profile</button>
</div>
```

The following code will be generated:

```html
<div hx-target="this">
    <button hx-get="/Attendee?attendeeid=1&handler=Profile">Attendee Profile</button>
</div>
```

Also, the `hx-all-route-data` attribute is available, which accepts a dictionary where
the key is the parameter name, and the value is the parameter value. In the example below,
a dictionary with specific parameters is created, which is then used as the value
of the `hx-all-route-data` attribute.

```html
@{
    var parameters = new {
        category = "science",
        pdf = true
    };
}

<button hx-target="#result"
        hx-action="List"
        hx-all-route-data="parameters">Books</a>
```

The following code will be generated:

```html
<button hx-target="#result" hx-get="/Books/List?category=science&pdf=true">Books</a>
```

In addition to the examples mentioned, the following properties are also available:
* `hx-host`
* `hx-protocol`
* `hx-fragment`

### HtmxHeaderTagHelper

The `htmx` library allows adding custom headers that will be submitted with an AJAX request.
However, since JSON format should be used for this, writing it out manually is not always convenient,
especially considering the need to escape special characters. Fortunately, the library provides
the `HtmxHeaderTagHelper` class, which takes care of this and allows specifying headers
in a clearer and more readable format.

```html
 <div hx-action="example"
      hx-header-Key-1="Value-1"
      hx-header-Key-2="Value-2">
      Get Some HTML, Including A Custom Header in the Request
  </div>
```

The following code will be generated:

```html
 <div hx-get="/home/example"
      hx-headers='{"Key-1":"Value-1","Key-2":"Value-2"}'>
      Get Some HTML, Including A Custom Header in the Request
  </div>
```

Also, if you have a dictionary with the headers you need,
you can assign them to the `hx-all-headers` attribute:

```html
 <div hx-action="example"
      hx-all-headers="headers">
      Get Some HTML, Including A Custom Header in the Request
  </div>
```

The `HtmxHeaderTagHelper` will take care of all the remaining work regarding JSON serialization and escaping.

### HtmxValsTagHelper

The `HtmxValsTagHelper` adds values that HTMX includes with a request. Use `hx-val-*`
attributes instead of writing JSON manually:

```html
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

You can also provide the values as a dictionary with the `hx-all-vals` attribute.

### HtmxRequestTagHelper

The `HtmxRequestTagHelper` configures the htmx request options supported by HTMX 1.9.x and 2.x.
Use the typed `hx-request-*` attributes instead of writing JSON manually:

```html
<button hx-get="/reports"
        hx-request-timeout="5000"
        hx-request-credentials="true"
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

### HtmxConfigTagHelper

HTMX configuration is defined at application startup through `AddHtmxToolkit`.
The version-specific callback exposes only settings supported by the selected HTMX version:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = true;
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
allowing you to define how htmx should handle responses based on HTTP status codes. Rules are
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
If you have enabled **Antiforgery** token generation in the configuration
(`IncludeAntiforgeryToken = true`), include it to ensure the token is present in form
parameters or headers and refreshed in a timely manner.

To do this, you can directly include the contents of the script file on the page:

```html
<script>
  @Html.HtmxToolkitScript()
</script>
```

Alternatively, to retrieve the debug version of the script,
you can pass the debug parameter with a value of `true`:

```html
<script>
  @Html.HtmxToolkitScript(debug: true)
</script>
```

The `debug` parameter determines which version will be included. By default, the minimized version
of the script will be returned, which weighs very little and takes up approximately 520 bytes.

The method returns a pre-initialized `HtmlString` with the script content,
so there will be no unnecessary conversions and allocations every time it's used.

Alternatively, you can register the corresponding endpoint for the script by calling:

```csharp
app.UseAuthorization();
...
app.MapHtmxToolkitScript();
app.MapControllers();
```

By default, the registered path is mapped to `/htmxtoolkit/[sha1-hash]`,
where **[sha1-hash]** represents a precalculated hash of the script content.
This approach eliminates the need to worry about cache invalidating
when updating the script in the future as the hash automatically changes
when the script content is modified.

If you want to change the path to your own, specify this path in the parameter.

```csharp
app.MapHtmxToolkitScript("/my-path");
```

Now, include it on the page.

```html
<script src="@Html.HtmxToolkitScriptPath()"></script>
```

Alternatively, to retrieve the debug version of the script, you can pass the `debug` parameter
with a value of `true`, which instructs to include a query parameter `?debug` in the path.
The presence of this parameter determines the loading of the debug version:

```html
<script src="@Html.HtmxToolkitScriptPath(debug: true)"></script>
```

The `debug` parameter determines whether to load the minimized version (used by default)
or the debug version of the script.

## Supported Versions

All releases in the following HTMX version lines are supported:

|      | Version                          |
|------|----------------------------------|
| .NET | 6, 7, 8, 9, 10, 11               |
| HTMX | 1.9.x, 2.x (default), 4.x (beta) |

## Contributions

Bug reports and contributions are welcome.

## License
This package is released as open source under the **MIT License**.
See the [LICENSE](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/LICENSE) file for more details.
