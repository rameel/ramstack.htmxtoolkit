using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

namespace Ramstack.HtmxToolkit.Tests.Configuration;

[TestFixture]
public class HtmxToolkitOptionsTests
{
    [Test]
    public void HtmxConfig_WithoutSelectedVersion_Throws()
    {
        var options = new HtmxToolkitOptions();

        Assert.That(
            () => options.HtmxConfig,
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("The HTMX version is not selected. Call UseHtmxV1, UseHtmxV2, or UseHtmxV4 in AddHtmxToolkit."));
    }

    [Test]
    public void UseHtmxV1_StoresInspectableConfig()
    {
        var options = new HtmxToolkitOptions();

        options.UseHtmxV1(config => config.Timeout = 2500);

        var config = options.GetHtmxConfig<HtmxV1Config>();
        Assert.Multiple(() =>
        {
            Assert.That(options.TargetVersion, Is.EqualTo(HtmxTargetVersion.V1));
            Assert.That(options.HtmxConfig, Is.SameAs(config));
            Assert.That(config.Timeout, Is.EqualTo(2500));
        });
    }

    [Test]
    public void UseHtmxV4_StoresInspectableConfig()
    {
        var options = new HtmxToolkitOptions();

        options.UseHtmxV4(config => config.DefaultTimeout = 5000);

        var config = options.GetHtmxConfig<HtmxV4Config>();
        Assert.Multiple(() =>
        {
            Assert.That(options.TargetVersion, Is.EqualTo(HtmxTargetVersion.V4));
            Assert.That(options.HtmxConfig, Is.SameAs(config));
            Assert.That(config.DefaultTimeout, Is.EqualTo(5000));
        });
    }

    [Test]
    public void UseHtmxV2_RepeatedCallsComposeOnSameConfig()
    {
        var options = new HtmxToolkitOptions();

        options.UseHtmxV2(config => config.Timeout = 1000);
        var first = options.HtmxConfig;
        options.UseHtmxV2(config => config.DefaultSettleDelay = 20);

        var config = options.GetHtmxConfig<HtmxV2Config>();
        Assert.Multiple(() =>
        {
            Assert.That(config, Is.SameAs(first));
            Assert.That(config.Timeout, Is.EqualTo(1000));
            Assert.That(config.DefaultSettleDelay, Is.EqualTo(20));
        });
    }

    [Test]
    public void SelectingDifferentVersions_Throws()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV1();

        Assert.That(
            () => options.UseHtmxV4(),
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("HTMX has already been configured for version V1. Cannot reconfigure to V4."));
    }

    [Test]
    public void GetHtmxConfig_ForDifferentVersion_Throws()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV4();

        Assert.That(
            () => options.GetHtmxConfig<HtmxV2Config>(),
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("HTMX configuration version 'V4' does not match the requested configuration type.")
        );
    }

    [Test]
    public void AddHtmxToolkit_DependencyInjection_ExposesConfiguredOptions()
    {
        var services = new ServiceCollection();
        services.AddHtmxToolkit(options =>
        {
            options.IncludeAntiforgeryToken = true;
            options.UseHtmxV4(config => config.DefaultTimeout = 5000);
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HtmxToolkitOptions>>().Value;
        var config = options.GetHtmxConfig<HtmxV4Config>();

        Assert.Multiple(() =>
        {
            Assert.That(options.IncludeAntiforgeryToken, Is.True);
            Assert.That(config.DefaultTimeout, Is.EqualTo(5000));
        });
    }

    [Test]
    public void AddHtmxToolkit_WithoutSelectedVersion_FailsValidation()
    {
        var services = new ServiceCollection();
        services.AddHtmxToolkit(options => options.IncludeAntiforgeryToken = false);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HtmxToolkitOptions>>();

        Assert.That(
            () => options.Value,
            Throws.TypeOf<OptionsValidationException>()
                  .With.Message.EqualTo("The HTMX version is not selected. Call UseHtmxV1, UseHtmxV2, or UseHtmxV4 in AddHtmxToolkit."));
    }
}
