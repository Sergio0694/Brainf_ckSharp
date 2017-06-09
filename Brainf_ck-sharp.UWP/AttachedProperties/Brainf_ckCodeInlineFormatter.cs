using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class Brainf_ckCodeInlineFormatter
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

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(String), typeof(RunInlineHelper), new PropertyMetadata(String.Empty, OnPropertyChanged));

        /// <summary>
        /// Gets the syntax highlight colors map for the available operators
        /// </summary>
        private static readonly IReadOnlyDictionary<char, Color> HighlightMap = new Dictionary<char, Color>
        {
            { '>', Color.FromArgb(byte.MaxValue, 0xDD, 0xDD, 0xDD) },
            { '<', Color.FromArgb(byte.MaxValue, 0xDD, 0xDD, 0xDD) },
            { '+', Colors.White },
            { '-', Colors.White },
            { '[', Color.FromArgb(byte.MaxValue, 0x59, 0x6C, 0xD6) },
            { ']', Color.FromArgb(byte.MaxValue, 0x59, 0x6C, 0xD6) },
            { '.', Colors.IndianRed },
            { ',', Colors.DarkKhaki }
        };

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            String code = e.NewValue.To<String>();
            IEnumerable<Run> runs =
                from c in code
                let text = $"{c}{ZeroWidthSpace}"
                let brush = new SolidColorBrush(HighlightMap[c])
                select new Run { Text = text, Foreground = brush };
            @this.Inlines.Clear();
            foreach (Run run in runs) @this.Inlines.Add(run);
        }

        public static String GetUnformattedSource(Span element)
        {
            return element.GetValue(UnformattedSourceProperty).To<String>();
        }

        public static void SetUnformattedSource(Span element, String value)
        {
            element?.SetValue(UnformattedSourceProperty, value);
        }

        public static readonly DependencyProperty UnformattedSourceProperty =
            DependencyProperty.RegisterAttached("UnformattedSource", typeof(String), typeof(Brainf_ckCodeInlineFormatter), new PropertyMetadata(String.Empty, OnUnformattedSourcePropertyChanged));

        private static void OnUnformattedSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Span @this = d.To<Span>();
            @this.Inlines.Clear();
            @this.Inlines.Add(new Run
            {
                Text = e.NewValue.To<String>()?.Aggregate(new StringBuilder(), (b, c) =>
                {
                    b.Append(c);
                    return b;
                }).ToString() ?? String.Empty
            });
        }
    }
}
