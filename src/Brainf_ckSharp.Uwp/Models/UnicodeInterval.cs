namespace Brainf_ckSharp.Uwp.Models
{
    /// <summary>
    /// A simple model representing a range of unicode characters
    /// </summary>
    public sealed class UnicodeInterval
    {
        /// <summary>
        /// Creates a new <see cref="UnicodeInterval"/> instance with the specified parameters
        /// </summary>
        /// <param name="start">The start of the interval</param>
        /// <param name="end">The end of the interval</param>
        public UnicodeInterval(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the start of the interval
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Gets the end of the interval
        /// </summary>
        public int End { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{Start}, {End}]";
        }
    }
}
