using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Brainf_ckEditBox
{
    /// <summary>
    /// A custom <see cref="RichEditBox"/> that formats Brainf*ck/PBrain code
    /// </summary>
    public sealed partial class Brainf_ckEditBox : RichEditBox
    {
        /// <summary>
        /// Creates a new <see cref="Brainf_ckEditBox"/> instance
        /// </summary>
        public Brainf_ckEditBox()
        {
            SelectionChanging += Brainf_ckEditBox_SelectionChanging;
            TextChanging += MarkdownRichEditBox_TextChanging;
            TextChanged += MarkdownRichEditBox_TextChanged;
        }

        private void Brainf_ckEditBox_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs args)
        {
            _SelectionLength = args.SelectionLength;
            _SelectionStart = args.SelectionStart;
        }

        // Checks when the text changes and applies the syntax highlight
        private void MarkdownRichEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            if (!args.IsContentChanging) return;

            // Batch updates
            IsUndoGroupingEnabled = true;

            // Syntax highlight
            ApplySyntaxHighlight();
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
                    if (value)
                    {
                        Document.BatchDisplayUpdates();
                        Document.BeginUndoGroup();
                    }
                    else
                    {
                        Document.EndUndoGroup();
                        Document.ApplyDisplayUpdates();
                    }
                }
            }
        }

        // Ends the undo grouping when the text is finally changed
        private void MarkdownRichEditBox_TextChanged(object sender, RoutedEventArgs e) => IsUndoGroupingEnabled = false;

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            bool ctrl = (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            IsUndoGroupingEnabled = true;
            if (ctrl && e.Key == VirtualKey.Z && Document.CanUndo())
            {
                Document.Undo(); // Fix for incorrect grouping when pressing CTRL + Z
            }

            base.OnKeyDown(e);
        }
    }
}
