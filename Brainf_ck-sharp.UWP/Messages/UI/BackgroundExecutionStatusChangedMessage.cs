using Brainf_ck_sharp.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.UI
{
    /// <summary>
    /// A message that signals the result of a script run in the background
    /// </summary>
    public sealed class BackgroundExecutionStatusChangedMessage
    {
        /// <summary>
        /// Gets whether or not the script was run successfully
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the stdout result for the executed script
        /// </summary>
        [NotNull]
        public InterpreterResult Result { get; }

        /// <summary>
        /// Creates a new message with the specified arguments
        /// </summary>
        /// <param name="success">The success result for the script</param>
        /// <param name="stdout">The stdout output</param>
        public BackgroundExecutionStatusChangedMessage(InterpreterResult result) => Result = result;
    }
}
