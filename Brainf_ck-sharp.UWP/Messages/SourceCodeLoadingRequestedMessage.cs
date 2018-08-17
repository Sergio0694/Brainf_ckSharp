using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Enums;
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
        /// Gets the source for the requested source code
        /// </summary>
        public SavedCodeLoadingSource Source { get; }

        /// <summary>
        /// Initializes a new message for the given code
        /// </summary>
        /// <param name="code">The code selected by the user</param>
        /// <param name="source">The source of the requested code</param>
        public SourceCodeLoadingRequestedMessage([NotNull] CategorizedSourceCode code, SavedCodeLoadingSource source)
        {
            RequestedCode = code;
            Source = source;
        }
    }
}
