using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class RandomModel : PageModel
{
    public IActionResult OnGetRandom() =>
        Content($"<b>Random number:</b> {Random.Shared.Next(1, 100)}");
}
