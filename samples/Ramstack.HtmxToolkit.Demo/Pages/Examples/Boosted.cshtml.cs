using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class BoostedModel : PageModel
{
    public IActionResult OnGetBoostedCheck() =>
        Content(
            Request.IsHtmxBoosted()
                ? "Boosted HTMX request detected!"
                : "Non-boosted HTMX request.");
}
