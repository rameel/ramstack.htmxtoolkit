using Ramstack.HtmxToolkit.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHtmxToolkit(options =>
{
    options.UseHtmxV4(config =>
    {
        config.NoSwap = ["204", "304", "4xx", "5xx"];
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.MapHtmxToolkitScript();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
