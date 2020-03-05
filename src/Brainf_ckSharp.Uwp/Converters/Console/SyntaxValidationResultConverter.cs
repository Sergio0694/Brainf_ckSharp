using System;
using System.Diagnostics.Contracts;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Uwp.Converters.Console
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions displaying syntax errors
    /// </summary>
    public static class SyntaxValidationResultConverter
    {
        /// <summary>
        /// Converts a given <see cref="SyntaxValidationResult"/> instance to its representation
        /// </summary>
        /// <param name="result">The input <see cref="SyntaxValidationResult"/> instance to format</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="SyntaxValidationResult"/> instance</returns>
        [Pure]
        public static string Convert(SyntaxValidationResult result)
        {
            string message = result.ErrorType switch
            {
                SyntaxError.MismatchedSquareBracket => "Mismatched square bracket",
                SyntaxError.IncompleteLoop => "Incomplete loop",
                SyntaxError.MismatchedParenthesis => "Mismatched parenthesis",
                SyntaxError.InvalidFunctionDeclaration => "Invalid function declaration",
                SyntaxError.NestedFunctionDeclaration => "Nested function declaration",
                SyntaxError.EmptyFunctionDeclaration => "Empty function declaration",
                SyntaxError.IncompleteFunctionDeclaration => "Incomplete function declaration",
                SyntaxError.MissingOperators => "Missing operators",
                { } error => throw new ArgumentOutOfRangeException($"Invalid syntax error: {error}")
            };

            return $"{message}, operator {result.ErrorOffset}";
        }
    }
}
