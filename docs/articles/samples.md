# Run the examples

The guides build small examples step by step. The repository also includes a demo with separate pages for
request detection, response headers, Tag Helpers, events, polling, and antiforgery.

## Run the demo

Install the .NET 10 SDK or later, then run:

```console
git clone https://github.com/rameel/ramstack.htmxtoolkit.git
cd ramstack.htmxtoolkit
dotnet run --project samples/Ramstack.HtmxToolkit.Demo --no-launch-profile -- --urls http://localhost:5000 --environment Development
```

Open <http://localhost:5000>. This command uses HTTP for local development, so a development HTTPS certificate
is not needed. The demo loads HTMX from a CDN and requires a network connection for that script.

The demo currently selects HTMX 4.x. The introductory tutorials use HTMX 2.x, the toolkit default.
To try another supported version, change both `UseHtmxV4(...)` in `Program.cs` and the HTMX script in
`Pages/Shared/_Layout.cshtml`. Review [Version compatibility](version-compatibility.md) before changing
version-specific options such as `NoSwap`.

## Find an example

Each Razor Page has a `.cshtml` file for the browser markup and a `.cshtml.cs` file for its handlers.
Start with the linked page, then open its code-behind file in the same directory.

| Task | Demo page | Related guide |
|---|---|---|
| Generate a page-handler URL | [UrlTagHelper](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/UrlTagHelper.cshtml) | [Tag Helpers](tag-helpers.md#generate-request-urls) |
| Send route values | [RouteData](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/RouteData.cshtml) | [Routes and dictionaries](tag-helpers.md#named-routes-and-route-dictionaries) |
| Send values and custom headers | [HtmxVals](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/HtmxVals.cshtml), [Headers](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/Headers.cshtml) | [Tag Helpers](tag-helpers.md#send-additional-values) |
| Set request options | [RequestOptions](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/RequestOptions.cshtml) | [Configure one request](tag-helpers.md#configure-one-request) |
| Detect HTMX and boosted requests | [HtmxRequest](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/HtmxRequest.cshtml), [Boosted](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/Boosted.cshtml) | [Requests](requests.md) |
| Use MVC attributes | [InventoryController](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Controllers/InventoryController.cs) | [MVC action selection](requests.md#select-mvc-actions-declaratively) |
| Change the response target | [FluentResponse](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/FluentResponse.cshtml) | [Responses](responses.md) |
| Trigger another update | [ServerEvents](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/ServerEvents.cshtml) | [Forms and events tutorial](forms-and-validation.md) |
| Poll until work completes | [Polling](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/Polling.cshtml) | [Polling recipe](recipes.md#poll-a-background-operation) |
| Submit a protected form | [Antiforgery](https://github.com/rameel/ramstack.htmxtoolkit/blob/main/samples/Ramstack.HtmxToolkit.Demo/Pages/Examples/Antiforgery.cshtml) | [Antiforgery](antiforgery.md) |

## Inspect the browser behavior

Keep the Network panel open while using each example. Compare the rendered `hx-*` attributes with the Razor
source, then inspect the request URL, form values, and `HX-*` response headers.

These are small demonstrations, not production data flows. Add your own persistence, authorization,
and error handling when adapting them. For a complete introduction with all files shown in order,
start with [Getting started](getting-started.md).
