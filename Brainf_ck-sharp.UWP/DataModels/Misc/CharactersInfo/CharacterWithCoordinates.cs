namespace Brainf_ck_sharp_UWP.DataModels.Misc.CharactersInfo
{
    /// <summary>
    /// A simple struct that indicates a character and its 2D position inside a plain text
    /// </summary>
    public struct CharacterWithCoordinates
    {
        /// <summary>
        /// Gets the position of the entry
        /// </summary>
        public Coordinate Position { get; }

        /// <summary>
        /// Gets the current character for the entry
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="position">The character coordinates</param>
        /// <param name="char">The current character</param>
        public CharacterWithCoordinates(Coordinate position, char @char)
        {
            Position = position;
            Character = @char;
        }
    }
}