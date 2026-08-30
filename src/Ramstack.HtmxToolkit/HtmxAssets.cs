using System.Security.Cryptography;
using System.Text;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides access to the embedded HTMX Toolkit script assets.
/// </summary>
public static class HtmxAssets
{
    /// <summary>
    /// The unminified HTMX Toolkit script.
    /// </summary>
    public static readonly string DebugScript = GetResourceContent("htmx-toolkit.js");

    /// <summary>
    /// The minified HTMX Toolkit script.
    /// </summary>
    public static readonly string Script = GetResourceContent("htmx-toolkit.min.js");

    /// <summary>
    /// The content hash of the HTMX Toolkit script.
    /// </summary>
    public static readonly string Hash = Convert
        .ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(DebugScript)).AsSpan(0, 8))
        .ToLowerInvariant();

    /// <summary>
    /// Reads an embedded text resource.
    /// </summary>
    /// <param name="name">The manifest resource name.</param>
    /// <returns>
    /// The resource contents.
    /// </returns>
    private static string GetResourceContent(string name)
    {
        using var stream = typeof(HtmxAssets).Assembly.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
