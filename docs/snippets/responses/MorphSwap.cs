// Morphing updates the existing elements instead of replacing them,
// so focus and the state of form fields survive the update.
Response.Htmx(htmx => htmx.Reswap(HtmxSwap.OuterMorph));

return PartialView("_Profile", profile);
