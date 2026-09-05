using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class HtmxRequestModel : PageModel
{
    public string? OrderStatus { get; private set; }

    public IActionResult OnGet(string? id)
    {
        if (id is null)
            return Page();

        var status = $"Order #{id} is ready for pickup.";
        OrderStatus = status;

        return Request.IsHtmxRequest()
            ? Content(HtmlEncoder.Default.Encode(status))
            : Page();
    }
}
