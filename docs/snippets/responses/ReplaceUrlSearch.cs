public IActionResult OnGetSearch(string query = "")
{
    // Keep the query in the address bar without adding a history entry for every keystroke.
    // The page must render the same results when /Catalog?query=... is opened directly.
    Response.Htmx(htmx => htmx.ReplaceUrl(Url.Page("/Catalog", new { query })!));

    return Partial("_SearchResults", catalog.Search(query));
}
