public IActionResult Details(int id)
{
    var product = catalog.Get(id);

    if (Request.IsHtmxRequest())
        return PartialView("_ProductDetails", product);

    return View(product);
}
