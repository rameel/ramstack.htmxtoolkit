# Loading states and request errors

Continue with the profile form from [Forms, validation, and events](forms-and-validation.md).
This tutorial uses HTMX 2.x. It shows a loading message, disables the save button during the request,
and displays an error when the server or connection fails.

Validation errors remain part of the returned form. Request errors need a separate message because the
default HTMX 2.x settings leave most 4xx and 5xx responses unswapped.

## 1. Show when a save is running

In `Pages/_ProfileForm.cshtml`, replace the opening `<form>` tag with:

```razor
<form id="profile-editor"
      method="post"
      asp-page="/Profile"
      asp-page-handler="Save"
      hx-post
      hx-page="/Profile"
      hx-page-handler="Save"
      hx-target="this"
      hx-swap="outerHTML"
      hx-indicator="#profile-busy"
      hx-disabled-elt="find button[type='submit']"
      hx-request-timeout="5000"
      novalidate>
```

Keep the form's contents and closing tag. In `Pages/Profile.cshtml`, add these elements immediately after
the `_ProfileForm` partial and before the summary:

```html
<p id="profile-busy" class="htmx-indicator" role="status">Saving…</p>
<p id="profile-request-error" role="alert"></p>
```

HTMX shows its indicator while the request is running and removes the disabled state when the request ends.
The `find` selector limits the disabled state to the form's submit button. The input stays available, and its
name and value are still submitted. HtmxToolkit turns the timeout input into `hx-request` JSON for HTMX 2.x;
the timeout is in milliseconds.

The indicator uses HTMX's default indicator CSS. If you disabled `IncludeIndicatorStyles`, supply your own
styles as described in the [HTMX indicator guide](https://htmx.org/attributes/hx-indicator/).
See also [HTMX's disabled-element attribute](https://htmx.org/attributes/hx-disabled-elt/).

## 2. Show a message when a request fails

Create `wwwroot/js/profile-feedback.js`:

```javascript
function showProfileError(event, message) {
    if (event.detail.elt.id === "profile-editor") {
        document.getElementById("profile-request-error").textContent = message;
    }
}

document.addEventListener("htmx:beforeRequest", event => {
    showProfileError(event, "");
});

document.addEventListener("htmx:responseError", event => {
    showProfileError(event, "The server could not complete the request. Try Save again.");
});

document.addEventListener("htmx:sendError", event => {
    showProfileError(event, "Could not confirm the save. Check your connection and try again.");
});

document.addEventListener("htmx:timeout", event => {
    showProfileError(event, "The request took too long. Try Save again.");
});
```

Load the file once in `Pages/Shared/_Layout.cshtml`, after HTMX and the Toolkit script:

```razor
<script src="~/js/profile-feedback.js" asp-append-version="true"></script>
```

The listeners use `document`, so they survive replacement of the form. They only handle events from
`profile-editor`; the summary request does not clear the message. The error element is outside the form
so it remains available after a swap. Use `textContent` to display a message as text.

An HTTP 500 response now leaves the input in place and shows a message. HTMX enables the button again,
so the user can submit the same form. Starting another save clears the old request error.
The form's HTTP 200 validation response still displays its field errors normally.

A timeout or lost connection does not prove that the server failed to save. Repeating this example's
assignment of a display name is safe. For operations such as creating an order, handle duplicate
submissions on the server before offering a retry.

## 3. Try success, failure, and recovery

Run the application and open <http://localhost:5080/Profile>. Keep the Network panel open:

1. Enable a slow network profile and save a valid name. While the request is running, “Saving…” should appear
   and the save button should be disabled. After success, the form and summary should update.
2. Submit an empty name. The form should show its validation error without a separate request-error message.
3. Switch the Network panel to Offline, then save. A connection error should appear, the input should remain,
   and the button should become available again. Return to Online and retry without reloading the page.
4. To exercise a timeout, temporarily set `hx-request-timeout="1"` in the partial, reload, and submit under
   network throttling. Restore `5000` and reload after checking the timeout message.

To test an HTTP error, temporarily replace the `OnPostSave` method in `Pages/Profile.cshtml.cs` with:

```csharp
public IActionResult OnPostSave() => StatusCode(500);
```

Restart the application and submit the form. The Network panel should show 500 and the page should keep the
form and show the server-error message. Restore the original handler, restart, and save again to check recovery.

## Use the pattern with MVC

In the [MVC example](mvc.md), add the indicator, disabled-button, and timeout attributes to `_ProductForm`.
Keep the indicator and error message outside that partial in `Edit.cshtml`. Change the JavaScript ID check
from `profile-editor` to `product-editor`, and use matching IDs for the indicator and error element.
The controller does not need extra response headers for this browser feedback.

## Adapting to HTMX 4

The JavaScript above uses HTMX 2.x lifecycle events. HTMX 4 changes their names and event data:

| Purpose                                 | HTMX 2.x                         | HTMX 4.x              |
|-----------------------------------------|----------------------------------|-----------------------|
| Clear the previous error before sending | `htmx:beforeRequest`             | `htmx:before:request` |
| Report an HTTP error                    | `htmx:responseError`             | `htmx:response:error` |
| Report a connection failure or timeout  | `htmx:sendError`, `htmx:timeout` | `htmx:error`          |

When porting the listeners, read the request context from `event.detail.ctx`; response status is available
as `ctx.response.status` when a response exists. `htmx:error` also covers exceptions during a swap, so inspect
`event.detail.error` before choosing a message. See the [HTMX 4 event reference](https://four.htmx.org/reference/events).

HTMX 4 swaps error responses by default. To keep the form in place as this tutorial does, also configure
`NoSwap = ["204", "304", "4xx", "5xx"]` as shown in [HTMX 4 configuration](configuration-v4.md#prevent-swaps-for-status-codes).

For a login page appearing inside a form or a request failing silently, see [Troubleshooting](troubleshooting.md).
