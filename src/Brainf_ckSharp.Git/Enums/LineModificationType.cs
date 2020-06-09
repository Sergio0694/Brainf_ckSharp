namespace Brainf_ckSharp.Git.Enums
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the type of modification for a given text line
    /// </summary>
    public enum LineModificationType
    {
        /// <summary>
        /// No changes have been detected for the current line
        /// </summary>
        None,

        /// <summary>
        /// The current line has been modified with respect to the reference text
        /// </summary>
        Modified,

        /// <summary>
        /// The current line has been modified and then saved
        /// </summary>
        Saved
    }
}
