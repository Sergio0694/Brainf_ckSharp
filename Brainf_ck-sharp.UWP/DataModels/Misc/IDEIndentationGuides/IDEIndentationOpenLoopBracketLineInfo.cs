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
        /// Gets the current indicator type
        /// </summary>
        public IDEIndentationInfoOpenLoopBracketType Type { get; }

        /// <summary>
        /// Gets whether or not the current indicator is nested in another brackets pair
        /// </summary>
        public bool Nested => Depth > 1 ||
                              Type == IDEIndentationInfoOpenLoopBracketType.InFunction ||
                              Type == IDEIndentationInfoOpenLoopBracketType.Nested;

        /// <summary>
        /// Creates a new instance for a given loop depth
        /// </summary>
        /// <param name="depth">The depth of the new loop bracket</param>
        /// <param name="selfContained">Indicates whether or not the brackets pair is closed on the same line</param>
        /// <param name="type">Gets the current indicator type</param>
        public IDEIndentationOpenLoopBracketLineInfo(uint depth, bool selfContained, IDEIndentationInfoOpenLoopBracketType type) 
            : base(selfContained ? IDEIndentationInfoLineType.SelfContainedLoop : IDEIndentationInfoLineType.OpenLoopBracket)
        {
            Depth = depth;
            Type = type;
        }

        /// <inheritdoc/>
        public override bool Equals(IDEIndentationLineInfo other)
        {
            return other is IDEIndentationOpenLoopBracketLineInfo info &&
                   LineType == info.LineType &&
                   Depth == info.Depth &&
                   Type == info.Type;
        }
    }
}
