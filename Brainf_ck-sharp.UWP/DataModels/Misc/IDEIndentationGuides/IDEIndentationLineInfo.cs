namespace Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// A class that indicates the indentation info on a given line in the IDE
    /// </summary>
    public class IDEIndentationLineInfo
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
    }
}
