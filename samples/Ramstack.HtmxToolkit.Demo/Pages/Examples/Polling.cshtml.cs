using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class PollingModel : PageModel
{
    public IActionResult OnGetStopPolling()
    {
        var stop = Random.Shared.Next(0, 20) == 10;
        Response.Htmx((h, f) => h.StopPolling(f), stop);

        var content = $"Polling... {DateTime.Now:HH:mm:ss}";
        if (stop)
            content += "<span id='poll-status' hx-swap-oob='true' style='color: orange'>Polling stopped!</span>";

        return Content(content);
    }
}
