using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Models.Ide.Views;
using Brainf_ckSharp.Uwp.Converters.Console;

namespace Brainf_ckSharp.Uwp.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to display a description for a section in a run view
    /// </summary>
    public static class IdeResultSectionDescriptionHelper
    {
        /// <summary>
        /// Gets the value of <see cref="SectionProperty"/> for a given <see cref="Span"/>
        /// </summary>
        /// <param name="element">The input <see cref="Span"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="SectionProperty"/> property for the input <see cref="Span"/> instance</returns>
        public static IdeResultWithSectionInfo GetSection(Span element)
        {
            return element.GetValue(SectionProperty) as IdeResultWithSectionInfo;
        }

        /// <summary>
        /// Sets the value of <see cref="SectionProperty"/> for a given <see cref="Span"/>
        /// </summary>
        /// <param name="element">The input <see cref="Span"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="SectionProperty"/> property</param>
        public static void SetSection(Span element, IdeResultWithSectionInfo value)
        {
            element.SetValue(SectionProperty, value);
        }

        /// <summary>
        /// A property that shows a formatted Brainf_ck code to a <see cref="Paragraph"/> object
        /// </summary>
        public static readonly DependencyProperty SectionProperty = DependencyProperty.RegisterAttached(
            "Section",
            typeof(IdeResultWithSectionInfo),
            typeof(Brainf_ckInlineFormatterHelper),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnSectionPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="SectionProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnSectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the arguments
            Span @this = (Span)d;
            IdeResultWithSectionInfo value = (IdeResultWithSectionInfo)e.NewValue;

            @this.Inlines.Clear();

            if (value is null) return;

            switch (value.Section)
            {
                case IdeResultSection.ExceptionType:
                    @this.Inlines.Add(new Run
                    {
                        Text = $"exception > {ExitCodeConverter.Convert(value.Result.ExitCode)}",
                        Foreground = new SolidColorBrush(Colors.DarkRed)
                    });
                    break;
                case IdeResultSection.Stdout:
                    @this.Inlines.Add(new Run
                    {
                        Text = value.Result.Stdout,
                        Foreground = new SolidColorBrush(Colors.Cornsilk)
                    });
                    break;
                case IdeResultSection.ErrorLocation:
                case IdeResultSection.BreakpointReached:
                    @this.Inlines.Add(new Run
                    {
                        Text = value.Result.HaltingInfo!.HaltingOperator.ToString(),
                        Foreground = Settings.GetCurrentTheme().GetBrush(value.Result.HaltingInfo.HaltingOperator)
                    });
                    @this.Inlines.Add(new Run
                    {
                        Text = $" at position {value.Result.HaltingInfo.HaltingOffset}"
                    });
                    break;
                case IdeResultSection.StackTrace:
                    @this.Inlines.Add(new Run
                    {
                        Text = $"{value.Result.HaltingInfo!.StackTrace.Count} stack frame(s)"
                    });
                    break;
                case IdeResultSection.FunctionDefinitions:
                    @this.Inlines.Add(new Run
                    {
                        Text = $"{value.Result.Functions.Count} defined function(s)"
                    });
                    break;
                case IdeResultSection.SourceCode:
                    @this.Inlines.Add(new Run
                    {
                        Text = $"{value.Result.SourceCode.Length} operator(s)"
                    });
                    break;
                case IdeResultSection.MemoryState:
                    @this.Inlines.Add(new Run
                    {
                        Text = $"{value.Result.MachineState.Count} memory cells"
                    });
                    break;
                case IdeResultSection.Statistics:
                    @this.Inlines.Add(new Run
                    {
                        Text = $"{value.Result.TotalOperations} operator(s) in {value.Result.ElapsedTime}"
                    });
                    break;
                default: throw new ArgumentException($"Invalid section: {value.Section}");
            }
        }
    }
}
