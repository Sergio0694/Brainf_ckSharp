namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// Inserts a given source <see cref="string"/> into the current selection
        /// </summary>
        /// <param name="source">The source text to insert</param>
        public void InsertText(string source)
        {
            CodeEditBox.InsertText(source);
        }
    }
}
