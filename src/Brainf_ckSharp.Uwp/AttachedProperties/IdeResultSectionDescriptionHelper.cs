using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Models.Ide.Views;
using Brainf_ckSharp.Uwp.Converters.Console;
using Brainf_ckSharp.Uwp.Themes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Extensions;
using Microsoft.Toolkit.Diagnostics;

namespace Brainf_ckSharp.Uwp.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to display a description for a section in a run view
    /// </summary>
    public static class IdeResultSectionDescriptionHelper
    {
        /// <summary>
        /// The <see cref="ISettingsService"/> instance currently in use
        /// </summary>
        private static readonly ISettingsService SettingsService = App.Current.Services.GetRequiredService<ISettingsService>();

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

        // Localized resources
        private static readonly string AtPosition = "IdeResults/AtPosition".GetLocalized();
        private static readonly string StackFrames = "IdeResults/StackFrames".GetLocalized();
        private static readonly string DefinedFunctions = "IdeResults/DefinedFunctions".GetLocalized();
        private static readonly string Operators = "IdeResults/Operators".GetLocalized();
        private static readonly string MemoryCells = "IdeResults/MemoryCells".GetLocalized();
        private static readonly string OperatorsInTime = "IdeResults/OperatorsInTime".GetLocalized();

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
                        Text = ExitCodeConverter.Convert(value.Result.ExitCode),
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
                case IdeResultSection.FaultingOperator:
                case IdeResultSection.BreakpointReached:
                    @this.Inlines.Add(new Run
                    {
                        Text = value.Result.HaltingInfo!.HaltingOperator.ToString(),
                        Foreground = SettingsService.GetValue<IdeTheme>(SettingsKeys.IdeTheme).AsBrainf_ckTheme().GetBrush(value.Result.HaltingInfo.HaltingOperator)
                    });
                    @this.Inlines.Add(new Run
                    {
                        Text = $" {string.Format(AtPosition, value.Result.HaltingInfo.HaltingOffset)}"
                    });
                    break;
                case IdeResultSection.StackTrace:
                    @this.Inlines.Add(new Run
                    {
                        Text = string.Format(StackFrames, value.Result.HaltingInfo!.StackTrace.Count)
                    });
                    break;
                case IdeResultSection.FunctionDefinitions:
                    @this.Inlines.Add(new Run
                    {
                        Text = string.Format(DefinedFunctions, value.Result.Functions.Count)
                    });
                    break;
                case IdeResultSection.SourceCode:
                    @this.Inlines.Add(new Run
                    {
                        Text = string.Format(Operators, value.Result.SourceCode.Length)
                    });
                    break;
                case IdeResultSection.MemoryState:
                    @this.Inlines.Add(new Run
                    {
                        Text = string.Format(MemoryCells, value.Result.MachineState.Count)
                    });
                    break;
                case IdeResultSection.Statistics:
                    @this.Inlines.Add(new Run
                    {
                        Text = string.Format(OperatorsInTime, value.Result.TotalOperations, value.Result.ElapsedTime)
                    });
                    break;
                default:
                    ThrowHelper.ThrowArgumentException("Invalid section type");
                    break;
            }
        }
    }
}
