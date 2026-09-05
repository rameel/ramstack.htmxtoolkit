using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

public class HtmxValsModel : PageModel
{
    public IActionResult OnGetShow(string? category, string? format)
    {
        return Content($"""
            <p><b>Category:</b> {Encode(category)}</p>
            <p><b>Format:</b> {Encode(format)}</p>
            """);

        static string Encode(string? text) =>
            text is { Length: > 0 }
                ? HtmlEncoder.Default.Encode(text)
                : "(not supplied)";
    }
}
