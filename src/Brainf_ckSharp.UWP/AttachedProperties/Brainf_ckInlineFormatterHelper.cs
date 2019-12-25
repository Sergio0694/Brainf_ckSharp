using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Brainf_ckSharp.UWP.Constants;
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
        /// A table that maps targeted <see cref="Span"/> items to the reusable <see cref="StringBuilder"/> instances
        /// </summary>
        private static readonly ConditionalWeakTable<Span, StringBuilder> BuildersTable = new ConditionalWeakTable<Span, StringBuilder>();

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
                newValue = (string)e.NewValue,
                pendingText = newValue;

            // Skip if the input code is empty
            if (string.IsNullOrEmpty(newValue))
            {
                @this.Inlines.Clear();
                return;
            }

            // Reuse the existing inlines when the new code is an extension of the previous code
            if (newValue.Length > oldValue.Length &&
                newValue.StartsWith(oldValue))
            {
                /* If the old value is a prefix of the current one, it means that
                 * additional text has been added to it. In this case, we can do an
                 * incremental update to save time and memory. Additionally, if there's
                 * at least a run already existing, we can check whether it is of the same
                 * type as the first one to add: if it is we can merge the two operations.
                 * This avoids creating new runs entirely when adding new characters one
                 * at a time, when they're all of the same type (eg. when typing
                 * the same Brainf*ck/PBrain operator multiple times). */
                pendingText = newValue.Substring(oldValue.Length);

                int i = 0;
                if (@this.Inlines.LastOrDefault() is Run run &&
                    ThemeInfo.HaveSameColor(run.Text[0], pendingText[0]))
                {
                    // Check the initial characters for matching color
                    for (i = 1; i < pendingText.Length; i++)
                        if (!ThemeInfo.HaveSameColor(run.Text[0], pendingText[i]))
                            break;

                    // Append as many initial characters as possible
                    run.Text += pendingText.Substring(0, i);
                }

                // Update the pending string or stop
                if (i < pendingText.Length) pendingText = pendingText.Substring(0, i);
                else return;
            }
            else if (oldValue.Length > newValue.Length &&
                     oldValue.StartsWith(newValue))
            {
                /* When the new value is a prefix of the previous value,
                 * it means that the user has deleted one or more characters
                 * from the previous string. In this case we just need to traverse
                 * the current inlines collection backwards and remove as many
                 * runs or characters as needed. This will avoid having to recompute
                 * the entire syntax highlight from scratch for the new text.
                 * The diffference is doubled because each character has an additional
                 * zero width space character next to it, for better line wrapping. */
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
            else
            {
                /* In case the new value is completely different, just delete all
                 * the existing inlines and start the syntax highlight from scratch */ 
                @this.Inlines.Clear();
            }

            // Get the builder to use and clear it
            StringBuilder builder = BuildersTable.GetOrCreateValue(@this);
            builder.Clear();

            // Parse the input code
            char last = pendingText[0];
            List<Run> inlines = new List<Run>();
            foreach (char c in pendingText)
            {
                if (!ThemeInfo.HaveSameColor(last, c))
                {
                    // Optimize the result by aggregating characters with the same color into the same inline
                    inlines.Add(new Run
                    {
                        Text = builder.ToString(),
                        Foreground = Settings.Theme.GetBrush(last)
                    });
                    builder.Clear();
                    last = c;
                }

                builder.Append($"{c}{ZeroWidthSpace}");
            }

            // Include the latest chunk of operators if present
            if (builder.Length > 0)
            {
                inlines.Add(new Run
                {
                    Text = builder.ToString(),
                    Foreground = Settings.Theme.GetBrush(last)
                });
            }

            // Append the formatted text
            foreach (Run run in inlines)
            {
                @this.Inlines.Add(run);
            }
        }
    }
}
