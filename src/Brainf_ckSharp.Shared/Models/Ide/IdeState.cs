namespace Brainf_ckSharp.Shared.Models.Ide
{
    /// <summary>
    /// A model representing a workspace state to be serialized and deserialized
    /// </summary>
    public sealed class IdeState
    {
        /// <summary>
        /// Gets or sets the path of the file in use, if present
        /// </summary>
        public string? FilePath { get; set; }

        /// <summary>
        /// Gets or sets the text currently displayed
        /// </summary>
        public string Text { get; set; } = null!;

        /// <summary>
        /// Gets the current row in the document in use
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets the current column in the document in use
        /// </summary>
        public int Column { get; set; }
    }
}
