using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class HeadersModel : PageModel
{
    public IActionResult OnGetCustomHeader()
    {
        Response.Htmx(h => h
            .TriggerEvent("customEvent", new { message = "#1 Fired from server!" })
            .TriggerEvent("logEvent", new { message = $"Custom-Header = {Request.Headers["Custom-Header"]}" })
            .TriggerEvent("customEvent", new { message = "#2 Fired from server!" })
            .TriggerEvent("customEvent", new { message = "#3 Fired from server!" }));

        return Content("<b>Custom headers sent!</b>");
    }
}
