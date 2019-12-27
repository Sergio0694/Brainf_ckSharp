using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JetBrains.Annotations;

namespace Brainf_ckSharp.UWP.Controls.IDE
{
    /// <summary>
    /// A custom <see cref="RichEditBox"/> to work with markdown text
    /// </summary>
    public sealed partial class Brainf_ckEditBox : RichEditBox
    {
        // The default character style for the document in use
        [CanBeNull]
        private ITextCharacterFormat _DefaultCharacterFormat;

        public Brainf_ckEditBox()
        {
            TextChanging += MarkdownRichEditBox_TextChanging;
            TextChanged += MarkdownRichEditBox_TextChanged;
        }

        // Checks when the text changes and applies the syntax highlight
        private void MarkdownRichEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            if (!args.IsContentChanging) return;

            // Notify of the updated text content
            string text = Document.GetText();

            // Reset the base foreground color
            IsUndoGroupingEnabled = true;
            Document.BatchDisplayUpdates();

            // Syntax highlight
            _DefaultCharacterFormat = Document.GetDefaultCharacterFormat();
            ApplySyntaxHighlight(text);

            // Apply the changes
            Document.ApplyDisplayUpdates();
        }

        private bool _IsUndoGroupingEnabled;

        /// <summary>
        /// Gets or sets whether or not undo grouping should be enabled
        /// </summary>
        private bool IsUndoGroupingEnabled
        {
            set
            {
                if (_IsUndoGroupingEnabled != value)
                {
                    _IsUndoGroupingEnabled = value;
                    if (value) Document.BeginUndoGroup();
                    else Document.EndUndoGroup();
                }
            }
        }

        // Ends the undo grouping when the text is finally changed
        private void MarkdownRichEditBox_TextChanged(object sender, RoutedEventArgs e) => IsUndoGroupingEnabled = false;
    }
}
