var url = Url.Action(nameof(Show), new { id = product.Id })!;

// The static lambda captures nothing: the URL is passed as state,
// so the call does not allocate a closure.
Response.Htmx(static (htmx, url) => htmx.PushUrl(url), url);
