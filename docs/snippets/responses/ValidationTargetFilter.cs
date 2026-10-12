// Sends invalid HTMX form posts to a shared error area,
// unless the action has already chosen another target.
public sealed class HtmxValidationTargetFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        var http = context.HttpContext;
        if (context.ModelState.IsValid || !http.Request.IsHtmxRequest())
            return;

        var htmx = http.Response.GetHtmxHeaders();
        htmx.Retarget ??= "#form-errors";
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}
