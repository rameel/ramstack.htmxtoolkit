// A widget polls this URL; it is not a page. The action is selected only
// for HTMX requests, so a direct browser visit gets 404 instead of a bare fragment.
[HttpGet("/orders/{id:int}/status")]
[HtmxRequest]
public IActionResult Status(int id) =>
    PartialView("_OrderStatus", orders.GetStatus(id));
