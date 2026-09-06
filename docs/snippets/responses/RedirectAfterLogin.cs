public IActionResult SignIn(LoginModel model)
{
    if (!auth.TrySignIn(model))
        return Unauthorized();

    Response.Htmx(htmx => htmx.Redirect("/dashboard"));
    return Ok();
}
