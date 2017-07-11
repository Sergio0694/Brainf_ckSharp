﻿namespace Brainf_ck_sharp_UWP.DataModels.Misc.IDEIndentationGuides
{
    /// <summary>
    /// A simple struct that indicates a bracket and its 2D position inside a plain text
    /// </summary>
    public struct IndentationCoordinateEntry
    {
        /// <summary>
        /// Gets the position of the entry
        /// </summary>
        public Coordinate Position { get; }

        /// <summary>
        /// Gets the current bracket character for the entry
        /// </summary>
        public char Bracket { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="position">The bracket coordinates</param>
        /// <param name="bracket">The bracket character</param>
        public IndentationCoordinateEntry(Coordinate position, char bracket)
        {
            Position = position;
            Bracket = bracket;
        }
    }
}