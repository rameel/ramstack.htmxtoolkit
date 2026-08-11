using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class HtmxRequestModel : PageModel
{
    public IActionResult OnGetPartialOrFull() =>
        Content(
            Request.IsHtmxRequest()
                ? "Partial response (HTMX request detected via <code>IsHtmxRequest()</code>)"
                : "Full page response. This wouldn't normally be a Content result, but demonstrates the check.");
}
