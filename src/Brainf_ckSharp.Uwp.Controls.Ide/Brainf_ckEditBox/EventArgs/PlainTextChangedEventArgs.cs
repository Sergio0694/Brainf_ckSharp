using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Uwp.Controls.Ide.EventArgs
{
    /// <summary>
    /// A <see langword="class"/> that contains info to signal plain text changes
    /// </summary>
    public sealed class PlainTextChangedEventArgs
    {
        /// <summary>
        /// Gets the currently displayed plain text
        /// </summary>
        public string PlainText { get; }

        /// <summary>
        /// Gets the <see cref="SyntaxValidationResult"/> instance for the currently displayed text
        /// </summary>
        public SyntaxValidationResult ValidationResult { get; }

        /// <summary>
        /// Creates a new <see cref="PlainTextChangedEventArgs"/> instance with the specified parameters
        /// </summary>
        /// <param name="plainText">The currently displayed plain text</param>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance for the currently displayed text</param>
        internal PlainTextChangedEventArgs(string plainText, SyntaxValidationResult validationResult)
        {
            PlainText = plainText;
            ValidationResult = validationResult;
        }
    }
}
