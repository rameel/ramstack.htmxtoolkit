using Ramstack.HtmxToolkit;
using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHtmxToolkit(options =>
{
    options.IncludeAntiforgeryToken = true;
    options.UseHtmxV2(config =>
    {
        config.DefaultSwapStyle = HtmxSwap.InnerHtml;
        config.MethodsThatUseUrlParams = [HttpVerb.Get, HttpVerb.Delete];
        config.ResponseHandling =
        [
            new() { Code = "204", Swap = false },
            new() { Code = "422", Swap = true },
            new() { Code = "[23]..", Swap = true },
            new() { Code = "[45]..", Swap = false, Error = true },
            new() { Code = "...", Swap = true }
        ];
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapHtmxToolkitScript();
app.MapRazorPages();

app.Run();
