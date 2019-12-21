using System;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// A class that indicates the indentation info on a given line in the IDE
    /// </summary>
    public class IDEIndentationLineInfo : IEquatable<IDEIndentationLineInfo>
    {
        /// <summary>
        /// Gets the info type for the current line
        /// </summary>
        public IDEIndentationInfoLineType LineType { get; }

        /// <summary>
        /// Creates a new instance for a given line type
        /// </summary>
        /// <param name="type">The type of the current line</param>
        public IDEIndentationLineInfo(IDEIndentationInfoLineType type) => LineType = type;

        /// <inheritdoc/>
        public virtual bool Equals(IDEIndentationLineInfo other)
        {
            return other.GetType() == typeof(IDEIndentationLineInfo) &&
                   (LineType == IDEIndentationInfoLineType.Straight ||
                    LineType == IDEIndentationInfoLineType.Empty ||
                    LineType == IDEIndentationInfoLineType.ClosedBracket) &&
                   other.LineType == LineType;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this == obj) return true;
            if (obj.GetType() != typeof(IDEIndentationLineInfo)) return false;
            return Equals((IDEIndentationLineInfo)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => (int)LineType;
    }
}
