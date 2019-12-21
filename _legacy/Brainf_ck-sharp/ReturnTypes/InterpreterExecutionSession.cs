using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.ReturnTypes
{
    /// <summary>
    /// A class that represents an execution session for a script
    /// </summary>
    public sealed class InterpreterExecutionSession : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/> used to bypass the breakpoints
        /// </summary>
        [CanBeNull]
        private readonly CancellationTokenSource TokenSource;

        /// <summary>
        /// Gets the <see cref="IEnumerator{T}"/> instance used to get the session results
        /// </summary>
        [NotNull]
        private readonly IEnumerator<InterpreterResult> ResultsEnumerator;

        /// <summary>
        /// Creates a new execution session
        /// </summary>
        /// <param name="enumerator">The <see cref="IEnumerator{T}"/> instance used to get the script results</param>
        /// <param name="source">An optional <see cref="CancellationTokenSource"/> used to signal when to ignore new breakpoints in the script</param>
        internal InterpreterExecutionSession([NotNull] IEnumerator<InterpreterResult> enumerator, [CanBeNull] CancellationTokenSource source)
        {
            ResultsEnumerator = enumerator;
            TokenSource = source;
        }

        #region Public APIs

        /// <inheritdoc/>
        public void Dispose()
        {
            ResultsEnumerator.Dispose();
            TokenSource?.Dispose();
        }

        /// <summary>
        /// Gets the current result of the session
        /// </summary>
        [NotNull]
        public InterpreterResult CurrentResult => ResultsEnumerator.Current;

        /// <summary>
        /// Gets whether or not the instance can continue its execution from its current state
        /// </summary>
        public bool CanContinue => CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                                   CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached);

        /// <summary>
        /// Continues the script execution from the current state
        /// </summary>
        [PublicAPI]
        public void Continue()
        {
            if (!CanContinue) throw new InvalidOperationException("The current session can't be continued");
            if (TokenSource == null) throw new InvalidOperationException("The current session is in an invalid state");
            ResultsEnumerator.MoveNext();
        }

        /// <summary>
        /// Continues the execution of the instance from the current state to the end of the code
        /// </summary>
        [PublicAPI]
        public void RunToCompletion()
        {
            if (!CanContinue) throw new InvalidOperationException("The current session can't continue from this state");
            if (TokenSource == null) throw new InvalidOperationException("The current session is in an invalid state");
            TokenSource.Cancel(); // Canceling the token will cause the interpreter to ignore new breakpoints
            ResultsEnumerator.MoveNext();
        }

        #endregion
    }
}
