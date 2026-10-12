[HttpPost]
public IActionResult SignIn(LoginInput input)
{
    if (!auth.TrySignIn(input))
    {
        ModelState.AddModelError("", "Invalid email or password.");
        return Request.IsHtmxRequest()
            ? PartialView("_SignInForm", input)
            : View(input);
    }

    var url = Url.Action("Index", "Dashboard")!;

    if (Request.IsHtmxRequest())
    {
        // HTMX cannot read headers from a 3xx response, so send HX-Redirect with 200.
        Response.Htmx(htmx => htmx.Redirect(url));
        return Ok();
    }

    return Redirect(url);
}
