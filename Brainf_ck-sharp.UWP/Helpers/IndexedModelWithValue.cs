using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A thin wrapper for items in a bindable collection that keep a reference to their position
    /// </summary>
    /// <typeparam name="T">The type of the wrapped item</typeparam>
    public class IndexedModelWithValue<T> : ViewModelBase
    {
        /// <summary>
        /// Creates a new instance with the given values
        /// </summary>
        /// <param name="value">The value to wrap</param>
        /// <param name="index">The position of the new value</param>
        public IndexedModelWithValue(T value, int index)
        {
            Index = index;
            Value = value;
        }

        private int _Index;

        /// <summary>
        /// Gets the index of the current instance
        /// </summary>
        public int Index
        {
            get { return _Index; }
            private set => Set(ref _Index, value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(Index)));
        }

        /// <summary>
        /// Gets the actual value wrapped by this instance
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Creates a new indexed collection from the given one, using lazy evaluation
        /// </summary>
        /// <param name="source">The source items list</param>
        [CollectionAccess(CollectionAccessType.Read)]
        [LinqTunnel]
        [NotNull]
        public static IEnumerable<IndexedModelWithValue<T>> New([NotNull] IEnumerable<T> source)
        {
            int index = 0;
            foreach (T item in source)
            {
                yield return new IndexedModelWithValue<T>(item, index);
                index++;
            }
        }
    }
}
