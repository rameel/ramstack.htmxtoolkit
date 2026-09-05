using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class ServerEventsModel : PageModel
{
    public IActionResult OnPostSave(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return BadRequest("Display name is required.");

        Response.Htmx(htmx => htmx.TriggerEvent("profileSaved"));
        return Content("Profile saved.");
    }

    public IActionResult OnGetSummary(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return BadRequest("Display name is required.");

        return Content($"Current profile: {HtmlEncoder.Default.Encode(displayName)}.");
    }
}
