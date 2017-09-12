namespace Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// A class that wraps some info for an IDE code line that contains an open loop bracket
    /// </summary>
    public class IDEIndentationOpenLoopBracketLineInfo : IDEIndentationLineInfo
    {
        /// <summary>
        /// Gets the depth of the current open loop bracket
        /// </summary>
        public uint Depth { get; }

        /// <summary>
        /// Creates a new instance for a given loop depth
        /// </summary>
        /// <param name="depth">The depth of the new loop bracket</param>
        /// <param name="selfContained">Indicates whether or not the brackets pair is closed on the same line</param>
        public IDEIndentationOpenLoopBracketLineInfo(uint depth, bool selfContained) 
            : base(selfContained ? IDEIndentationInfoLineType.SelfContainedLoop : IDEIndentationInfoLineType.OpenLoopBracket)
        {
            Depth = depth;
        }
    }
}
