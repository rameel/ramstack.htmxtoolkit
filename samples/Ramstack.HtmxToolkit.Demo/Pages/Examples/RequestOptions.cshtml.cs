using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class RequestOptionsModel : PageModel
{
    public async Task<IActionResult> OnGetPreviewAsync()
    {
        await Task.Delay(750, HttpContext.RequestAborted);
        return Content($"Report preview generated at {DateTime.UtcNow:HH:mm:ss} UTC.");
    }
}
