using System;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// A class that contains the indentation info on a new function declaration
    /// </summary>
    public sealed class IDEIndentationFunctionBracketInfo : IDEIndentationLineInfo
    {
        /// <summary>
        /// Gets whether or not the function declaration is nested in a loop
        /// </summary>
        public bool Nested { get; }

        /// <summary>
        /// Creates a new instance for an open function body
        /// </summary>
        /// <param name="nested">Indicates whether or not the function declaration is nested</param>
        /// <param name="type">The type of indentation indicator to display</param>
        public IDEIndentationFunctionBracketInfo(bool nested, IDEIndentationInfoLineType type) : base(type)
        {
            if (type != IDEIndentationInfoLineType.OpenFunctionBracket && type != IDEIndentationInfoLineType.SelfContainedFunction)
            {
                throw new ArgumentOutOfRangeException("Invalid indentation type");
            }
            Nested = nested;
        }

        /// <inheritdoc/>
        public override bool Equals(IDEIndentationLineInfo other)
        {
            return other is IDEIndentationFunctionBracketInfo info &&
                   LineType == info.LineType &&
                   Nested == info.Nested;
        }
    }
}
