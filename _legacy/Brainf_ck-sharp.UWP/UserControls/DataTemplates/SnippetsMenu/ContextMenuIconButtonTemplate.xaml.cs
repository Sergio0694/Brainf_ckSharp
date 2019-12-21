using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.SnippetsMenu
{
    public sealed partial class ContextMenuIconButtonTemplate : UserControl
    {
        public ContextMenuIconButtonTemplate()
        {
            this.InitializeComponent();
            this.ManageLightsPointerStates(value =>
            {
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the icon to display in the current instance
        /// </summary>
        [NotNull]
        public string Icon
        {
            get => IconBlock.Text;
            set => IconBlock.Text = value;
        }

        /// <inheritdoc cref="Button"/>
        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabled2Property);
            set => SetValue(IsEnabled2Property, value);
        }

        public static readonly DependencyProperty IsEnabled2Property = DependencyProperty.Register(
            "IsEnabled2", typeof(bool), typeof(ContextMenuIconButtonTemplate), new PropertyMetadata(true, OnIsEnabled2PropertyChanged));

        private static void OnIsEnabled2PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextMenuIconButtonTemplate @this = d.To<ContextMenuIconButtonTemplate>();
            bool value = e.NewValue.To<bool>();
            @this.Opacity = value ? 1 : 0.6;
            @this.IsHitTestVisible = value;
            @this.LightBorder.Opacity = value ? 0.8 : 0;
        }
    }
}
