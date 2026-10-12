[HttpGet("/profile")]
public IActionResult Profile() =>
    View();

[HttpGet("/profile")]
[HtmxRequest(RequestType = HtmxRequestType.Partial)]
public IActionResult ProfileFragment() =>
    PartialView("_Profile");
