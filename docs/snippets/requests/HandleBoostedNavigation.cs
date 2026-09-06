public IActionResult OnGetBoostedCheck() =>
    Request.IsHtmxBoosted()
        ? Content("Boosted HTMX request detected.")
        : RedirectToPage();
