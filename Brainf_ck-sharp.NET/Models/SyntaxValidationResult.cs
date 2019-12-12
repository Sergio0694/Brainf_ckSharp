using Brainf_ck_sharp.NET.Enum;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A model that represents the result of a parsing operation on a given source file
    /// </summary>
    public readonly struct SyntaxValidationResult
    {
        /// <summary>
        /// Gets whether or not the input source file has been parsed successfully
        /// </summary>
        public bool IsSuccess => ErrorType == SyntaxError.None;

        /// <summary>
        /// Gets the specific syntax error that caused the source file not to be parsed correctly, if any
        /// </summary>
        public SyntaxError ErrorType { get; }
        
        /// <summary>
        /// Gets the position of the parsing error, if present
        /// </summary>
        public int ErrorOffset { get; }

        /// <summary>
        /// Creates a new <see cref="SyntaxValidationResult"/> instaance with the specified parameters
        /// </summary>
        /// <param name="error">The syntax error for the current source file, if any</param>
        /// <param name="offset">The index of the parsing error, if present</param>
        internal SyntaxValidationResult(SyntaxError error, int offset)
        {
            ErrorType = error;
            ErrorOffset = offset;
        }
    }
}
