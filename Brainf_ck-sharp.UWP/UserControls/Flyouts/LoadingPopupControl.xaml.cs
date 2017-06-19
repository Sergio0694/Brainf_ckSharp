using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class LoadingPopupControl : UserControl
    {
        public LoadingPopupControl()
        {
            SizeChanged += (_, e) => _BlurEffect?.AdjustSize(e.NewSize.Width, e.NewSize.Height);
            Loaded += LoadingPopupControl_Loaded;
            this.InitializeComponent();
        }

        // Setup the effect
        private async void LoadingPopupControl_Loaded(object sender, RoutedEventArgs e)
        {
            _BlurEffect = await EffectBorder.GetAttachedInAppSemiAcrylicEffectAsync(EffectBorder,
                6, 400, Colors.Black, 0.4f, Win2DCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
        }

        // The in-app acrylic blur effect
        private AttachedStaticCompositionEffect<Border> _BlurEffect;
    }
}
