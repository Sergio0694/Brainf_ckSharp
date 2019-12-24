using System;
using System.Collections.Generic;
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
        /// <param name="value">The valaue to set for the <see cref="SourceProperty"/> property</param>
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
                newValue = (string)e.NewValue,
                pendingText = newValue;

            // Skip if the input code is empty
            if (string.IsNullOrEmpty(newValue))
            {
                @this.Inlines.Clear();
                return;
            }

            // Reuse the existing inlines when the new code is an extension of the previous code
            if (newValue.StartsWith(oldValue))
            {
                pendingText = newValue.Substring(oldValue.Length);
            }
            else @this.Inlines.Clear();

            // Parse the input code
            StringBuilder builder = new StringBuilder();
            char last = pendingText[0];
            List<Run> inlines = new List<Run>();
            foreach (char c in pendingText)
            {
                // Only display the language operators
                if (!Brainf_ckParser.IsOperator(c)) continue;
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
