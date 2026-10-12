options.UseHtmxV2(config =>
{
    // Assigning the list replaces the HTMX defaults, so the rules repeat them
    // and add one more: swap validation forms returned with status 422.
    config.ResponseHandling =
    [
        new() { Code = "204", Swap = false },
        new() { Code = "[23]..", Swap = true },
        new() { Code = "422", Swap = true, Error = false },
        new() { Code = "[45]..", Swap = false, Error = true }
    ];
});
