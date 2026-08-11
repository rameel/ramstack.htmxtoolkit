using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class ResponseHeadersModel : PageModel
{
    public IActionResult OnGetDeclarativeReswap()
    {
        Response.Htmx(h => h.Reswap(HtmxSwap.InnerHtml));
        return Content("Reswapped with InnerHtml via <code>Response.Htmx(h => h.Reswap(HtmxSwap.InnerHtml))</code>!");
    }
}
