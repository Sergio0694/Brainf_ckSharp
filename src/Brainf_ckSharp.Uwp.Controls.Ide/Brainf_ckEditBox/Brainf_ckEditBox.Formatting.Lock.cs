using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

#nullable enable

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
                ((RichEditBox)@this).TextChanged -= @this.MarkdownRichEditBox_TextChanged;
                @this.Document.BatchDisplayUpdates();
                @this.IsUndoGroupingEnabled = true;
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
                ((RichEditBox)This).TextChanged += This.MarkdownRichEditBox_TextChanged;
                This.Document.ApplyDisplayUpdates();
                This.IsUndoGroupingEnabled = false;

                // Redraw the overlays, if needed
                This.TryUpdateBracketsList();
                if (This.RenderWhitespaceCharacters) This.TryUpdateWhitespaceCharactersList();
                This.TryProcessErrorCoordinate();

                This._TextOverlaysCanvas!.Invalidate();
            }
        }
    }
}
