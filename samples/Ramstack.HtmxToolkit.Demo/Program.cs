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

app.MapStaticAssets();
app.UseRouting();
app.MapDefaultControllerRoute();
app.MapRazorPages().WithStaticAssets();

app.Run();
