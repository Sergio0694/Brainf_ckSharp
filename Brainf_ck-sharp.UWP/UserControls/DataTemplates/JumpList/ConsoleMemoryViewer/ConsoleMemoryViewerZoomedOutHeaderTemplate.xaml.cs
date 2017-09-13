using System;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.JumpList.ConsoleMemoryViewer
{
    public sealed partial class ConsoleMemoryViewerZoomedOutHeaderTemplate : UserControl
    {
        public ConsoleMemoryViewerZoomedOutHeaderTemplate()
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
        public String Title
        {
            get => (String)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(String), typeof(ConsoleMemoryViewerZoomedOutHeaderTemplate), new PropertyMetadata(default(String), OnTitlePropertyChanged));

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<ConsoleMemoryViewerZoomedOutHeaderTemplate>().TitleBlock.Text = e.NewValue.To<String>() ?? String.Empty;
        }

        /// <summary>
        /// Gets or sets the data model linked with this instance
        /// </summary>
        public MemoryViewerSectionBase DataModel
        {
            get => (MemoryViewerSectionBase)GetValue(DataModelProperty);
            set => SetValue(DataModelProperty, value);
        }

        public static readonly DependencyProperty DataModelProperty = DependencyProperty.Register(
            nameof(DataModel), typeof(MemoryViewerSectionBase), typeof(ConsoleMemoryViewerZoomedOutHeaderTemplate), 
            new PropertyMetadata(default(MemoryViewerSectionBase), OnDataModelPropertyChanged));

        private static void OnDataModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConsoleMemoryViewerZoomedOutHeaderTemplate @this = d.To<ConsoleMemoryViewerZoomedOutHeaderTemplate>();
            if (e.NewValue is MemoryViewerSectionBase data)
            {
                switch (data)
                {
                    case MemoryViewerMemoryCellsSectionData state:
                        @this.InfoBlock.Text = $"{state.State.Count} {LocalizationManager.GetResource("MemoryCells")}";
                        break;
                    case MemoryViewerFunctionsSectionData state:
                        @this.InfoBlock.Text = $"{state.Functions.Count} {LocalizationManager.GetResource(state.Functions.Count > 1 ? "DefinedFunctions" : "DefinedFunction")}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid section type");
                }
            }
        }
    }
}
