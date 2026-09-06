---
_appTitle: Ramstack.HtmxToolkit
---

# Ramstack.HtmxToolkit

Strongly typed HTMX integration for ASP.NET Core MVC, Razor Pages, and Minimal APIs.

Ramstack.HtmxToolkit provides strongly typed request and response headers, MVC action filters, Razor Tag Helpers,
version-specific HTMX configuration, and automatic antiforgery support.

## Get started

Install the NuGet package:

```console
dotnet add package Ramstack.HtmxToolkit
```

Then read the [getting started guide](articles/getting-started.md),
or browse the generated [API reference](api/index.md).

## Explore the guides

- [Understand the integration](articles/overview.md)
- [Choose HTMX 1.x, 2.x, or 4.x](articles/choosing-version.md)
- [Return full pages and fragments](articles/pages-and-fragments.md)
- [Configure requests and responses](articles/requests.md)
- [Protect POST requests](articles/antiforgery.md)
- [Solve common problems](articles/troubleshooting.md)

## Supported environments

The package targets .NET 6 and runs on .NET 6 or later.
It supports HTMX 1.9.x, HTMX 2.x, and HTMX 4.x; HTMX 2.x is the default target.

> [!IMPORTANT]
> HtmxToolkit does not bundle HTMX. Add the matching HTMX client release to the application separately.
