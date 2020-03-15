using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Models;

namespace Brainf_ckSharp.Uwp.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to formatted Brainf*ck/PBrain code
    /// </summary>
    public static partial class Brainf_ckInlineFormatterHelper
    {
        /// <summary>
        /// Gets the value of <see cref="StackTraceProperty"/> for a given <see cref="Paragraph"/>
        /// </summary>
        /// <param name="element">The input <see cref="Paragraph"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="StackTraceProperty"/> property for the input <see cref="Paragraph"/> instance</returns>
        public static IReadOnlyList<string> GetStackTrace(Paragraph element)
        {
            return element.GetValue(StackTraceProperty) as IReadOnlyList<string>;
        }

        /// <summary>
        /// Sets the value of <see cref="StackTraceProperty"/> for a given <see cref="Paragraph"/>
        /// </summary>
        /// <param name="element">The input <see cref="Paragraph"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="StackTraceProperty"/> property</param>
        public static void SetStackTrace(Paragraph element, IReadOnlyList<string> value)
        {
            element.SetValue(StackTraceProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck code to a <see cref="Paragraph"/> object
        /// </summary>
        public static readonly DependencyProperty StackTraceProperty = DependencyProperty.RegisterAttached(
            "StackTrace",
            typeof(IReadOnlyList<string>),
            typeof(Brainf_ckInlineFormatterHelper),
            new PropertyMetadata(Array.Empty<FunctionDefinition>(), OnStackTracePropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="StackTraceProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnStackTracePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the arguments
            Paragraph @this = (Paragraph)d;
            IReadOnlyList<string> value = (IReadOnlyList<string>)e.NewValue;

            @this.Inlines.Clear();

            for (int i = 0; i < value.Count; i++)
            {
                if (i > 0) @this.Inlines.Add(new LineBreak());

                @this.Inlines.Add(new Run
                {
                    Text = "at",
                    Foreground = new SolidColorBrush(Colors.DimGray),
                    FontSize = @this.FontSize - 1
                });
                @this.Inlines.Add(new LineBreak());

                Span span = new Span();
                SetSource(span, value[i]);

                @this.Inlines.Add(span);
            }
        }
    }
}
