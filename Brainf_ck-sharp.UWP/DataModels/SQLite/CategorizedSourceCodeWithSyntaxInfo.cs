using Brainf_ck_sharp;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.SQLite
{
    /// <summary>
    /// A class that wraps a categorized source code with additional info on its syntax state
    /// </summary>
    public class CategorizedSourceCodeWithSyntaxInfo : CategorizedSourceCode
    {
        /// <summary>
        /// Gets whether or not the syntax is correct for the current code
        /// </summary>
        public bool IsSyntaxValid { get; }

        /// <summary>
        /// Creates a new instance that wraps a saved source code and its syntax info
        /// </summary>
        /// <param name="type">The category for the source code to wrap</param>
        /// <param name="code">The current source code</param>
        public CategorizedSourceCodeWithSyntaxInfo(SavedSourceCodeType type, [NotNull] SourceCode code) : base(type, code)
        {
            (bool valid, _) = Brainf_ckInterpreter.CheckSourceSyntax(code.Code);
            IsSyntaxValid = valid;
        }
    }
}
