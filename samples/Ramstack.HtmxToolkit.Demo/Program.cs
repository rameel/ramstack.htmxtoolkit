using Ramstack.HtmxToolkit.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapHtmxAntiforgeryScript();
app.MapRazorPages();

app.Run();
