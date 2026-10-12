[HtmxRequest(BoostedFilter = HtmxBoostedFilter.Boosted)]
public IActionResult Navigation() =>
    PartialView("_Navigation", menu.Items);
