using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.Ide.Helpers;

namespace Brainf_ckSharp.Uwp.AttachedProperties
{
    /// <summary>
    /// An attached property that controls the reference text to use to assing the number of lines text to a target <see cref="TextBlock"/>
    /// </summary>
    public static class NumberOfLinesHelper
    {
        /// <summary>
        /// Gets the value of <see cref="ReferenceTextProperty"/> for a given <see cref="TextBlock"/>
        /// </summary>
        /// <param name="element">The input <see cref="TextBlock"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="ReferenceTextProperty"/> property for the input <see cref="TextBlock"/> instance</returns>
        public static string GetReferenceText(TextBlock element)
        {
            return (string)element.GetValue(ReferenceTextProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="ReferenceTextProperty"/> for a given <see cref="TextBlock"/>
        /// </summary>
        /// <param name="element">The input <see cref="TextBlock"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="ReferenceTextProperty"/> property</param>
        public static void SetReferenceText(TextBlock element, string value)
        {
            element?.SetValue(ReferenceTextProperty, value);
        }

        /// <summary>
        /// An attached property that controls the reference text to use to assing the number of lines text to a target <see cref="TextBlock"/>
        /// </summary>
        public static readonly DependencyProperty ReferenceTextProperty = DependencyProperty.RegisterAttached(
            "ReferenceText",
            typeof(string),
            typeof(NumberOfLinesHelper),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnReferenceTextPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="ReferenceTextProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnReferenceTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock @this = (TextBlock)d;
            string value = (string)e.NewValue;

            int numberOfLines = value.Count('\r');
            @this.Text = TextGenerator.GetLineNumbersText(numberOfLines);
        }
    }
}
