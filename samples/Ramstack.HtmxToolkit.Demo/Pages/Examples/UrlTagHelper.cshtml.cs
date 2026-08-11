using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class UrlTagHelperModel : PageModel
{
    public IActionResult OnGetServerTime() =>
        Content($"<b>Server time:</b> {DateTime.Now:HH:mm:ss}");

    public IActionResult OnGetHello() =>
        Content("Hello from the server!");
}
