using System.Diagnostics.Contracts;
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
            string message = SyntaxErrorConverter.Convert(result.ErrorType);

            return $"{message}, operator {result.ErrorOffset}";
        }
    }
}
