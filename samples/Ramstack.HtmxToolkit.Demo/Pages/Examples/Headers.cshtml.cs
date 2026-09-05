using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class HeadersModel : PageModel
{
    public IActionResult OnGetShow() =>
        Content($"X-Report-Format: {HtmlEncoder.Default.Encode(Request.Headers["X-Report-Format"].ToString())}");
}
