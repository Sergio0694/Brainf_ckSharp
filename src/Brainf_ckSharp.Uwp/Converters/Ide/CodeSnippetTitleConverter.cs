using System.Diagnostics.Contracts;
using Brainf_ckSharp.Shared.Constants;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Uwp;

namespace Brainf_ckSharp.Uwp.Converters.Ide;

/// <summary>
/// A <see langword="class"/> with a converter for titles of code snippets
/// </summary>
public static class CodeSnippetTitleConverter
{
    /// <summary>
    /// Converts a given code snippet into its title
    /// </summary>
    /// <param name="snippet">The input code snippet</param>
    /// <returns>The title for <paramref name="snippet"/></returns>
    [Pure]
    public static string Convert(string snippet)
    {
        return snippet switch
        {
            CodeSnippets.ResetCell => "CodeSnippets/ResetCell".GetLocalized(),
            CodeSnippets.DuplicateValue => "CodeSnippets/DuplicateValue".GetLocalized(),
            CodeSnippets.IfZeroThen => "if (x == 0) { }",
            CodeSnippets.IfGreaterThanZeroThenElse => "if (x > 0) { } else { }",
            _ => ThrowHelper.ThrowArgumentException<string>(nameof(snippet), "Invalid target code snippet")
        };
    }
}
