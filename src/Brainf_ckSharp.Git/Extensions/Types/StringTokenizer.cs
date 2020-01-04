using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// A <see langword="ref"/> <see langword="struct"/> that tokenizes a given <see cref="string"/> instance
    /// </summary>
    public readonly ref struct StringTokenizer
    {
        /// <summary>
        /// The target text to segment
        /// </summary>
        private readonly string Text;

        /// <summary>
        /// The separator character to use
        /// </summary>
        private readonly char Separator;

        /// <summary>
        /// Creates a new <see cref="StringTokenizer"/> instance with the specified parameters
        /// </summary>
        /// <param name="text">The target text to tokenize</param>
        /// <param name="separator">The separator character to use</param>
        public StringTokenizer(string text, char separator)
        {
            Text = text;
            Separator = separator;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(Text.AsSpan(), Separator);

        /// <summary>
        /// An enumerator for no-allocation substrings
        /// </summary>
        public ref struct Enumerator
        {
            /// <summary>
            /// The target <see cref="ReadOnlySpan{T}"/> instance
            /// </summary>
            private readonly ReadOnlySpan<char> Span;

            /// <summary>
            /// The separator character to use
            /// </summary>
            private readonly char Separator;

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
            /// <param name="separator">The sepaarator character to use</param>
            public Enumerator(ReadOnlySpan<char> span, char separator)
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
                if (end < length)
                {
                    _Start = end;

                    int index = Span.Slice(end).IndexOf(Separator);

                    // Extract the current subsequence
                    if (index > 0)
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
            public ReadOnlySpan<char> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Span.Slice(_Start, _End - _Start);
            }
        }
    }
}
