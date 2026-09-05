using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class BoostedModel : PageModel
{
    public IActionResult OnGetBoostedCheck() =>
        Request.IsHtmxBoosted()
            ? Content("Boosted HTMX request detected.")
            : RedirectToPage();
}
