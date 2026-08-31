using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class PollingModel : PageModel
{
    public PollingState State { get; } = new(false, "Polling is active");

    public IActionResult OnGetPoll()
    {
        var stopped = Random.Shared.Next(0, 20) == 10;
        var message = stopped
            ? "Polling stopped!"
            : $"Polling... {DateTime.Now:HH:mm:ss}";

        return Partial("_PollingStatus", new PollingState(stopped, message));
    }

    public sealed record PollingState(bool Stopped, string Message);
}
