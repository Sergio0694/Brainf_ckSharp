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
        private void LoadingPopupControl_Loaded(object sender, RoutedEventArgs e)
        {
            RootGrid.Background = CompositionBrushBuilder.FromBackdropAcrylic(Colors.Black, 0.5f, 6, new Uri("ms-appx:///Assets/Misc/lightnoise.png")).AsBrush();
        }
    }
}
