using System.Security.Cryptography;
using System.Text;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Provides access to embedded javascript assets.
/// </summary>
public static class HtmxAssets
{
    /// <summary>
    /// The debug version of the script to integrate with the anti-forgery feature of ASP.NET Core.
    /// </summary>
    public static readonly string DebugScript = GetResource("htmx-toolkit.js");

    /// <summary>
    /// The minified version of the script to integrate with the anti-forgery feature of ASP.NET Core.
    /// </summary>
    public static readonly string Script = GetResource("htmx-toolkit.min.js");

    /// <summary>
    /// The hash of the current javascript.
    /// </summary>
    public static readonly string Hash = Convert
        .ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(DebugScript)))
        .ToLowerInvariant();

    private static string GetResource(string name)
    {
        var stream = typeof(HtmxAssets).Assembly.GetManifestResourceStream(name)!;

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
