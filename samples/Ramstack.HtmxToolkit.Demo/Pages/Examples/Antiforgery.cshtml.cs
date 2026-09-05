using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class AntiforgeryModel : PageModel
{
    public IActionResult OnPostSave(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return BadRequest("Display name is required.");

        return Content($"Settings saved for {HtmlEncoder.Default.Encode(displayName)}.");
    }
}
