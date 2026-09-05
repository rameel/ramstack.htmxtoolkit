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

        if (Request.IsHtmxRequest(out var headers))
        {
            var metadata = $"{status} Source: {headers.Source}; target: {headers.Target}; type: {headers.RequestType}.";
            return Content(HtmlEncoder.Default.Encode(metadata));
        }

        OrderStatus = $"{status} This was a normal browser request.";
        return Page();
    }
}
