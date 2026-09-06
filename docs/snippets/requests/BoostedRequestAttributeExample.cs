[HtmxRequest(Boosted = true)]
public IActionResult Navigation() =>
    PartialView("_Navigation", menu.Items);
