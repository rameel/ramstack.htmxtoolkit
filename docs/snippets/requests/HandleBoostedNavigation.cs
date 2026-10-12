public IActionResult Navigation()
{
    if (Request.IsHtmxBoosted())
        return PartialView("_Navigation", menu.Items);

    return RedirectToAction("Index");
}
