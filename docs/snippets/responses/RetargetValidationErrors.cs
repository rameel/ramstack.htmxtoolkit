Response.Htmx(htmx => htmx
    .Retarget("#validation-errors")
    .Reswap(HtmxSwap.InnerHtml));

return PartialView("_ValidationSummary", ModelState);
