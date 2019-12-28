﻿using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;

namespace Brainf_ckSharp.Uwp.Controls.Ide
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
            Paste += Brainf_ckEditBox_Paste;
        }

        // Handles text being pasted by the user
        private async void Brainf_ckEditBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = true;

            // Insert the text if there is some available
            if (await ClipboardHelper.TryGetTextAsync() is { } text)
            {
                InsertText(text);
            }
        }

        // Updates the length of the current selection
        private void Brainf_ckEditBox_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs args)
        {
            _SelectionLength = args.SelectionLength;
        }

        // Checks when the text changes and applies the syntax highlight
        private void MarkdownRichEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
        {
            if (!args.IsContentChanging) return;

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
                    if (value) Document.BeginUndoGroup();
                    else Document.EndUndoGroup();
                }
            }
        }

        // Ends the undo grouping when the text is finally changed
        private void MarkdownRichEditBox_TextChanged(object sender, RoutedEventArgs e) => IsUndoGroupingEnabled = false;

        /// <summary>
        /// The list of shortcut keys to ignore
        /// </summary>
        private readonly HashSet<VirtualKey> IgnoredShortcuts = new HashSet<VirtualKey>(new[]
        {
            VirtualKey.E, VirtualKey.R, // Indent right
            VirtualKey.J,               // Can't remember now
            VirtualKey.L,               // Reset indent
            VirtualKey.B,               // Bold
            VirtualKey.I,               // Italic, or TAB
            VirtualKey.U                // Underline
        });

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            // Disable the unwanted shortcuts
            bool ctrl = VirtualKey.Control.IsDown();
            if (ctrl && IgnoredShortcuts.Contains(e.Key))
            {
                goto HandleAndReturn;
            }

            IsUndoGroupingEnabled = true;

            /* CTRL + Z triggers the undo operation. In this case, first
             * an undo is programmatically request, in order to fix the
             * incorrect undo grouping that is generated by the custom
             * syntaax highlight. The event is not handled, so that when
             * the handler returns, the control will still perform another
             * undo operation on its own, resulting in the correct
             * number of undo requests for the current text. */
            if (ctrl && e.Key == VirtualKey.Z && Document.CanUndo())
            {
                Document.Undo();

                goto BaseOnKeyDown;
            }

            // Delete/canc keys
            if (e.Key == VirtualKey.Back ||
                e.Key == VirtualKey.Delete)
            {
                _IsDeleteRequested = true;

                goto BaseOnKeyDown;
            }

            // Tab shortcuts
            if (e.Key == VirtualKey.Tab)
            {
                /* There are three possible operations to perform when the
                 * tab key is pressed. If the current selection is up to a
                 * single character, a '\t' is typed. Otherwise, the current
                 * selection is shifted forwards or backwards by adding and
                 * removing tabs depending on whether the shift key is down. */
                if (Math.Abs(Document.Selection.Length) <= 1)
                {
                    Document.Selection.TypeText("\t");
                }
                else if (VirtualKey.Shift.IsDown()) ShiftBackward();
                else ShiftForward();

                goto HandleAndReturn;
            }

            // Manually handle the enter key to avoid \v
            if (e.Key == VirtualKey.Enter)
            {
                Document.Selection.TypeText("\r");

                goto HandleAndReturn;
            }

            BaseOnKeyDown:
            base.OnKeyDown(e);
            return;

            HandleAndReturn:
            e.Handled = true;
        }
    }
}