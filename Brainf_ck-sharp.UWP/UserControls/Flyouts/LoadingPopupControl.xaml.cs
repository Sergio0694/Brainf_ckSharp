using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UICompositionAnimations.Behaviours;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class LoadingPopupControl : UserControl
    {
        public LoadingPopupControl()
        {
            Loaded += LoadingPopupControl_Loaded;
            this.InitializeComponent();
        }

        // Setup the effect
        private async void LoadingPopupControl_Loaded(object sender, RoutedEventArgs e)
        {
            await EffectBorder.AttachCompositionInAppCustomAcrylicEffectAsync(EffectBorder,
                6, 400, Colors.Black, 0.5f, Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"), disposeOnUnload: true);
        }
    }
}
