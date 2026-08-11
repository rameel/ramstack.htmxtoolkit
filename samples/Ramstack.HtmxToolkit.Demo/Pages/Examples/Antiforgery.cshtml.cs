using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages.Examples;

[ValidateAntiForgeryToken]
public class AntiforgeryModel : PageModel
{
    public IActionResult OnPostFormSubmit(ContactForm form) =>
        Content($"""
            <b>Form submitted successfully!</b><br/>
            <b>Name:</b> {form.Name}<br/>
            <b>Email:</b> {form.Email}<br/>
            <b>Message:</b> {form.Message}
            """);

    public class ContactForm
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
    }
}
