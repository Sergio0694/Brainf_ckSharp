using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Models;
using Microsoft.Toolkit.Uwp;

#nullable enable

namespace Brainf_ckSharp.Uwp.AttachedProperties;

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
    public static IReadOnlyList<string>? GetStackTrace(Paragraph element)
    {
        return element.GetValue(StackTraceProperty) as IReadOnlyList<string>;
    }

    /// <summary>
    /// Sets the value of <see cref="StackTraceProperty"/> for a given <see cref="Paragraph"/>
    /// </summary>
    /// <param name="element">The input <see cref="Paragraph"/> for which to set the property value</param>
    /// <param name="value">The value to set for the <see cref="StackTraceProperty"/> property</param>
    public static void SetStackTrace(Paragraph element, IReadOnlyList<string>? value)
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
        new(Array.Empty<FunctionDefinition>(), OnStackTracePropertyChanged));

    // Localized resources
    private static readonly string At = "StackTrace/At".GetLocalized();
    private static readonly string Frames = "StackTrace/Frames".GetLocalized();

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

        if (value.Count == 0) return;

        int i = 0;
        foreach (var entry in CompressStackTrace(value))
        {
            if (i++ > 0) @this.Inlines.Add(new LineBreak());

            // Insert the "at" separator if needed
            @this.Inlines.Add(new Run
            {
                Text = $"{At}{(entry.Occurrences > 1 ? $" [{entry.Occurrences * entry.Length} {Frames}]" : string.Empty)}",
                Foreground = new SolidColorBrush(Colors.DimGray),
                FontSize = @this.FontSize - 1
            });
            @this.Inlines.Add(new LineBreak());

            // Add the formatted call line
            Span line = new();
            SetSource(line, entry.Item);
            @this.Inlines.Add(line);
        }
    }

    /// <summary>
    /// Compresses a stack trace by aggregating recursive calls
    /// </summary>
    /// <param name="frames">The input list of stack frames</param>
    public static IEnumerable<(string Item, int Occurrences, int Length)> CompressStackTrace(IReadOnlyList<string> frames)
    {
        int i = 0;
        List<(int Length, int Occurrences)> info = new();
        while (i < frames.Count)
        {
            for (int step = 1; step < 5 && i + (step * 2) - 1 < frames.Count; step++)
            {
                // Find a valid sub-pattern of a given length
                bool valid = true;
                for (int j = 0; j < step; j++)
                    if (!frames[i + j].Equals(frames[i + step + j]))
                    {
                        valid = false;
                        break;
                    }

                if (!valid) continue;

                // Check of many times the pattern repeats
                int occurrences = 2;
                for (int j = i + (step * 2); j + step - 1 < frames.Count; j += step)
                {
                    valid = true;
                    for (int k = 0; k < step; k++)
                        if (!frames[i + k].Equals(frames[j + k]))
                        {
                            valid = false;
                            break;
                        }

                    if (valid) occurrences++;
                }

                // Store the current sub-sequence info
                info.Add((step, occurrences));
            }

            // Return the current compressed chunk
            if (info.Count == 0)
            {
                yield return (frames[i], 1, 1);
                i += 1;
            }
            else
            {
                var best = info.OrderByDescending(item => item.Length * item.Occurrences).ThenBy(item => item.Length).First();
                StringBuilder builder = new();
                for (int j = 0; j < best.Length; j++)
                    builder.Append(frames[i + j]);
                string call = builder.ToString();
                if (call.Contains(':'))
                {
                    // Only aggregate recursive calls
                    yield return (call, best.Occurrences, best.Length);
                    i += best.Length * best.Occurrences;
                }
                else
                {
                    // The repeated loops are just user code
                    yield return (frames[i], 1, 1);
                    i += 1;
                }
            }

            info.Clear();
        }
    }
}
