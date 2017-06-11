using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// Indicates that the user requested to run the current script being edited
    /// </summary>
    public sealed class PlayScriptMessage
    {
        /// <summary>
        /// Gets the buffer value for the current operation
        /// </summary>
        public String StdinBuffer { get; }

        /// <summary>
        /// Initializes a new instance with the given buffer
        /// </summary>
        /// <param name="buffer">The Stdin buffer to use</param>
        public PlayScriptMessage([NotNull] String buffer) => StdinBuffer = buffer;
    }
}
