using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide;

internal sealed partial class Brainf_ckEditBox
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
            this.This = @this;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FormattingLock For(Brainf_ckEditBox @this) => new(@this);

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this.This.TextChanging += this.This.MarkdownRichEditBox_TextChanging;
            ((RichEditBox)this.This).TextChanged += this.This.MarkdownRichEditBox_TextChanged;
            this.This.Document.ApplyDisplayUpdates();
            this.This.IsUndoGroupingEnabled = false;

            // Redraw the overlays, if needed
            this.This.TryUpdateBracketsList();
            if (this.This.RenderWhitespaceCharacters) this.This.TryUpdateWhitespaceCharactersList();
            this.This.TryProcessErrorCoordinate();

            // Notify external subscribers
            this.This.FormattingCompleted?.Invoke(this.This, EventArgs.Empty);

            this.This._TextOverlaysCanvas!.Invalidate();
        }
    }
}
