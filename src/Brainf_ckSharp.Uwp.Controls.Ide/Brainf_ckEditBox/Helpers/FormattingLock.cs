using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// A helper <see langword="ref"/> <see langword="struct"/> that pauses UI updates when text formatting is performed
        /// </summary>
        private readonly ref struct FormattingLock
        {
            /// <summary>
            /// The current <see cref="Brainf_ckEditBox"/> instance
            /// </summary>
            private readonly Brainf_ckEditBox This;

            /// <summary>
            /// Creates a new <see cref="FormattingLock"/> instance with the specified parameters
            /// </summary>
            /// <param name="this">The current <see cref="Brainf_ckEditBox"/> instance</param>
            private FormattingLock(Brainf_ckEditBox @this)
            {
                This = @this;

                @this.TextChanging -= @this.MarkdownRichEditBox_TextChanging;
                @this.Document.BatchDisplayUpdates();
            }

            /// <summary>
            /// Creates a new <see cref="FormattingLock"/> instance for a target <see cref="Brainf_ckEditBox"/>
            /// </summary>
            /// <param name="this">The current <see cref="Brainf_ckEditBox"/> instance</param>
            /// <returns>A new <see cref="FormattingLock"/> instance targeting <paramref name="this"/></returns>
            [Pure]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FormattingLock For(Brainf_ckEditBox @this) => new FormattingLock(@this);

            /// <inheritdoc cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                This.TextChanging += This.MarkdownRichEditBox_TextChanging;
                This.Document.ApplyDisplayUpdates();
            }
        }
    }
}
