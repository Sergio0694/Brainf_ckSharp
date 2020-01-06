using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckIde
    {
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeleteCharacter()
        {
            CodeEditBox.DeleteSelectionOrCharacter();
        }
    }
}
