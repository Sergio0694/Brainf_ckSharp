using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Enums;

namespace Brainf_ckSharp.Uwp.Converters.Console
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions displaying syntax errors
    /// </summary>
    public static class SyntaxErrorConverter
    {
        /// <summary>
        /// Converts a given <see cref="SyntaxError"/> instance to its representation
        /// </summary>
        /// <param name="error">The input <see cref="SyntaxError"/> instance to format</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="SyntaxError"/> instance</returns>
        [Pure]
        public static string Convert(SyntaxError error)
        {
            return error switch
            {
                SyntaxError.None => "None",
                SyntaxError.MismatchedSquareBracket => "Mismatched square bracket",
                SyntaxError.IncompleteLoop => "Incomplete loop",
                SyntaxError.MismatchedParenthesis => "Mismatched parenthesis",
                SyntaxError.InvalidFunctionDeclaration => "Invalid function declaration",
                SyntaxError.NestedFunctionDeclaration => "Nested function declaration",
                SyntaxError.EmptyFunctionDeclaration => "Empty function declaration",
                SyntaxError.IncompleteFunctionDeclaration => "Incomplete function declaration",
                SyntaxError.MissingOperators => "Missing operators",
                _ => throw new ArgumentOutOfRangeException($"Invalid syntax error: {error}")
            };
        }
    }
}
