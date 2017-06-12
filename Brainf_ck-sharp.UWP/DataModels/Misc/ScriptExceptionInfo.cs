using System;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.Helpers;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    /// <summary>
    /// Represents the exception info for a faulted script run in the app
    /// </summary>
    public class ScriptExceptionInfo
    {
        /// <summary>
        /// Gets the exception type for the current instance
        /// </summary>
        public ScriptExceptionType ExceptionType { get; }

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
        private ScriptExceptionInfo(ScriptExceptionType type, [NotNull] String message)
        {
            ExceptionType = type;
            Message = message;
        }

        /// <summary>
        /// Creates a new instance from a faulted result
        /// </summary>
        /// <param name="result">The source result to parse</param>
        public static ScriptExceptionInfo FromResult([NotNull] InterpreterResult result)
        {
            // Syntax error
            if (result.HasFlag(InterpreterExitCode.MismatchedParentheses))
            {
                return new ScriptExceptionInfo(ScriptExceptionType.SyntaxError, LocalizationManager.GetResource("WrongBrackets"));
            }

            // Interpreter error
            if (result.HasFlag(InterpreterExitCode.InternalException))
            {
                return new ScriptExceptionInfo(ScriptExceptionType.InternalError, LocalizationManager.GetResource("InterpreterError"));
            }

            // Possible infinite loop
            if (result.HasFlag(InterpreterExitCode.ThresholdExceeded))
            {
                return new ScriptExceptionInfo(ScriptExceptionType.RuntimeError, LocalizationManager.GetResource("ThresholdExceeded"));
            }

            // Handled exception
            if (result.HasFlag(InterpreterExitCode.ExceptionThrown))
            {
                
                String message;
                if (result.HasFlag(InterpreterExitCode.LowerBoundExceeded)) message = LocalizationManager.GetResource("ExLowerBound");
                else if (result.HasFlag(InterpreterExitCode.UpperBoundExceeded)) message = LocalizationManager.GetResource("ExUpperBound");
                else if (result.HasFlag(InterpreterExitCode.NegativeValue)) message = LocalizationManager.GetResource("ExNegativeValue");
                else if (result.HasFlag(InterpreterExitCode.MaxValueExceeded)) message = LocalizationManager.GetResource("ExMaxValue");
                else if (result.HasFlag(InterpreterExitCode.StdinBufferExhausted)) message = LocalizationManager.GetResource("ExEmptyStdin");
                else if (result.HasFlag(InterpreterExitCode.StdoutBufferLimitExceeded)) message = LocalizationManager.GetResource("ExMaxStdout");
                else throw new InvalidOperationException("The interpreter exception type isn't valid");
                return new ScriptExceptionInfo(ScriptExceptionType.RuntimeError, message);
            }
            throw new ArgumentException("The input result isn't valid");
        }
    }

    /// <summary>
    /// Indicates the type of exception that was thrown by a script
    /// </summary>
    public enum ScriptExceptionType
    {
        SyntaxError,
        RuntimeError,
        ThresholdExceeded,
        InternalError
    }
}
