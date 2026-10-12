public IActionResult OnPostSave()
{
    _profiles.DisplayName = Input.DisplayName;

    // Components that listen for "profile-saved" reload themselves
    // after the form is swapped. The event needs no data.
    Response.Htmx(htmx => htmx.TriggerEvent("profile-saved", HtmxTriggerTiming.AfterSwap));

    return Partial("_ProfileForm", this);
}
