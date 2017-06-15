using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers;

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

        public static String GetSource(Span element)
        {
            return element.GetValue(SourceProperty).To<String>();
        }

        public static void SetSource(Span element, String value)
        {
            element?.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck code to a <see cref="Span"/> object
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(String), typeof(Brainf_ckCodeInlineFormatter), new PropertyMetadata(String.Empty, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            String
                raw = e.NewValue.To<String>(),
                code = Regex.Replace(raw, @"[^\+\-\[\]\.,><]", "");
            IEnumerable<Run> runs =
                from c in code
                let text = $"{c}{ZeroWidthSpace}"
                let brush = new SolidColorBrush(Brainf_ckFormatterHelper.GetSyntaxHighlightColorFromChar(c))
                select new Run { Text = text, Foreground = brush };
            @this.Inlines.Clear();
            foreach (Run run in runs) @this.Inlines.Add(run);
        }

        public static IReadOnlyList<String> GetStackTrace(Span element)
        {
            return element.GetValue(StackTraceProperty).To<IReadOnlyList<String>>();
        }

        public static void SetStackTrace(Span element, IReadOnlyList<String> value)
        {
            element?.SetValue(StackTraceProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck stack trace to a <see cref="Span"/> object
        /// </summary>
        public static readonly DependencyProperty StackTraceProperty =
            DependencyProperty.RegisterAttached("StackTrace", typeof(IReadOnlyList<String>), typeof(Brainf_ckCodeInlineFormatter), 
                new PropertyMetadata(DependencyProperty.UnsetValue, OnStackTracePropertyPropertyChanged));

        private static void OnStackTracePropertyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            IReadOnlyList<String> stack = e.NewValue.To<IReadOnlyList<String>>();
            List<Inline> inlines = new List<Inline>();
            foreach ((String call, int i) in stack.Select((c, i) => (c, i)))
            {
                // Insert the "at" separator if needed
                if (i > 0)
                {
                    inlines.Add(new LineBreak());
                    inlines.Add(new Run
                    {
                        Text = "at",
                        Foreground = new SolidColorBrush(Colors.DimGray),
                        FontSize = @this.FontSize - 1
                    });
                    inlines.Add(new LineBreak());
                }

                // Add the formatted call line
                Span line = new Span();
                SetSource(line, call);
                inlines.Add(line);
            }
            @this.Inlines.Clear();
            foreach (Inline inline in inlines) @this.Inlines.Add(inline);
        }

        public static String GetUnformattedSource(Span element)
        {
            return element.GetValue(UnformattedSourceProperty).To<String>();
        }

        public static void SetUnformattedSource(Span element, String value)
        {
            element?.SetValue(UnformattedSourceProperty, value);
        }

        /// <summary>
        /// A property that shows an unformatted Brainf_ck code to a <see cref="Span"/> object (it just renders the characters)
        /// </summary>
        public static readonly DependencyProperty UnformattedSourceProperty =
            DependencyProperty.RegisterAttached("UnformattedSource", typeof(String), typeof(Brainf_ckCodeInlineFormatter), new PropertyMetadata(String.Empty, OnUnformattedSourcePropertyChanged));

        private static void OnUnformattedSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            String
                raw = e.NewValue.To<String>(),
                code = Regex.Replace(raw, @"[^\+\-\[\]\.,><]", "");
            @this.Inlines.Clear();
            @this.Inlines.Add(new Run
            {
                Text = code?.Aggregate(new StringBuilder(), (b, c) =>
                {
                    b.Append($"{c}{ZeroWidthSpace}");
                    return b;
                }).ToString() ?? String.Empty
            });
        }
    }
}
