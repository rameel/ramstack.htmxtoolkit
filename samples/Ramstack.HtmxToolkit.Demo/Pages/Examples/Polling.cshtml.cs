using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class PollingModel : PageModel
{
    public IActionResult OnPostStart() =>
        Partial("_PollingStatus", new ProgressState(0));

    public IActionResult OnGetProgress(int progress)
    {
        progress = Math.Clamp(progress + 10, 0, 100);

        return Partial("_PollingStatus", new ProgressState(progress));
    }

    public sealed record ProgressState(int Percent)
    {
        public bool Completed => Percent == 100;

        public string Scale =>
            (Percent / 100d).ToString("0.##", CultureInfo.InvariantCulture);
    }
}
