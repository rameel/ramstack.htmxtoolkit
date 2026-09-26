using Microsoft.AspNetCore.Http;
using Ramstack.HtmxToolkit;

var request = new DefaultHttpContext().Request;
request.Headers[HtmxRequestHeaderNames.Request] = "true";

if (!request.IsHtmxRequest())
    throw new InvalidOperationException("HTMX request detection failed.");

Console.WriteLine("HTMX request detected.");
