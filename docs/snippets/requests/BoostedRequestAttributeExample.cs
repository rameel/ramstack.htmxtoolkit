[HtmxRequest(Kind = HtmxRequestKind.Boosted)]
public IActionResult Navigation() =>
    PartialView("_Navigation", menu.Items);
