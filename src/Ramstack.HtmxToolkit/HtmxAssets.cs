using System.Security.Cryptography;
using System.Text;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides access to the embedded HTMX toolkit script assets.
/// </summary>
public static class HtmxAssets
{
    /// <summary>
    /// The unminified HTMX toolkit script.
    /// </summary>
    public static readonly string DebugScript = GetResource("htmx-toolkit.js");

    /// <summary>
    /// The minified HTMX toolkit script.
    /// </summary>
    public static readonly string Script = GetResource("htmx-toolkit.min.js");

    /// <summary>
    /// The content hash of the HTMX toolkit script.
    /// </summary>
    public static readonly string Hash = Convert
        .ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(DebugScript)).AsSpan(0, 8))
        .ToLowerInvariant();

    private static string GetResource(string name)
    {
        var stream = typeof(HtmxAssets).Assembly.GetManifestResourceStream(name)!;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
