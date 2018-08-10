using Brainf_ck_sharp_UWP.DataModels.SQLite;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.IDEStatus
{
    /// <summary>
    /// A message that signals whenever the code the user is working on changes
    /// </summary>
    public sealed class WorkingSourceCodeChangedMessage
    {
        /// <summary>
        /// The current code the user is working on
        /// </summary>
        [CanBeNull]
        public CategorizedSourceCode Code { get; }

        /// <summary>
        /// Initializes a new message for the given code
        /// </summary>
        /// <param name="code">The code selected by the user</param>
        public WorkingSourceCodeChangedMessage([CanBeNull] CategorizedSourceCode code) => Code = code;
    }
}
