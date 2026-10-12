[HttpPost]
public IActionResult Save(ProductInput input)
{
    var product = catalog.Update(input);

    // Load the details fragment into the page, but show the product page URL
    // in the address bar so that a reload or a shared link opens the full page.
    Response.Htmx(htmx => htmx.Location(
        Url.Action(nameof(Details), new { id = product.Id })!,
        new HtmxLocationOptions
        {
            Target = "#product-details",
            Swap = HtmxSwap.OuterHtml,
            HistoryUrl = Url.Action(nameof(Show), new { id = product.Id })
        }));

    return NoContent();
}
