using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite.Enums;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite
{
    /// <summary>
    /// A small wrapper class for a <see cref="SourceCode"/> object and its <see cref="SavedSourceCodeType"/> category
    /// </summary>
    public class CategorizedSourceCode
    {
        /// <summary>
        /// Gets the current category for the saved source code
        /// </summary>
        public SavedSourceCodeType Type { get; }

        /// <summary>
        /// Gets the current categorized source code saved in the app
        /// </summary>
        [NotNull]
        public SourceCode Code { get; }

        /// <summary>
        /// Creates a new instance that wraps a saved source code
        /// </summary>
        /// <param name="type">The category for the source code to wrap</param>
        /// <param name="code">The current source code</param>
        public CategorizedSourceCode(SavedSourceCodeType type, [NotNull] SourceCode code)
        {
            Type = type;
            Code = code;
        }
    }
}