// A page on another origin loads fragments from this application. The browser hides
// cross-origin response headers from HTMX unless the server exposes them.
builder.Services.AddCors(options =>
    options.AddPolicy("htmx", policy => policy
        .WithOrigins("https://app.example.com")
        .AllowAnyHeader()
        .WithExposedHeaders(
            HtmxResponseHeaderNames.Trigger,
            HtmxResponseHeaderNames.Retarget,
            HtmxResponseHeaderNames.Reswap)));
