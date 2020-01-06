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
    }
}
