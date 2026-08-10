using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ramstack.HtmxToolkit.Demo.Pages;

[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    public IActionResult OnGet() =>
        Page();

    public IActionResult OnGetServerTime() =>
        Content($"<b>Server time:</b> {DateTime.Now:HH:mm:ss}");

    public IActionResult OnGetHello() =>
        Content("Hello from the server!");

    public IActionResult OnGetGreet(string name) =>
        Content($"<b>Hello, {name}!</b>");

    public IActionResult OnGetCustomHeader()
    {
        Response.Htmx(h => h
            .TriggerEvent("customEvent", new { message = "#1 Fired from server!" })
            .TriggerEvent("logEvent", new { message = $"Custom-Header = {Request.Headers["Custom-Header"]}" }));

        return Content("<em>Custom headers sent. Check the console and events.</em>")
            .Htmx(h => h
                .TriggerEvent("customEvent", new { message = "#2 Fired from server!" })
                .TriggerEvent("customEvent", new { message = "#3 Fired from server!" }));
    }

    public IActionResult OnGetReswap()
    {
        Response.Htmx(h => h.Reswap(HtmxSwap.AfterBegin));
        return Content("<p>Prepended to top with AfterBegin swap!</p>");
    }

    public IActionResult OnGetRetarget()
    {
        Response.Htmx(h => h.Retarget("#fluent-result"));
        return Content("<b>Retargeted to #fluent-result!</b>");
    }

    public IActionResult OnGetStopPolling()
    {
        var stop = Random.Shared.Next(0, 50) == 1;
        Response.Htmx((h, f) => h.StopPolling(f), stop);

        return Content(stop
            ? "<span style='color: orange;'>Polling stopped!</span>"
            : $"<span>Polling... {DateTime.Now:HH:mm:ss}</span>");
    }

    public IActionResult OnGetPartialOrFull()
    {
        return Content(
            Request.IsHtmxRequest()
                ? "Partial response (HTMX request detected via <code>IsHtmxRequest()</code>)"
                : "Full page response. This wouldn't normally be a Content result, but demonstrates the check.");
    }

    public IActionResult OnGetDeclarativeReswap()
    {
        Response.Htmx(h => h.Reswap(HtmxSwap.InnerHtml));
        return Content("Reswapped with InnerHtml via <code>Response.Htmx(h => h.Reswap(HtmxSwap.InnerHtml))</code>!");
    }

    public IActionResult OnGetBoostedCheck()
    {
        return Content(
            Request.IsHtmxBoosted()
                ? "Boosted HTMX request detected!"
                : "Non-boosted HTMX request.");
    }

    public IActionResult OnGetRandom() =>
        Content($"<b>Random number:</b> {Random.Shared.Next(1, 100)}");

    public IActionResult OnPostFormSubmit(ContactForm form)
    {
        return Content($"""
            <b>Form submitted successfully!</b><br/>
            <b>Name:</b> {form.Name}<br/>
            <b>Email:</b> {form.Email}<br/>
            <b>Message:</b> {form.Message}
            """);
    }

    public class ContactForm
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
    }
}
