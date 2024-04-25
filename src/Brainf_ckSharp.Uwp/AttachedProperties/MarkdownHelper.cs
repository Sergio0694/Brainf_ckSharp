using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

#nullable enable

namespace Brainf_ckSharp.Uwp.AttachedProperties;

/// <summary>
/// A <see langword="class"/> with an attached XAML property to append simple markdown text to a <see cref="TextBlock"/> control
/// </summary>
public static class MarkdownHelper
{
    /// <summary>
    /// An attached property that holds the markdown text to add to a target control
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
        "Text",
        typeof(string),
        typeof(MarkdownHelper),
        new(null, OnTextPropertyChanged));

    /// <summary>
    /// Gets the value of <see cref="TextProperty"/> for a given <see cref="UIElement"/>
    /// </summary>
    /// <param name="element">The input <see cref="UIElement"/> for which to get the property value</param>
    /// <returns>The value of the <see cref="TextProperty"/> property for the input <see cref="UIElement"/> instance</returns>
    public static string? GetText(TextBlock element)
    {
        return (string?)element.GetValue(TextProperty);
    }

    /// <summary>
    /// Sets the value of <see cref="TextProperty"/> for a given <see cref="UIElement"/>
    /// </summary>
    /// <param name="element">The input <see cref="UIElement"/> for which to set the property value</param>
    /// <param name="value">The value to set for the <see cref="TextProperty"/> property</param>
    public static void SetText(TextBlock element, string? value)
    {
        element.SetValue(TextProperty, value);
    }

    /// <summary>
    /// Updates the UI when <see cref="TextProperty"/> changes
    /// </summary>
    /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBlock @this = (TextBlock)d;

        @this.Inlines.Clear();

        // Nothing to add if the input text is null
        if (e.NewValue is not string text)
        {
            return;
        }

        ReadOnlySpan<char> remainingSpan = text.AsSpan();
        bool isBold = false;

        while (true)
        {
            int indexOfMarker = remainingSpan.IndexOf("**".AsSpan(), StringComparison.Ordinal);

            if (indexOfMarker == -1)
            {
                break;
            }

            AppendInline(@this, remainingSpan.Slice(0, indexOfMarker).ToString(), isBold);

            remainingSpan = remainingSpan.Slice(indexOfMarker + 2);
            isBold ^= true;
        }

        // Append the last inline
        AppendInline(@this, remainingSpan.ToString(), isBold);
    }

    /// <summary>
    /// Appends a given inline to the input element.
    /// </summary>
    /// <param name="element">The target <see cref="TextBlock"/> element.</param>
    /// <param name="text">The text to append.</param>
    /// <param name="isBold">Whether the text to append is bold.</param>
    private static void AppendInline(TextBlock element, string text, bool isBold)
    {
        Run run = new() { Text = text };

        if (isBold)
        {
            run.FontWeight = FontWeights.Bold;
        }

        element.Inlines.Add(run);
    }
}
