using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel.VirtualKeyboard
{
    /// <summary>
    /// A virtual keyboard with the Brainf*ck/PBrain operators
    /// </summary>
    public sealed partial class OperatorsKeyboard : UserControl
    {
        public OperatorsKeyboard()
        {
            this.InitializeComponent();

            bool showPBrainOperators = Ioc.Default.GetRequiredService<ISettingsService>().GetValue<bool>(SettingsKeys.ShowPBrainButtons);

            VisualStateManager.GoToState(this, showPBrainOperators ? nameof(ExtendedState) : nameof(MinimalState), false);

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, UpdateKeyboardVisualState);
        }

        /// <summary>
        /// Updates the UI when the settings for the virtual keyboard is changed
        /// </summary>
        /// <param name="message">The <see cref="PropertyChangedMessage{T}"/> instance to receive</param>
        private void UpdateKeyboardVisualState(PropertyChangedMessage<bool> message)
        {
            if (message.Sender.GetType() == typeof(SettingsSubPageViewModel) &&
                message.PropertyName == nameof(SettingsKeys.ShowPBrainButtons))
            {
                VisualStateManager.GoToState(this, message.NewValue ? nameof(ExtendedState) : nameof(MinimalState), false);
            }
        }

        /// <summary>
        /// Handles a given operator button being clicked
        /// </summary>
        /// <param name="sender">An <see cref="OperatorButton"/> instance with the input operator</param>
        /// <param name="e">Unused event args, the input operator is in the <see cref="OperatorButton.Operator"/> property</param>
        private void OperatorButton_OnClick(object sender, EventArgs e)
        {
            if (sender is OperatorButton button)
            {
                Debug.Assert(button.Operator != null);
                Debug.Assert(button.Operator.Length == 1);
                Debug.Assert(Brainf_ckParser.IsOperator(button.Operator[0]));

                char op = button.Operator[0];

                Messenger.Default.Send(new OperatorKeyPressedNotificationMessage(op));
            }
            else throw new InvalidOperationException("Invalid operator button pressed");
        }
    }
}
