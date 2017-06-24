namespace Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// A class that wraps some info for an IDE code line that contains an open bracket
    /// </summary>
    public class IDEIndentationOpenBracketLineInfo : IDEIndentationLineInfo
    {
        /// <summary>
        /// Gets the depth of the current open bracket
        /// </summary>
        public uint Depth { get; }

        /// <summary>
        /// Creates a new instance for a given loop depth
        /// </summary>
        /// <param name="depth">The depth of the new bracket</param>
        public IDEIndentationOpenBracketLineInfo(uint depth) : base(IDEIndentationInfoLineType.OpenBracket)
        {
            Depth = depth;
        }
    }
}
