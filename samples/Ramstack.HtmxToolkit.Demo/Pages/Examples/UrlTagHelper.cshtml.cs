using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class UrlTagHelperModel : PageModel
{
    public IActionResult OnGetStatus() =>
        Content($"Service is healthy. Checked at {DateTime.UtcNow:HH:mm:ss} UTC.");
}
