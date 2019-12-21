using System;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ckSharp.Legacy.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc
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
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance for an exception that was generated
        /// </summary>
        /// <param name="type">The exception type</param>
        /// <param name="message">The info message for the exception</param>
        private ScriptExceptionInfo(ScriptExceptionType type, [NotNull] string message)
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
            if (result.ExitCode.HasFlag(InterpreterExitCode.SyntaxError))
            {
                return new ScriptExceptionInfo(ScriptExceptionType.SyntaxError, LocalizationManager.GetResource("SyntaxError"));
            }

            // Interpreter error
            if (result.ExitCode.HasFlag(InterpreterExitCode.InternalException))
            {
                return new ScriptExceptionInfo(ScriptExceptionType.InternalError, LocalizationManager.GetResource("InterpreterError"));
            }

            // Possible infinite loop
            if (result.ExitCode.HasFlag(InterpreterExitCode.ThresholdExceeded))
            {
                return new ScriptExceptionInfo(ScriptExceptionType.ThresholdExceeded, LocalizationManager.GetResource("ThresholdExceeded"));
            }

            // Handled exception
            if (result.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown))
            {
                
                string message;
                if (result.ExitCode.HasFlag(InterpreterExitCode.LowerBoundExceeded)) message = LocalizationManager.GetResource("ExLowerBound");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.UpperBoundExceeded)) message = LocalizationManager.GetResource("ExUpperBound");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.NegativeValue)) message = LocalizationManager.GetResource("ExNegativeValue");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.MaxValueExceeded)) message = LocalizationManager.GetResource("ExMaxValue");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.StdinBufferExhausted)) message = LocalizationManager.GetResource("ExEmptyStdin");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.StdoutBufferLimitExceeded)) message = LocalizationManager.GetResource("ExMaxStdout");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.UndefinedFunctionCalled)) message = LocalizationManager.GetResource("ExUndefinedFunction");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.DuplicateFunctionDefinition)) message = LocalizationManager.GetResource("ExDuplicateFunctionDefinition");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.FunctionsLimitExceeded)) message = LocalizationManager.GetResource("ExFunctionsLimitExceeded");
                else if (result.ExitCode.HasFlag(InterpreterExitCode.StackLimitExceeded)) message = LocalizationManager.GetResource("ExStackLimitExceeded");
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
