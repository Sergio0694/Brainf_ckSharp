using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Actions;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects.Base;

namespace Brainf_ck_sharp_UWP.UserControls
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            // Update the effects and initialize the UI
            this.Loaded += Shell_Loaded;
            this.SizeChanged += (s, e) =>
            {
                _HeaderEffect?.AdjustSize();
                _KeyboardEffect?.AdjustSize();
            };
            this.InitializeComponent();
            Console.ViewModel.IsEnabled = true;

            // Hide the title placeholder if needed
            if (UniversalAPIsHelper.IsMobileDevice)
            {
                PCPlaceholderGrid.Visibility = Visibility.Collapsed;
                KeyboardCanvas.Visibility = Visibility.Collapsed;
                KeyboardBorder.Visibility = Visibility.Collapsed;
            }
        }

        // Acrylic brush for the header
        private AttachedStaticCompositionEffect<Border> _HeaderEffect;

        // Acrylic brush for the virtual keyboard
        private AttachedStaticCompositionEffect<Border> _KeyboardEffect;

        // Initialize the effects
        private async void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            Console.AdjustTopMargin(HeaderGrid.ActualHeight + 12);
            if (UniversalAPIsHelper.IsMobileDevice)
            {
                _HeaderEffect = await HeaderBorder.GetAttachedInAppSemiAcrylicEffectAsync(HeaderBorder, 8, 800,
                    Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.6f,
                    HeaderCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
                OperatorsKeyboard.Background = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 10, 10, 10));
            }
            else
            {
                _HeaderEffect = await HeaderBorder.GetAttachedSemiAcrylicEffectAsync(Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.8f,
                    HeaderCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
                _KeyboardEffect = await KeyboardBorder.GetAttachedSemiAcrylicEffectAsync(Color.FromArgb(byte.MaxValue, 16, 16, 16), 0.95f,
                    KeyboardCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new PlayScriptMessage());
        }
    }
}
