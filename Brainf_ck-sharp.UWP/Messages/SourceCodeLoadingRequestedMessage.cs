using Brainf_ck_sharp_UWP.DataModels.SQLite;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages
{
    /// <summary>
    /// A message that signals when the user requests to load a source code
    /// </summary>
    public sealed class SourceCodeLoadingRequestedMessage
    {
        /// <summary>
        /// The selected code to load
        /// </summary>
        [NotNull]
        public CategorizedSourceCode RequestedCode { get; }

        /// <summary>
        /// Initializes a new message for the given code
        /// </summary>
        /// <param name="code">The code selected by the user</param>
        public SourceCodeLoadingRequestedMessage([NotNull] CategorizedSourceCode code) => RequestedCode = code;
    }
}
