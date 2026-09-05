using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class RouteDataModel : PageModel
{
    public IActionResult OnGetOrder(int id) =>
        Content($"Order #{id} is packed and ready for pickup.");
}
