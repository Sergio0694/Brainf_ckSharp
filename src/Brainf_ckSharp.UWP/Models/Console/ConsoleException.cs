using Brainf_ckSharp.Enums;
using Brainf_ckSharp.UWP.Models.Console.Interfaces;

namespace Brainf_ckSharp.UWP.Models.Console
{
    /// <summary>
    /// A model for a console exception being thrown by an executed command
    /// </summary>
    public sealed class ConsoleException : IConsoleEntry
    {
        /// <summary>
        /// Creates a new <see cref="ConsoleException"/> instance with the specified parameters
        /// </summary>
        /// <param name="exitCode">The exit code for the executed command</param>
        public ConsoleException(ExitCode exitCode) => ExitCode = exitCode;

        /// <summary>
        /// Gets the exit code for the executed command
        /// </summary>
        public ExitCode ExitCode { get; }
    }
}
