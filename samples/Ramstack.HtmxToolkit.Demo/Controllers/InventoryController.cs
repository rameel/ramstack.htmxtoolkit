using Microsoft.AspNetCore.Mvc;

namespace Ramstack.HtmxToolkit.Demo.Controllers;

public sealed class InventoryController : Controller
{
    [HttpGet]
    [HtmxRequest]
    [HtmxResponse(Reswap = HtmxSwap.OuterHtml)]
    public IActionResult Status(string sku)
    {
        var content = sku == "BOOK-42"
            ? "BOOK-42: Hypermedia Systems — 3 copies in stock."
            : "No inventory record was found for that SKU.";

        return Content($"<div id='inventory-status' class='result'>{content}</div>", "text/html");
    }
}
