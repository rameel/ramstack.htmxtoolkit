[HttpGet("/inventory/{sku}")]
[HtmxRequest]
[HtmxResponse(Retarget = "#inventory-status", Reswap = HtmxSwap.OuterHtml)]
public IActionResult Inventory(string sku) =>
    PartialView("_Inventory", inventory.Find(sku));
