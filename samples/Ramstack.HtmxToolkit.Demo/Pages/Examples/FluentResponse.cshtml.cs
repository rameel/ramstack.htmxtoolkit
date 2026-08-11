using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class FluentResponseModel : PageModel
{
    public IActionResult OnGetReswap()
    {
        Response.Htmx(h => h.Reswap(HtmxSwap.AfterBegin));
        return Content("<p>Prepended to top with AfterBegin swap!</p>");
    }

    public IActionResult OnGetRetarget()
    {
        Response.Htmx(h => h.Retarget("#fluent-result"));
        return Content("<b>Retargeted to #fluent-result!</b>");
    }
}
