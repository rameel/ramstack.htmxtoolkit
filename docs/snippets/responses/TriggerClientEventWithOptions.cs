public sealed class ProductsController(IOptions<JsonOptions> jsonOptions) : Controller
{
    [HttpPost]
    public IActionResult Save(Product product)
    {
        Response.Htmx(htmx => htmx.TriggerEvent(
            "product-saved",
            new { product.Id, product.Status },
            jsonOptions.Value.JsonSerializerOptions));

        return PartialView("_ProductRow", product);
    }
}
