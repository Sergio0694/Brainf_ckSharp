using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Brainf_ckSharp.UWP.Constants;
using Brainf_ckSharp.UWP.Extensions.System;
using Brainf_ckSharp.UWP.Models.Themes;

namespace Brainf_ckSharp.UWP.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to formatted Brainf*ck/PBrain code
    /// </summary>
    public static class Brainf_ckInlineFormatterHelper
    {
        /// <summary>
        /// Gets the zero width space character
        /// </summary>
        public const char ZeroWidthSpace = '\u200B';

        /// <summary>
        /// Gets the value of <see cref="SourceProperty"/> for a given <see cref="Span"/>
        /// </summary>
        /// <param name="element">The input <see cref="Span"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="SourceProperty"/> property for the input <see cref="Span"/> instance</returns>
        public static string GetSource(Span element)
        {
            return element.GetValue(SourceProperty).To<string>();
        }

        /// <summary>
        /// Sets the value of <see cref="SourceProperty"/> for a given <see cref="Span"/>
        /// </summary>
        /// <param name="element">The input <see cref="Span"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="SourceProperty"/> property</param>
        public static void SetSource(Span element, string value)
        {
            element.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck code to a <see cref="Span"/> object
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
            "Source",
            typeof(string),
            typeof(Brainf_ckInlineFormatterHelper),
            new PropertyMetadata(string.Empty, OnSourcePropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="SourceProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the arguments
            Span @this = (Span)d;
            string
                oldValue = (string)e.OldValue,
                newValue = (string)e.NewValue;

            // Skip if the input code is empty
            if (string.IsNullOrEmpty(newValue))
            {
                @this.Inlines.Clear();
                return;
            }

            /* When the new value is a prefix of the previous value,
             * it means that the user has deleted one or more characters
             * from the previous string. In this case we just need to traverse
             * the current inlines collection backwards and remove as many
             * runs or characters as needed. This will avoid having to recompute
             * the entire syntax highlight from scratch for the new text. */
            if (oldValue.Length > newValue.Length && oldValue.StartsWith(newValue))
            {
                /* The difference is doubled because each character has an additional
                 * zero width space character next to it, for better line wrapping */
                int difference = (oldValue.Length - newValue.Length) * 2;

                do
                {
                    // Get the last inline, remove it entirely if needed
                    Run run = (Run)@this.Inlines.LastOrDefault();
                    if (run.Text.Length <= difference)
                    {
                        @this.Inlines.RemoveLast();
                        difference -= run.Text.Length;
                    }
                    else
                    {
                        // Inline too long, only remove the exceeding characters
                        run.Text = run.Text.Substring(0, run.Text.Length - difference);
                        break;
                    }
                } while (difference > 0);

                return;
            }

            // Reuse the existing inlines when the new code is an extension of the previous code
            int start = 0, end = newValue.Length;
            if (newValue.Length > oldValue.Length && newValue.StartsWith(oldValue))
            {
                /* Move the initial offset ahead to skip the prefix characters.
                 * Using a moving starting index saves an entire substring creation. */
                start = oldValue.Length;

                if (@this.Inlines.LastOrDefault() is Run run)
                {
                    // Check how many characters can be inserted into the last existing run
                    int i = start;
                    while (i < end && ThemeInfo.HaveSameColor(run.Text[0], newValue[i]))
                    {
                        i++;
                    }

                    // If the entire prefix fit in the last run, just concat the string
                    if (i == end)
                    {
                        run.Text += newValue.AsSpan(start, end - start).InterleaveWithCharacter(ZeroWidthSpace);
                        return;
                    }

                    // If at least one character has been moved, insert those
                    if (i > start)
                    {
                        run.Text += newValue.AsSpan(start, i).InterleaveWithCharacter(ZeroWidthSpace);
                    }
                }
            }
            else @this.Inlines.Clear();

            // Parse and render the remaining text with new runs
            while (start < end)
            {
                char c = newValue[start];
                int i = start + 1;

                // Aggregate as many characters as possible into a single run
                while (i < end && ThemeInfo.HaveSameColor(c, newValue[i]))
                {
                    i++;
                }

                // Create and display the new run
                @this.Inlines.Add(new Run
                {
                    Text = newValue.AsSpan(start, end - i + 1).InterleaveWithCharacter(ZeroWidthSpace),
                    Foreground = Settings.Theme.GetBrush(c)
                });

                start = i;
            }
        }
    }
}
