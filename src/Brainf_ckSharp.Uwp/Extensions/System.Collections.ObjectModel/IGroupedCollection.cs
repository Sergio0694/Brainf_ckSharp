namespace Brainf_ckSharp.Uwp.Extensions.System.Collections.ObjectModel
{
    /// <summary>
    /// An <see langword="interface"/> for a grouped collection of items
    /// </summary>
    public interface IGroupedCollection
    {
        /// <summary>
        /// Gets the key for the current collection, as an <see cref="object"/>
        /// </summary>
        object Key { get; }

        /// <summary>
        /// Gets the number of items currently in the grouped collection
        /// </summary>
        int Count { get; }
    }
}
