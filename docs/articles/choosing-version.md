# Choose an HTMX version

HtmxToolkit generates version-sensitive configuration, request options, request-header access, and compatibility behavior.
Configure the same major HTMX version that the browser loads.

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

| Behavior | HTMX 1.x | HTMX 2.x | HTMX 4.x |
|---|---|---|---|
| Default when omitted | No | Yes | No |
| Global config type | `HtmxV1Config` | `HtmxV2Config` | `HtmxV4Config` |
| Per-request attribute | `hx-request` | `hx-request` | `hx-config` |
| Triggering element header | `HX-Trigger` | `HX-Trigger` | `HX-Source` |
| Expected response kind | Not reported | Not reported | `HX-Request-Type` |
| Morph swap support | Toolkit compatibility extension | Toolkit compatibility extension | Native |

See [Version compatibility](version-compatibility.md) for the complete application-facing differences.

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

Only assigned properties are emitted. A `null` property leaves the corresponding HTMX default unchanged.

## Decide which version to use

For an existing application, select the version it already loads and review [Version compatibility](version-compatibility.md) before upgrading.

For a new application, choose the release required by its HTMX attributes and extensions. HtmxToolkit's default is HTMX 2.x;
choosing V4 enables the V4 request contract and its native morphing and Fetch API options.

Continue with [Application configuration](configuration.md), then use the dedicated [HTMX 1.x](configuration-v1.md),
[HTMX 2.x](configuration-v2.md), or [HTMX 4.x](configuration-v4.md) reference guide.
