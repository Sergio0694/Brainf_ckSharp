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
        private readonly Brainf_ckEditBox @this;

        /// <summary>
        /// Creates a new <see cref="FormattingLock"/> instance with the specified parameters
        /// </summary>
        /// <param name="this">The current <see cref="Brainf_ckEditBox"/> instance</param>
        private FormattingLock(Brainf_ckEditBox @this)
        {
            this.@this = @this;

            @this.TextChanging -= @this.MarkdownRichEditBox_TextChanging;
            ((RichEditBox)@this).TextChanged -= @this.MarkdownRichEditBox_TextChanged;

            _ = @this.Document.BatchDisplayUpdates();

            @this.IsUndoGroupingEnabled = true;
        }

        /// <summary>
        /// Creates a new <see cref="FormattingLock"/> instance for a target <see cref="Brainf_ckEditBox"/>
        /// </summary>
        /// <param name="this">The current <see cref="Brainf_ckEditBox"/> instance</param>
        /// <returns>A new <see cref="FormattingLock"/> instance targeting <paramref name="this"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FormattingLock For(Brainf_ckEditBox @this)
        {
            return new(@this);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this.@this.TextChanging += this.@this.MarkdownRichEditBox_TextChanging;
            ((RichEditBox)this.@this).TextChanged += this.@this.MarkdownRichEditBox_TextChanged;

            _ = this.@this.Document.ApplyDisplayUpdates();

            this.@this.IsUndoGroupingEnabled = false;

            // Redraw the overlays, if needed
            this.@this.TryUpdateBracketsList();

            if (this.@this.RenderWhitespaceCharacters)
            {
                this.@this.TryUpdateWhitespaceCharactersList();
            }

            this.@this.TryProcessErrorCoordinate();

            // Notify external subscribers
            this.@this.FormattingCompleted?.Invoke(this.@this, EventArgs.Empty);

            this.@this.textOverlaysCanvas!.Invalidate();
        }
    }
}
