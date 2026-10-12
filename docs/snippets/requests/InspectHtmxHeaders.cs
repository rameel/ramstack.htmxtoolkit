if (Request.IsHtmxRequest(out var htmx))
{
    logger.LogDebug(
        "HTMX request for {Target} from {Source}",
        htmx.Target,
        htmx.Source ?? htmx.Trigger);
}
