using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Models.Console.Interfaces;

namespace Brainf_ckSharp.Shared.Models.Console
{
    /// <summary>
    /// A model for a console syntax error being produced while parsing a script
    /// </summary>
    public sealed class ConsoleSyntaxError : IConsoleEntry
    {
        /// <summary>
        /// Creates a new <see cref="ConsoleSyntaxError"/> instance with the specified parameters
        /// </summary>
        /// <param name="result">The <see cref="SyntaxValidationResult"/> instance for the executed command</param>
        public ConsoleSyntaxError(SyntaxValidationResult result) => Result = result;

        /// <summary>
        /// Gets the exit code for the executed command
        /// </summary>
        public SyntaxValidationResult Result { get; }
    }
}
