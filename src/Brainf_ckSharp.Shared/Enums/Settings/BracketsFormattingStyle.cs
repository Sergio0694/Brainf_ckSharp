namespace Brainf_ckSharp.Shared.Enums.Settings
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the formatting style for a pair of brackets
    /// </summary>
    public enum BracketsFormattingStyle
    {
        /// <summary>
        /// Open brackets stay on the same line as previous operators
        /// </summary>
        SameLine,

        /// <summary>
        /// Open brackets always go on a new line, if possible
        /// </summary>
        NewLine
    }
}
