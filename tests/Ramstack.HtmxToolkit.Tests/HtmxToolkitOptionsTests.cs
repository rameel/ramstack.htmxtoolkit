using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Ramstack.HtmxToolkit.Builder;

namespace Ramstack.HtmxToolkit.Tests;

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
            Assert.That(options.Htmx, Is.TypeOf<HtmxV2Options>());
        });
    }

    [Test]
    public void UseHtmxV1_StoresInspectableOptions()
    {
        var options = new HtmxToolkitOptions();

        options.UseHtmxV1(htmx => htmx.Timeout = 2500);

        var htmx = options.GetHtmxOptions<HtmxV1Options>();
        Assert.Multiple(() =>
        {
            Assert.That(options.TargetVersion, Is.EqualTo(HtmxTargetVersion.V1));
            Assert.That(options.Htmx, Is.SameAs(htmx));
            Assert.That(htmx.Timeout, Is.EqualTo(2500));
        });
    }

    [Test]
    public void UseHtmxV4_StoresInspectableOptions()
    {
        var options = new HtmxToolkitOptions();

        options.UseHtmxV4(htmx => htmx.DefaultTimeout = 5000);

        var htmx = options.GetHtmxOptions<HtmxV4Options>();
        Assert.Multiple(() =>
        {
            Assert.That(options.TargetVersion, Is.EqualTo(HtmxTargetVersion.V4));
            Assert.That(options.Htmx, Is.SameAs(htmx));
            Assert.That(htmx.DefaultTimeout, Is.EqualTo(5000));
        });
    }

    [Test]
    public void UseHtmxV2_RepeatedCallsComposeOnSameOptions()
    {
        var options = new HtmxToolkitOptions();

        options.UseHtmxV2(htmx => htmx.Timeout = 1000);
        var first = options.Htmx;
        options.UseHtmxV2(htmx => htmx.DefaultSettleDelay = 20);

        var htmx = options.GetHtmxOptions<HtmxV2Options>();
        Assert.Multiple(() =>
        {
            Assert.That(htmx, Is.SameAs(first));
            Assert.That(htmx.Timeout, Is.EqualTo(1000));
            Assert.That(htmx.DefaultSettleDelay, Is.EqualTo(20));
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
    public void GetHtmxOptions_ForDifferentVersion_Throws()
    {
        var options = new HtmxToolkitOptions();
        options.UseHtmxV4();

        Assert.That(
            () => options.GetHtmxOptions<HtmxV2Options>(),
            Throws.TypeOf<InvalidOperationException>()
                  .With.Message.EqualTo("HTMX configuration version 'V4' does not match the requested options type.")
        );
    }

    [Test]
    public void UseHtmxV4_AfterReadingDefault_ThrowsInvalidOperationException()
    {
        var options = new HtmxToolkitOptions();
        var defaultHtmxOptions = options.Htmx;

        Assert.Multiple(() =>
        {
            Assert.That(defaultHtmxOptions, Is.TypeOf<HtmxV2Options>());
            Assert.That(
                () => options.UseHtmxV4(htmx => htmx.DefaultTimeout = 5000),
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
            options.UseHtmxV4(htmx => htmx.DefaultTimeout = 5000);
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HtmxToolkitOptions>>().Value;
        var htmx = options.GetHtmxOptions<HtmxV4Options>();

        Assert.Multiple(() =>
        {
            Assert.That(options.IncludeAntiforgeryToken, Is.True);
            Assert.That(htmx.DefaultTimeout, Is.EqualTo(5000));
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
            Assert.That(options.Htmx, Is.TypeOf<HtmxV2Options>());
            Assert.That(options.IncludeAntiforgeryToken, Is.False);
        });
    }
}
