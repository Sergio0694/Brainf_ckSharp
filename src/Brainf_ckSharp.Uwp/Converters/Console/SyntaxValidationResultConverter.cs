using System.Diagnostics.Contracts;
using Windows.ApplicationModel.Resources;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Microsoft.Toolkit.Uwp.Extensions;

namespace Brainf_ckSharp.Uwp.Converters.Console
{
    /// <summary>
    /// A <see langword="class"/> with a collection of helper functions displaying syntax errors
    /// </summary>
    public static class SyntaxValidationResultConverter
    {
        /// <summary>
        /// The <see cref="Windows.ApplicationModel.Resources.ResourceLoader"/> instance to retrieve localized text from
        /// </summary>
        /// <remarks>
        /// The controls project already includes the strings for <see cref="SyntaxError"/>, as they're displayed in the IDE
        /// </remarks>
        private static readonly ResourceLoader ResourceLoader = ResourceLoader.GetForViewIndependentUse("Brainf_ckSharp.Uwp.Controls.Ide/Resources");

        /// <summary>
        /// The "operator" localized text
        /// </summary>
        private static readonly string Operator = "SyntaxValidationResult/Operator".GetLocalized();

        /// <summary>
        /// Converts a given <see cref="SyntaxValidationResult"/> instance to its representation
        /// </summary>
        /// <param name="result">The input <see cref="SyntaxValidationResult"/> instance to format</param>
        /// <returns>A <see cref="string"/> representing the input <see cref="SyntaxValidationResult"/> instance</returns>
        [Pure]
        public static string Convert(SyntaxValidationResult result)
        {
            string message = ResourceLoader.GetString($"{nameof(SyntaxError)}/{result.ErrorType}");

            return $"{message}, {Operator} {result.ErrorOffset}";
        }
    }
}
