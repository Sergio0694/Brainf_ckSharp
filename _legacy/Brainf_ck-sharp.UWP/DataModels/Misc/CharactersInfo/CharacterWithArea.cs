using Windows.Foundation;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.CharactersInfo
{
    /// <summary>
    /// A simple struct that indicates a character and its 2D area inside a plain text
    /// </summary>
    public struct CharacterWithArea
    {
        /// <summary>
        /// Gets the area of the entry
        /// </summary>
        public Rect Area { get; }

        /// <summary>
        /// Gets the current character for the entry
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="area">The character area</param>
        /// <param name="char">The current character</param>
        public CharacterWithArea(Rect area, char @char)
        {
            Area = area;
            Character = @char;
        }
    }
}