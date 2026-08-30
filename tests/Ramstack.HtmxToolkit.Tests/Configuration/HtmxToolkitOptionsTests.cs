using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

namespace Ramstack.HtmxToolkit.Tests.Configuration;

[TestFixture]
public class HtmxToolkitOptionsTests
{
    [Test]
    public void DefaultsToHtmx2()
    {
        var options = new HtmxToolkitOptions();

        Assert.Multiple(() =>
        {
            Assert.That(options.TargetVersion, Is.EqualTo(HtmxTargetVersion.V2));
            Assert.That(options.HtmxConfig, Is.TypeOf<HtmxV2Config>());
        });
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
    public void UseHtmxV4_AfterReadingDefault_ThrowsInvalidOperationException()
    {
        var options = new HtmxToolkitOptions();
        var defaultHtmxConfig = options.HtmxConfig;

        Assert.Multiple(() =>
        {
            Assert.That(defaultHtmxConfig, Is.TypeOf<HtmxV2Config>());
            Assert.That(
                () => options.UseHtmxV4(config => config.DefaultTimeout = 5000),
                Throws.TypeOf<InvalidOperationException>()
                      .With.Message.EqualTo("HTMX has already been configured for version V2. Cannot reconfigure to V4.")
            );
        });
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
    public void AddHtmxToolkit_WithoutConfiguration_ExposesDefaultOptions()
    {
        var services = new ServiceCollection();
        services.AddHtmxToolkit();

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HtmxToolkitOptions>>().Value;

        Assert.Multiple(() =>
        {
            Assert.That(options.TargetVersion, Is.EqualTo(HtmxTargetVersion.V2));
            Assert.That(options.HtmxConfig, Is.TypeOf<HtmxV2Config>());
            Assert.That(options.IncludeAntiforgeryToken, Is.True);
        });
    }
}
