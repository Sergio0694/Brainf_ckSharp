using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    /// <summary>
    /// A <see langword="ref"/> <see langword="struct"/> that enumerates the items in a given <see cref="ReadOnlySpan{T}"/> instance
    /// </summary>
    /// <typeparam name="T">The type of items to enumerate</typeparam>
    public readonly ref struct ReadOnlySpanEnumerator<T>
    {
        /// <summary>
        /// The source <see cref="ReadOnlySpan{T}"/> instance
        /// </summary>
        private readonly ReadOnlySpan<T> Span;

        /// <summary>
        /// Creates a new <see cref="ReadOnlySpanEnumerator{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="span">The source <see cref="ReadOnlySpan{T}"/> to enumerate</param>
        public ReadOnlySpanEnumerator(ReadOnlySpan<T> span)
        {
            Span = span;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(Span);

        /// <summary>
        /// An enumerator for a source <see cref="ReadOnlySpan{T}"/> instance
        /// </summary>
        public ref struct Enumerator
        {
            /// <summary>
            /// The target <see cref="ReadOnlySpan{T}"/> instance
            /// </summary>
            private readonly ReadOnlySpan<T> Span;

            /// <summary>
            /// The current index
            /// </summary>
            private int _Index;

            /// <summary>
            /// Creates a new <see cref="Enumerator"/> instance with the specified parameters
            /// </summary>
            /// <param name="span">The source <see cref="ReadOnlySpan{T}"/> instance</param>
            public Enumerator(ReadOnlySpan<T> span)
            {
                Span = span;
                _Index = -1;
            }

            /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int index = _Index + 1;

                if (index < Span.Length)
                {
                    _Index = index;

                    return true;
                }

                return false;
            }

            /// <inheritdoc cref="IEnumerator{T}.Current"/>
            public (int Index, T Value) Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    int index = _Index;
                    T value = Unsafe.Add(ref MemoryMarshal.GetReference(Span), index);

                    return (index, value);
                }
            }
        }
    }
}
