using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ramstack.HtmxToolkit.TagHelpers;

/// <summary>
/// Represents a <see cref="TagHelper"/> that defines a single response handling entry
/// as a child of the <c>htmx-config</c> tag helper.
/// Each entry specifies how htmx should handle responses matching a particular HTTP status code pattern.
/// </summary>
/// <remarks>Supported only in HTMX 2.x.</remarks>
[HtmlTargetElement("response-handling", ParentTag = "htmx-config", TagStructure = TagStructure.WithoutEndTag)]
public sealed class ResponseHandlingTagHelper : TagHelper
{
    private readonly ResponseHandlingConfig _config = new();

    /// <summary>
    /// Gets or sets a regular expression that will be tested against response status codes.
    /// </summary>
    [HtmlAttributeName("code")]
    public string? Code
    {
        get => _config.Code;
        set => _config.Code = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the response should be swapped into the DOM.
    /// </summary>
    [HtmlAttributeName("swap")]
    public bool? Swap
    {
        get => _config.Swap;
        set => _config.Swap = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should treat this response as an error.
    /// </summary>
    [HtmlAttributeName("error")]
    public bool? Error
    {
        get => _config.Error;
        set => _config.Error = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether htmx should ignore title tags in the response.
    /// </summary>
    [HtmlAttributeName("ignore-title")]
    public bool? IgnoreTitle
    {
        get => _config.IgnoreTitle;
        set => _config.IgnoreTitle = value;
    }

    /// <summary>
    /// Gets or sets a CSS selector to use to select content from the response.
    /// </summary>
    [HtmlAttributeName("select")]
    public string? Select
    {
        get => _config.Select;
        set => _config.Select = value;
    }

    /// <summary>
    /// Gets or sets a CSS selector specifying an alternative target for the response.
    /// </summary>
    [HtmlAttributeName("target")]
    public string? Target
    {
        get => _config.Target;
        set => _config.Target = value;
    }

    /// <summary>
    /// Gets or sets an alternative swap mechanism for the response.
    /// </summary>
    [HtmlAttributeName("swap-override")]
    public string? SwapOverride
    {
        get => _config.SwapOverride;
        set => _config.SwapOverride = value;
    }

    /// <inheritdoc />
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (context.Items.TryGetValue(typeof(HtmxConfigTagHelper), out var value) && value is HtmxConfigTagHelper parent)
        {
            parent.ResponseHandling ??= new List<ResponseHandlingConfig>();
            parent.ResponseHandling.Add(_config);

            output.SuppressOutput();
            return Task.CompletedTask;
        }

        Error_NotNested();
        return Task.CompletedTask;
    }

    private static void Error_NotNested()
    {
        const string Message = "The '<response-handling>' tag helper can only be used inside the '<htmx-config>' tag helper";
        throw new InvalidOperationException(Message);
    }
}
