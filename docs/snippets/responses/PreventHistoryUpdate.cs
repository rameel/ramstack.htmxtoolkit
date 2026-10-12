[HttpPost("/orders")]
public IActionResult Create(OrderInput input)
{
    if (!ModelState.IsValid)
    {
        // The boosted form on /orders/new would push /orders, the POST URL,
        // into the history. Keep /orders/new so that a reload shows the form again.
        Response.Htmx(htmx => htmx.PreventHistoryUpdate());
        return View("New", input);
    }

    var order = orders.Create(input);
    return RedirectToAction(nameof(Details), new { id = order.Id });
}
