if (Request.IsHtmxRequest(out var htmx))
{
    var metadata = $"Source: {htmx.Source}; target: {htmx.Target}; type: {htmx.RequestType}.";
    return Content(HtmlEncoder.Default.Encode(metadata));
}
