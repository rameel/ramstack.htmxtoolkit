[HtmxRequest]
[HtmxResponse(
    Retarget = "#inventory-status",
    Reswap = HtmxSwap.OuterHtml)]
[HttpGet("/inventory/{sku}")]
public IActionResult Inventory(string sku) =>
    PartialView("_Inventory", inventory.Find(sku));
