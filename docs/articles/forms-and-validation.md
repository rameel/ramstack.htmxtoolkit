# Forms, validation, and events

Continue in the `HtmxStarter` project from [Getting started](getting-started.md).
This tutorial adds a profile form that shows server validation errors, saves a display name,
and refreshes a separate summary through a response event.

The example keeps one shared profile in memory. It resets when the application restarts and is visible
to every visitor. Replace this store with your application's user-specific persistence before using the pattern
for real profiles.

## 1. Add the input model and store

Create `Models/ProfileInput.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace HtmxStarter.Models;

public sealed class ProfileInput
{
    private string _displayName = "";

    [Required(ErrorMessage = "Enter a display name.")]
    [StringLength(40, MinimumLength = 2, ErrorMessage = "Use 2 to 40 characters.")]
    public string DisplayName
    {
        get => _displayName;
        set => _displayName = value?.Trim() ?? "";
    }
}
```

The setter removes surrounding whitespace during model binding, before validation runs.
An input such as `" A "` therefore fails the two-character minimum. Trimming only in the save handler
would be too late: ASP.NET Core has already validated the model by then.

Create `Models/ProfileStore.cs`:

```csharp
namespace HtmxStarter.Models;

public sealed class ProfileStore
{
    public string DisplayName { get; set; } = "Ada";
}
```

In `Program.cs`, add `using HtmxStarter.Models;` and register the store before `builder.Build()`:

```csharp
builder.Services.AddSingleton<ProfileStore>();
```

## 2. Add the form partial

Create `Pages/_ProfileForm.cshtml`:

```razor
@model ProfileModel

<form id="profile-editor"
      method="post"
      hx-post
      asp-page="/Profile"
      asp-page-handler="Save"
      hx-target="this"
      hx-swap="outerHTML"
      novalidate>
    <div asp-validation-summary="All" role="alert"></div>

    <label asp-for="Input.DisplayName">Display name</label>
    <input asp-for="Input.DisplayName" />
    <button type="submit">Save</button>

    @if (Model.Saved)
    {
        <p role="status">Profile saved.</p>
    }
</form>
```

`asp-for` produces the name `Input.DisplayName`, which matches the bound property in the handler below.
`asp-validation-summary` displays errors from `ModelState`.

The form replaces itself with `outerHTML`. Every returned form keeps the same ID and request attributes,
so it can be submitted again. `novalidate` lets you see the server's validation response without the browser
blocking an empty submission first. You can remove it when you add browser validation.

The `asp-page` and `asp-page-handler` attributes produce both the `hx-post` URL and, through the standard
Form Tag Helper, the ordinary form `action`, so the form also works without HTMX. The Form Tag Helper
also creates a hidden antiforgery input. The Toolkit script detects that input and does not add a second token.
For forms without that input, the layout metadata and script supply the token instead.

## 3. Add the page and summary

Create `Pages/_ProfileSummary.cshtml`:

```razor
@model string

<p>Current display name: <strong>@Model</strong></p>
```

Create `Pages/Profile.cshtml`:

```razor
@page
@model ProfileModel

<h1>Profile</h1>

@await Html.PartialAsync("_ProfileForm", Model)

<aside id="profile-summary"
       hx-get
       asp-page="/Profile"
       asp-page-handler="Summary"
       hx-trigger="profile-saved from:body"
       aria-live="polite">
    @await Html.PartialAsync("_ProfileSummary", Model.CurrentDisplayName)
</aside>
```

The summary is outside the form's swap target. It stays in place while the form is replaced and listens
for a `profile-saved` event bubbling through `body`.

## 4. Handle validation and saving

Create `Pages/Profile.cshtml.cs`:

```csharp
using HtmxStarter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ramstack.HtmxToolkit;

namespace HtmxStarter.Pages;

public sealed class ProfileModel : PageModel
{
    private readonly ProfileStore _profiles;

    public ProfileModel(ProfileStore profiles) => _profiles = profiles;

    [BindProperty]
    public ProfileInput Input { get; set; } = new();

    public bool Saved { get; private set; }

    public string CurrentDisplayName => _profiles.DisplayName;

    public void OnGet() => Input.DisplayName = _profiles.DisplayName;

    public IActionResult OnGetSummary() =>
        Partial("_ProfileSummary", _profiles.DisplayName);

    public IActionResult OnPostSave()
    {
        if (!ModelState.IsValid)
            return Request.IsHtmxRequest() ? Partial("_ProfileForm", this) : Page();

        _profiles.DisplayName = Input.DisplayName;
        ModelState.Clear();
        Saved = true;

        Response.Htmx(htmx => htmx.TriggerEvent(
            "profile-saved", HtmxTriggerTiming.AfterSwap));

        return Request.IsHtmxRequest() ? Partial("_ProfileForm", this) : RedirectToPage();
    }
}
```

Razor Pages validates the antiforgery token before the POST handler runs. Model binding then fills `Input`
and applies its validation attributes. Invalid input returns the form with errors and leaves the store unchanged.

A successful save returns a fresh form and sends `profile-saved`. HTMX receives that event, requests
`/Profile?handler=Summary`, and updates the summary with the stored name. No custom JavaScript is needed.
`ModelState.Clear()` lets the form show the trimmed value instead of the originally submitted text.

Normal form submissions return a full page on error and redirect back after a successful save.

## 5. Try both paths

Run the application as in Getting started and open <http://localhost:5080/Profile>.

1. Clear the display name and select **Save**. The form should show “Enter a display name.” and the summary should still show “Ada”.
2. Enter a one-character name, then try `" A "` with surrounding spaces. Both should show the length error.
3. Enter `" Grace "` and save. The error should disappear, the form should show “Profile saved.”, and both the input and summary should show “Grace” without surrounding spaces.
4. Save a different name to check that replacing the form did not break the next submission.
5. Disable JavaScript and reload. The form should still save using normal navigation.

For a valid HTMX save, the Network panel shows a POST followed by a GET for the summary.
With the default HTMX 2.x setup, the POST response carries `HX-Trigger-After-Swap`.
HTMX 4.x uses `HX-Trigger` for the same Toolkit call; see [event timing](version-compatibility.md#response-events).

## Why validation uses HTTP 200 here

The invalid form is returned with status 200 so the default HTMX 2.x configuration displays it.
If your application uses status 422 for validation, also configure [response handling](configuration-v2.md#handle-validation-responses).
Changing `Retarget` or `Reswap` alone does not make HTMX 2.x swap an error response.

For errors outside the form, see [Return validation errors to another target](recipes.md#return-validation-errors-to-another-target).
For MVC and Minimal APIs, read [Antiforgery](antiforgery.md) before copying the POST pattern.

## Next step

Continue with [Loading states and request errors](loading-and-errors.md) to show that a save is in progress,
disable repeated clicks, and let the user recover from a failed request.
