public IActionResult OpenProduct(int id)
{
    Response.Htmx(htmx => 
        htmx.Location($"/products/{id}", new HtmxLocationOptions
        {
            Source = "#view-product",
            Target = "#product-details",
            Swap = HtmxSwap.OuterHtml,
            Select = "#product-details",
            Push = $"/products/{id}"
        }));

    return NoContent();
}
