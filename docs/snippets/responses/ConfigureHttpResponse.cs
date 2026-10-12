public IActionResult OnGetProduct(string sku)
{
    var product = catalog.Find(sku);
    if (product is null)
    {
        // Runs only for HTMX requests: show the message in the notice area
        // instead of the product panel that the button targets.
        Response.Htmx(htmx => htmx.Retarget("#product-notice"));
        return Partial("_ProductNotFound", sku);
    }

    return Partial("_ProductDetails", product);
}
