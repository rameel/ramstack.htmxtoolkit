using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class RouteDataModel : PageModel
{
    public IActionResult OnGetGreet(string name) =>
        Content($"<b>Hello, {name}!</b>");
}
