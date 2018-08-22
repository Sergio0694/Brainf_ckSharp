using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp;
using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.UI;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.AttachedProperties
{
    /// <summary>
    /// A Brainf_ck code inline formatter to display code with syntax highlight inside a <see cref="Span"/> object
    /// </summary>
    public static class Brainf_ckCodeInlineFormatter
    {
        /// <summary>
        /// Gets the zero width space character
        /// </summary>
        public const char ZeroWidthSpace = '\u200B';

        public static string GetSource(Span element)
        {
            return element.GetValue(SourceProperty).To<string>();
        }

        public static void SetSource(Span element, string value)
        {
            element?.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck code to a <see cref="Span"/> object
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(string), typeof(Brainf_ckCodeInlineFormatter), new PropertyMetadata(string.Empty, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the arguments
            Span @this = d.To<Span>();
            string raw = e.NewValue.To<string>();
            if (string.IsNullOrEmpty(raw))
            {
                @this.Inlines.Clear();
                return; // It's a trap!
            }

            // Parse the input code
            StringBuilder builder = new StringBuilder();
            char last = raw[0];
            List<Run> inlines = new List<Run>();
            foreach (char c in raw)
            {
                // Only display the language operators
                if (!Brainf_ckInterpreter.Operators.Contains(c)) continue;
                if (!Brainf_ckFormatterHelper.HaveSameColor(last, c))
                {
                    // Optimize the result by aggregating characters with the same color into the same inline
                    inlines.Add(new Run
                    {
                        Text = builder.ToString(),
                        Foreground = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(last))
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
                    Foreground = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(last))
                });
            }

            // Append the formatted text
            @this.Inlines.Clear();
            foreach (Run run in inlines) @this.Inlines.Add(run);
        }

        [UsedImplicitly] // XAML attached property
        public static IReadOnlyList<string> GetStackTrace(Span element)
        {
            return element.GetValue(StackTraceProperty).To<IReadOnlyList<string>>();
        }

        [UsedImplicitly]
        public static void SetStackTrace(Span element, IReadOnlyList<string> value)
        {
            element?.SetValue(StackTraceProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck stack trace to a <see cref="Span"/> object
        /// </summary>
        public static readonly DependencyProperty StackTraceProperty =
            DependencyProperty.RegisterAttached("StackTrace", typeof(IReadOnlyList<string>), typeof(Brainf_ckCodeInlineFormatter), 
                new PropertyMetadata(DependencyProperty.UnsetValue, OnStackTracePropertyPropertyChanged));

        private static void OnStackTracePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            IReadOnlyList<string> stack = e.NewValue.To<IReadOnlyList<string>>();
            if (stack == null)
            {
                @this.Inlines.Clear();
                return; // Fall back!
            }
            List<Inline> inlines = new List<Inline>();
            int depth = 0;
            bool skipped = false;
            int count = -1;
            foreach ((string call, int occurrences) in stack.CompressEqual((s1, s2) => s1.Equals(s2)))
            {
                // Insert the "at" separator if needed
                if (depth > 0)
                {
                    if (skipped) skipped = false;
                    else
                    {
                        inlines.Add(new LineBreak());
                        inlines.Add(new Run
                        {
                            Text = $"at{(count > 1 ? $" [{count} {LocalizationManager.GetResource("StackFrames")}]" : string.Empty)}",
                            Foreground = new SolidColorBrush(Colors.DimGray),
                            FontSize = @this.FontSize - 1
                        });
                        inlines.Add(new LineBreak());
                    }
                }

                // Skip the first line if it's empty
                if (call.Length == 0 && !skipped) skipped = true;
                else
                {
                    // Add the formatted call line
                    Span line = new Span();
                    SetSource(line, call);
                    inlines.Add(line);
                }
                count = occurrences;
                depth++;
            }
            @this.Inlines.Clear();
            foreach (Inline inline in inlines) @this.Inlines.Add(inline);
        }

        [UsedImplicitly]
        public static string GetUnformattedSource(Span element)
        {
            return element.GetValue(UnformattedSourceProperty).To<string>();
        }

        [UsedImplicitly]
        public static void SetUnformattedSource(Span element, string value)
        {
            element?.SetValue(UnformattedSourceProperty, value);
        }

        /// <summary>
        /// A property that shows an unformatted Brainf_ck code to a <see cref="Span"/> object (it just renders the characters)
        /// </summary>
        public static readonly DependencyProperty UnformattedSourceProperty =
            DependencyProperty.RegisterAttached("UnformattedSource", typeof(string), typeof(Brainf_ckCodeInlineFormatter), new PropertyMetadata(string.Empty, OnUnformattedSourcePropertyChanged));

        // The regex pattern to remove unwaanted characters
        private static readonly string Pattern = $"[^{Brainf_ckInterpreter.Operators.Aggregate(c => $@"\{c}")}]";

        private static void OnUnformattedSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            string
                raw = e.NewValue.To<string>(),
                code = Regex.Replace(raw, Pattern, "");
            @this.Inlines.Clear();
            @this.Inlines.Add(new Run { Text = code.Aggregate(c => $"{c}{ZeroWidthSpace}") });
        }
    }
}
