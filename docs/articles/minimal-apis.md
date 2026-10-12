# Use HtmxToolkit with Minimal APIs

Minimal APIs can use `IsHtmxRequest()`, `GetHtmxHeaders()`, and `Response.Htmx(...)` directly.
They do not process Razor Tag Helpers or use the MVC action attributes. HtmxToolkit does not provide an HTML renderer.

The following .NET 10 example is a complete application. It returns a small HTML page and accepts a protected POST.
The HTML is kept in a string only to make the example runnable without a view engine.

## Create the application

```console
dotnet new web -n HtmxMinimal -f net10.0
cd HtmxMinimal
dotnet add package Ramstack.HtmxToolkit
```

## Add the page and endpoint

Replace `Program.cs` with:

```csharp
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Antiforgery;
using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHtmxToolkit();
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

var app = builder.Build();
app.UseStaticFiles();

app.MapGet("/", (HttpContext context, IAntiforgery antiforgery) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);
    var token = HtmlEncoder.Default.Encode(tokens.RequestToken!);
    var header = HtmlEncoder.Default.Encode(tokens.HeaderName!);
    var field = HtmlEncoder.Default.Encode(tokens.FormFieldName);
    var pathBase = HtmlEncoder.Default.Encode(context.Request.PathBase.Value ?? "");

    return Results.Content($$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="utf-8">
            <title>HTMX with Minimal APIs</title>
            <meta name="htmx-config" content='{}'
                  data-antiforgery-request-token="{{token}}"
                  data-antiforgery-header-name="{{header}}"
                  data-antiforgery-form-field-name="{{field}}">
        </head>
        <body>
            <h1>Say hello</h1>
            <form hx-post="{{pathBase}}/greeting" hx-target="#result">
                <label>Name <input name="name" required></label>
                <button type="submit">Send</button>
            </form>
            <div id="result" aria-live="polite"></div>
            <script src="https://cdn.jsdelivr.net/npm/htmx.org@2.0.11/dist/htmx.min.js"></script>
            <script src="{{pathBase}}/_content/Ramstack.HtmxToolkit/htmx-toolkit.min.js"></script>
        </body>
        </html>
        """, "text/html");
});

app.MapPost("/greeting", async (HttpContext context, IAntiforgery antiforgery) =>
{
    if (!await antiforgery.IsRequestValidAsync(context))
        return Results.BadRequest("Invalid antiforgery token.");

    var form = await context.Request.ReadFormAsync();
    var name = form["name"].ToString().Trim();
    if (name.Length is 0 or > 40)
        return Results.Content("<p>Enter a name between 1 and 40 characters.</p>", "text/html");

    context.Response.Htmx(htmx => htmx.TriggerEvent("greeting-created"));

    return Results.Content($"<p>Hello, {HtmlEncoder.Default.Encode(name)}!</p>", "text/html");
});

app.Run();
```

The GET endpoint creates the same antiforgery metadata that `<htmx-config />` normally renders in Razor.
The Toolkit script reads it and adds the token to the POST. The POST explicitly validates the token before
reading and processing the input. It also encodes the submitted name before including it in HTML.

The validation message uses HTTP 200 so the default HTMX 2.x configuration displays it. An invalid
antiforgery token instead returns HTTP 400 and stops the operation.

`Response.Htmx(...)` adds the `greeting-created` event only for an HTMX request. You can observe its
`HX-Trigger` header in the Network panel and add a listener as shown in [the events tutorial](forms-and-validation.md).

This example validates manually and does not bind a `[FromForm]` parameter. Applications using ASP.NET Core's
form-binding antiforgery middleware should follow that framework's registration and middleware requirements;
see [Antiforgery](antiforgery.md#minimal-apis).

## Run it

```console
dotnet run --no-launch-profile -- --urls http://localhost:5082 --environment Development
```

Open <http://localhost:5082>, enter a name, and select **Send**. The greeting should appear below the form.
Inspect the POST for `HX-Request: true` and `RequestVerificationToken`. A POST without a valid token
must return 400.

For a larger application, move HTML rendering into templates or Razor views. If Minimal API endpoints serve
fragments for an existing Razor Pages or MVC frontend, keep configuration and token metadata in that frontend's
layout instead of generating another document here.
