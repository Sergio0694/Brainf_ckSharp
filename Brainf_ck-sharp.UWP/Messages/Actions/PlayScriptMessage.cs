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
        /// Gets the kind of requested operation
        /// </summary>
        public ScriptPlayType Type { get; }

        /// <summary>
        /// Gets the buffer value for the current operation
        /// </summary>
        public String StdinBuffer { get; }

        /// <summary>
        /// Initializes a new instance with the given buffer
        /// </summary>
        /// <param name="type">The kind of requested operation</param>
        /// <param name="buffer">The Stdin buffer to use</param>
        public PlayScriptMessage(ScriptPlayType type, [NotNull] String buffer)
        {
            Type = type;
            StdinBuffer = buffer;
        }
    }

    /// <summary>
    /// Indicates the kind of execution requested
    /// </summary>
    public enum ScriptPlayType
    {
        Default,
        RepeatedCommand
    }
}
