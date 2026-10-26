// The same URL returns a page or a fragment depending on these headers,
// so they must be part of the output cache key.
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("HtmxHtml", policy =>
        policy.SetVaryByHeader(
            HtmxRequestHeaderNames.Request,
            HtmxRequestHeaderNames.HistoryRestoreRequest,
            HtmxRequestHeaderNames.RequestType));
});
