using System.Diagnostics;

namespace Brainf_ckSharp.Git.Models
{
    /// <summary>
    /// A model that represents a given line in a document being inspected
    /// </summary>
    [DebuggerDisplay("({NumberOfOccurrencesInOldText}, {NumberOfOccurrencesInNewText}, {LineNumberInOldText})")]
    internal sealed class DiffEntry
    {
        /// <summary>
        /// The number of occurrences for the current entry in the reference text
        /// </summary>
        public int NumberOfOccurrencesInOldText;

        /// <summary>
        /// The number of occurrences for the current entry in the updated text
        /// </summary>
        public int NumberOfOccurrencesInNewText;

        /// <summary>
        /// The line number for the current entry in the reference text
        /// </summary>
        public int LineNumberInOldText;
    }
}
