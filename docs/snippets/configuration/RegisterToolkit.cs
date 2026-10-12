using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// HTMX 2.x is the default. Select the version that your pages load,
// and set only the HTMX options that should differ from the HTMX defaults.
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV4(config =>
    {
        config.DefaultTimeout = 10_000;
    });
});
