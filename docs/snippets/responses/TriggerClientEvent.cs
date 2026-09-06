Response.Htmx(htmx => htmx.TriggerEvent(
    "product-saved",
    new { product.Id },
    HtmxTriggerTiming.AfterSwap));

return PartialView("_ProductRow", product);
