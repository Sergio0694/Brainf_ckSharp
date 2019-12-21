using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ckSharp.Legacy;
using Brainf_ckSharp.Legacy.ReturnTypes;
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
        /// Gets whether or not the IDE is currently opened on this saved code
        /// </summary>
        public bool IsSelected { get; }

        /// <summary>
        /// Creates a new instance that wraps a saved source code and its syntax info
        /// </summary>
        /// <param name="type">The category for the source code to wrap</param>
        /// <param name="code">The current source code</param>
        /// <param name="selected">Indicates whether or not this code is currently opened in the IDE</param>
        public CategorizedSourceCodeWithSyntaxInfo(SavedSourceCodeType type, [NotNull] SourceCode code, bool selected = false) : base(type, code)
        {
            SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(code.Code);
            IsSyntaxValid = result.Valid;
            IsSelected = selected;
        }
    }
}
