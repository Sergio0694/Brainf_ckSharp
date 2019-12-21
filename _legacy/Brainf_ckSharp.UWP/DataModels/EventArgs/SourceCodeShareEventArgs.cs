using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite;
using Brainf_ck_sharp.Legacy.UWP.Enums;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.EventArgs
{
    /// <summary>
    /// The arguments for an event that signals a request to share a saved source code
    /// </summary>
    public sealed class SourceCodeShareEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the requested share method to use
        /// </summary>
        public SourceCodeShareType Type { get; }

        /// <summary>
        /// Gets the source code to share
        /// </summary>
        [CanBeNull]
        public SourceCode Code { get; }

        /// <summary>
        /// Creates a new instance for an event
        /// </summary>
        /// <param name="type">The share method to use</param>
        /// <param name="code">The saved code to share</param>
        public SourceCodeShareEventArgs(SourceCodeShareType type, [CanBeNull] SourceCode code)
        {
            Type = type;
            Code = code;
        }
    }
}
