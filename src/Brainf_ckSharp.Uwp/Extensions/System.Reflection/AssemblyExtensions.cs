using System.Diagnostics.Contracts;
using System.IO;

#nullable enable

namespace System.Reflection;

/// <summary>
/// An extension <see langword="class"/> for the <see cref="Assembly"/> type
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Returns the contents of a specified manifest file, as a <see cref="string"/>
    /// </summary>
    /// <param name="assembly">The target <see cref="Assembly"/> instance</param>
    /// <param name="path">The path of the file to read</param>
    /// <returns>The text contents of the specified manifest file</returns>
    [Pure]
    public static string GetManifestResourceString(this Assembly assembly, string path)
    {
        using Stream stream = assembly.GetManifestResourceStream(path);
        using StreamReader reader = new(stream);

        return reader.ReadToEnd().Trim();
    }
}
