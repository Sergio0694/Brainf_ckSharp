using System;
using System.Collections.Generic;
using Windows.Devices.Input;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Converters;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.JumpList.IDEResult
{
    public sealed partial class IDEResultZoomedOutHeaderTemplate : UserControl
    {
        public IDEResultZoomedOutHeaderTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((pointer, value) =>
            {
                // Visual states
                VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false);

                // Lights
                if (pointer != PointerDeviceType.Mouse) return;
                LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the title to display in the control
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(IDEResultZoomedOutHeaderTemplate), new PropertyMetadata(default(string), OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<IDEResultZoomedOutHeaderTemplate>().TitleBlock.Text = e.NewValue.To<string>() ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the data model linked with this instance
        /// </summary>
        public IDEResultSectionDataBase DataModel
        {
            get => (IDEResultSectionDataBase)GetValue(DataModelProperty);
            set => SetValue(DataModelProperty, value);
        }

        public static readonly DependencyProperty DataModelProperty = DependencyProperty.Register(
            nameof(DataModel), typeof(IDEResultSectionDataBase), typeof(IDEResultZoomedOutHeaderTemplate), 
            new PropertyMetadata(default(IDEResultSectionDataBase), OnDataModelPropertyChanged));

        private static void OnDataModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IDEResultZoomedOutHeaderTemplate @this = d.To<IDEResultZoomedOutHeaderTemplate>();
            if (e.NewValue is IDEResultSectionDataBase data)
            {
                switch (data)
                {
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.Stdout:
                        @this.InfoBlock.Text = section.Session.CurrentResult.Output;
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.Cornsilk);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.SourceCode:
                        @this.InfoBlock.Text = section.Session.CurrentResult.SourceCode.Length > 1 
                            ? $"{section.Session.CurrentResult.SourceCode.Length} {LocalizationManager.GetResource("LowercaseOperators")}" 
                            : LocalizationManager.GetResource("LowercaseSingleOperator");
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.LightGray);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.StackTrace:
                        int depth = section.Session.CurrentResult.ExceptionInfo?.StackTrace.Count ?? 0;
                        @this.InfoBlock.Text = depth > 1
                            ? $"{depth} {LocalizationManager.GetResource("MoreStackFrames")}"
                            : LocalizationManager.GetResource("OneStackFrame");
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.LightGray);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.ErrorLocation ||
                                                                  section.Section == IDEResultSection.BreakpointReached:
                        char c = section.Session.CurrentResult.ExceptionInfo?.FaultedOperator ?? (char)0;
                        List<Run> inlines = new List<Run>
                        {
                            new Run
                            {
                                Text = c.ToString(),
                                Foreground = new SolidColorBrush(Brainf_ckFormatterHelper.Instance.GetSyntaxHighlightColorFromChar(c))
                            },
                            new Run
                            {
                                Text = $" {LocalizationManager.GetResource("LowercaseAtPosition")} {section.Session.CurrentResult.ExceptionInfo?.ErrorPosition ?? 0}",
                                Foreground = new SolidColorBrush(Colors.LightGray)
                            }
                        };
                        @this.InfoBlock.Inlines.Clear();
                        foreach (Run run in inlines) @this.InfoBlock.Inlines.Add(run);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    case IDEResultSectionSessionData section when section.Section == IDEResultSection.Stats:
                        @this.InfoBlock.Text = section.Session.CurrentResult.TotalOperations > 1
                            ? $"{section.Session.CurrentResult.TotalOperations} {LocalizationManager.GetResource("LowercaseOperators")}"
                            : LocalizationManager.GetResource("LowercaseSingleOperator");
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.LightGray);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    case IDEResultExceptionInfoData exception:
                        string message = ExceptionTypeConverter.Convert(exception.Info.ExceptionType);
                        @this.InfoBlock.Text = $"{message[0].ToString().ToUpperInvariant()}{message.Substring(1)}";
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.DarkRed);
                        @this.InfoBlock.FontWeight = FontWeights.SemiBold;
                        break;
                    case IDEResultSectionStateData state:
                        @this.InfoBlock.Text = $"{state.IndexedState.Count} {LocalizationManager.GetResource("MemoryCells")}";
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.LightGray);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    case IDEResultSectionFunctionsData state:
                        @this.InfoBlock.Text = $"{state.IndexedDefinitions.Count} {LocalizationManager.GetResource(state.IndexedDefinitions.Count > 1 ? "DefinedFunctions" : "DefinedFunction")}";
                        @this.InfoBlock.Foreground = new SolidColorBrush(Colors.LightGray);
                        @this.InfoBlock.FontWeight = FontWeights.Normal;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(data), "Invalid section type");
                }
            }
        }
    }
}
