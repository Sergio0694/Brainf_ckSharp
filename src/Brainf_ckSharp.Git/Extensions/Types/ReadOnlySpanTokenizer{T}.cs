using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="ref"/> <see langword="struct"/> that tokenizes a given <see cref="ReadOnlySpan{T}"/> instance
    /// </summary>
    /// <typeparam name="T">The type of items to tokenize</typeparam>
    public readonly ref struct ReadOnlySpanTokenizer<T> where T : IEquatable<T>
    {
        /// <summary>
        /// The target <see cref="ReadOnlySpan{T}"/> instance
        /// </summary>
        private readonly ReadOnlySpan<T> Span;

        /// <summary>
        /// The separator <typeparamref name="T"/> item to use
        /// </summary>
        private readonly T Separator;

        /// <summary>
        /// Creates a new <see cref="ReadOnlySpanTokenizer{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="span">The target <see cref="ReadOnlySpan{T}"/> to tokenize</param>
        /// <param name="separator">The separator <typeparamref name="T"/> item to use</param>
        public ReadOnlySpanTokenizer(ReadOnlySpan<T> span, T separator)
        {
            Span = span;
            Separator = separator;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(Span, Separator);

        /// <summary>
        /// An enumerator for no-allocation substrings
        /// </summary>
        public ref struct Enumerator
        {
            /// <summary>
            /// The target <see cref="ReadOnlySpan{T}"/> instance
            /// </summary>
            private readonly ReadOnlySpan<T> Span;

            /// <summary>
            /// The separator item to use
            /// </summary>
            private readonly T Separator;

            /// <summary>
            /// The current initial offset
            /// </summary>
            private int _Start;

            /// <summary>
            /// The current final offset
            /// </summary>
            private int _End;

            /// <summary>
            /// Creates a new <see cref="Enumerator"/> instance with the specified parameters
            /// </summary>
            /// <param name="span">The input <see cref="ReadOnlySpan{T}"/> instance</param>
            /// <param name="separator">The separator item to use</param>
            public Enumerator(ReadOnlySpan<T> span, T separator)
            {
                Span = span;
                Separator = separator;
                _Start = 0;
                _End = -1;
            }

            /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext"/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int
                    end = _End + 1,
                    length = Span.Length;

                // Additional check if the separator is not the last character
                if (end <= length)
                {
                    _Start = end;

                    int index = Span.Slice(end).IndexOf(Separator);

                    // Extract the current subsequence
                    if (index >= 0)
                    {
                        _End = end + index;

                        return true;
                    }

                    _End = length;

                    return true;
                }

                return false;
            }

            /// <inheritdoc cref="IEnumerator{T}.Current"/>
            public ReadOnlySpan<T> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Span.Slice(_Start, _End - _Start);
            }
        }
    }
}
