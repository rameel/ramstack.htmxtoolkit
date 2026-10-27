---
_appTitle: Ramstack.HtmxToolkit
---

# Ramstack.HtmxToolkit

Strongly typed HTMX integration for ASP.NET Core MVC, Razor Pages, and Minimal APIs.

Read HTMX request headers through C# properties, return response instructions with a fluent API, and generate
request URLs through ASP.NET Core routing. The toolkit also provides MVC action attributes, version-specific
HTMX configuration, and automatic antiforgery token delivery through the Toolkit script.

## Get started

Install the NuGet package:

```console
dotnet add package Ramstack.HtmxToolkit
```

Choose your starting point:

| What you need                                    | Start here                                                                                                          |
|--------------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| Understand how HTMX and the toolkit fit together | [Overview](articles/overview.md)                                                                                    |
| Build your first interaction                     | [Getting started](articles/getting-started.md), [MVC](articles/mvc.md), or [Minimal APIs](articles/minimal-apis.md) |
| Follow the course or find a task guide           | [Guides](articles/index.md)                                                                                         |
| Look up a type or method                         | [API reference](api/index.md)                                                                                       |
| Diagnose an integration problem                  | [Troubleshooting](articles/troubleshooting.md)                                                                      |
| Upgrade from HtmxToolkit 1.x                     | [Migrate from 1.x to 2.0](articles/migrate-to-v2.md)                                                                |

## Supported environments

The package targets .NET 6 and runs on .NET 6 or later.
It supports HTMX 1.9.x, HTMX 2.x, and HTMX 4.x; HTMX 2.x is the default target.

> [!IMPORTANT]
> HtmxToolkit does not bundle HTMX. Add the matching HTMX client release to the application separately.
