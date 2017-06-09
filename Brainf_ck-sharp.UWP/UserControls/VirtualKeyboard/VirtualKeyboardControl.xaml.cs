using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp_UWP.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard
{
    /// <summary>
    /// A <see cref="UserControl"/> with a 2-rows grid that hosts virtual buttons for the 8 Brainf_ck operators
    /// </summary>
    public sealed partial class VirtualKeyboardControl : UserControl
    {
        public VirtualKeyboardControl()
        {
            this.InitializeComponent();
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

        private void NextButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed('>');

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed('<');

        private void PlusButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed('+');

        private void MinusButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed('-');

        private void OpenBracketButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed('[');

        private void CloseBracketButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed(']');

        private void PointButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed('.');

        private void CommaButton_Tapped(object sender, TappedRoutedEventArgs e) => OnKeyPressed(',');

        #endregion
    }
}
