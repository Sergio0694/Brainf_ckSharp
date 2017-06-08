using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.ConsoleModels
{
    /// <summary>
    /// Represents the exception info for a faulted script run in the console
    /// </summary>
    public sealed class ConsoleExceptionResult : ConsoleCommandModelBase
    {
        /// <summary>
        /// Gets the exception type for the current instance
        /// </summary>
        public ConsoleExceptionType ExceptionType { get; }

        /// <summary>
        /// Gets the message for the current exception
        /// </summary>
        [NotNull]
        public String Message { get; }

        /// <summary>
        /// Initializes a new instance for an exception that was generated
        /// </summary>
        /// <param name="type">The exception type</param>
        /// <param name="message">The info message for the exception</param>
        public ConsoleExceptionResult(ConsoleExceptionType type, [NotNull] String message)
        {
            ExceptionType = type;
            Message = message;
        }
    }

    /// <summary>
    /// Indicates the type of exception that was thrown by a script
    /// </summary>
    public enum ConsoleExceptionType
    {
        SyntaxError,
        RuntimeError,
        ThresholdExceeded,
        InternalError
    }
}
