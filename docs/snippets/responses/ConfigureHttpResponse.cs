public IActionResult OnGetProduct(string sku)
{
    if (sku == "BOOK-42")
        return Content("BOOK-42: Hypermedia Systems — in stock.");

    Response.Htmx(htmx => htmx.Retarget("#product-notice"));
    return Content("No product found for that SKU.");
}
