# Choose an HTMX version

HtmxToolkit adapts configuration, request options, and response event headers to the selected HTMX version.
Select the same major version that the browser loads.

HTMX 2.x is selected when no version is specified.

## Select a version

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV1();
});
```

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2();
});
```

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV4();
});
```

The version can be selected only once. Repeated configuration for the same version updates the same version-specific object;
trying to select a different version causes startup validation to fail.

> [!IMPORTANT]
> Selecting a target version does not download or serve HTMX. The `<script>` loaded by the application must match the configured major version.

## What the selection changes

| Behavior                  | HTMX 1.9.x                                | HTMX 2.x                                  | HTMX 4.x          |
|---------------------------|-------------------------------------------|-------------------------------------------|-------------------|
| Default when omitted      | No                                        | Yes                                       | No                |
| Global config type        | `HtmxV1Config`                            | `HtmxV2Config`                            | `HtmxV4Config`    |
| Per-request attribute     | `hx-request`                              | `hx-request`                              | `hx-config`       |
| Triggering element header | `HX-Trigger`                              | `HX-Trigger`                              | `HX-Source`       |
| Expected response kind    | `HX-Request-Type` from the Toolkit script | `HX-Request-Type` from the Toolkit script | `HX-Request-Type` |
| Morph swap support        | Toolkit compatibility extension           | Toolkit compatibility extension           | Native            |

See [Version compatibility](version-compatibility.md) for the differences that affect application code.

## Configure while selecting

Each selection method accepts a version-specific configuration delegate:

```csharp
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV2(config =>
    {
        config.HistoryCacheSize = 20;
        config.Timeout = 10_000;
        config.GlobalViewTransitions = true;
    });
});
```

Only settings you assign a value are included in the generated configuration. A `null` property leaves the HTMX default unchanged.

## Decide which version to use

For an existing application, select the version it already loads and review [Version compatibility](version-compatibility.md) before upgrading.

For a new application with no version-specific requirements, start with HTMX 2.x to follow the introductory
tutorials without changes. It is HtmxToolkit's default target. Choose V4 when you want its native morphing
or Fetch API options, and check that the extensions you plan to use support it.

Continue with [Application configuration](configuration.md), then use the dedicated [HTMX 1.9.x](configuration-v1.md),
[HTMX 2.x](configuration-v2.md), or [HTMX 4.x](configuration-v4.md) reference guide.
