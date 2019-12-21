using System;
using Windows.ApplicationModel;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp.Legacy.UWP.Enums;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Settings;
using Brainf_ck_sharp.Legacy.UWP.Messages.Requests;
using Brainf_ck_sharp.Legacy.UWP.Messages.Settings;
using Brainf_ck_sharp.Legacy.UWP.Messages.UI;
using Brainf_ck_sharp.Legacy.UWP.PopupService;
using Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.SnippetsMenu;
using Brainf_ck_sharp.Legacy.UWP.UserControls.VirtualKeyboard.Controls;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations.Enums;
using UICompositionAnimations.XAMLTransform;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.VirtualKeyboard
{
    /// <summary>
    /// A <see cref="UserControl"/> with a 2-rows grid that hosts virtual buttons for the 8 Brainf_ck operators
    /// </summary>
    public sealed partial class VirtualKeyboardControl : UserControl
    {
        public VirtualKeyboardControl()
        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled &&
                AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ShowPBrainButtons)))
            {
                // Show the PBrain buttons on startup, if needed
                PBrainColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            Messenger.Default.Register<PBrainButtonsVisibilityChangedMessage>(this, m => AnimateUI(m.Value));
        }

        /// <summary>
        /// Raised whenever the user taps on any of the buttons in the virtual keyboard
        /// </summary>
        public event EventHandler<char> KeyPressed;

        // Raises the KeyPressed event with the given character
        private void OnKeyPressed(char key)
        {
            KeyPressed?.Invoke(this, key);
            Messenger.Default.Send(new OperatorAddedMessage(key));
        }

        #region Virtual buttons tap handlers

        private void NextButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed('>');

        private void BackButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed('<');

        private void PlusButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed('+');

        private void MinusButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed('-');

        private void OpenBracketButton_Tapped(object sender, RoutedEventArgs e)
        {
            if (_SnippetsMenuOpen) _SnippetsMenuOpen = false;
            else OnKeyPressed('[');
        }

        private void CloseBracketButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed(']');

        private void PointButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed('.');

        private void CommaButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed(',');

        private void FunctionOpenButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed('(');

        private void FunctionCloseButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed(')');

        private void CallButton_Tapped(object sender, RoutedEventArgs e) => OnKeyPressed(':');

        #endregion

        // The duration of the keyboard animation
        private const int ExpansionAnimationDuration = 250;

        // Animates the expansion/collapse of the PBrain keyboard section
        private async void AnimateUI(bool extended)
        {
            if (extended)
            {
                PBrainBorder.Width = 0;
                PBrainColumn.Width = new GridLength(1, GridUnitType.Auto);
                await XAMLTransformToolkit.CreateDoubleAnimation(
                    PBrainBorder, "Width", 0, ActualWidth / 5, ExpansionAnimationDuration,
                    EasingFunctionNames.CircleEaseOut, true).ToStoryboard().WaitAsync();
                PBrainColumn.Width = new GridLength(1, GridUnitType.Star);
                PBrainBorder.Width = double.NaN;
            }
            else
            {
                PBrainBorder.Width = PBrainBorder.ActualWidth;
                PBrainColumn.Width = new GridLength(1, GridUnitType.Auto);
                await XAMLTransformToolkit.CreateDoubleAnimation(
                    PBrainBorder, "Width", null, 0, ExpansionAnimationDuration,
                    EasingFunctionNames.CircleEaseOut, true).ToStoryboard().WaitAsync();
                PBrainColumn.Width = new GridLength(0);
            }
        }

        // Indicates whether or not the touch snippets menu is currently open
        private bool _SnippetsMenuOpen;

        private async void OpenSquareBracket_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (sender is KeyboardButton button && e.HoldingState == HoldingState.Started &&
                (e.PointerDeviceType == PointerDeviceType.Touch || e.PointerDeviceType == PointerDeviceType.Pen) &&
                await Messenger.Default.RequestAsync<AppSection, CurrentAppSectionInfoRequestMessage>() == AppSection.IDE)
            {
                TouchCodeSnippetsBrowserFlyout browser = new TouchCodeSnippetsBrowserFlyout
                {
                    Height = 48 * 6 + 2, // Ugly hack (height of a snippet template by number of available templates)
                    Width = 220
                };
                await FlyoutManager.Instance.ShowCustomContextFlyout(browser, button, margin: new Point(60, 0));
            }
        }
    }
}
