options.UseHtmxV4(config =>
{
    // Keep the page unchanged on server errors, but still swap 4xx responses
    // such as 422 validation forms. Assigning the list replaces the HTMX default,
    // ["204", "304"], so the list repeats it.
    config.NoSwap = ["204", "304", "5xx"];
});
