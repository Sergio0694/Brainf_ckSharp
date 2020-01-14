using Windows.UI.Text;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
        /// <summary>
        /// The reference text currently in use
        /// </summary>
        private string _ReferenceText = "\r";

        /// <summary>
        /// Loads a given text file and starts using it as reference for the git diff indicators
        /// </summary>
        /// <param name="text"></param>
        public void LoadText(string text)
        {
            _ReferenceText = text;

            CodeEditBox.Document.LoadFromString(text);
        }

        /// <summary>
        /// Marks the currently loaded text file as being saved and updates the git diff indicators
        /// </summary>
        public void MarkTextAsSaved()
        {
            _ReferenceText = CodeEditBox.PlainText;

            UpdateDiffInfo();

            IdeOverlaysCanvas.Invalidate();
        }

        /// <summary>
        /// Types a new character into the current document
        /// </summary>
        /// <param name="character">The character to type</param>
        public void TypeCharacter(char character)
        {
            CodeEditBox.Document.Selection.TypeText(character.ToString());
        }

        /// <summary>
        /// Deletes the last character as if the delete key had been pressed
        /// </summary>
        public void DeleteCharacter()
        {
            CodeEditBox.DeleteSelectionOrCharacter();
        }
    }
}
