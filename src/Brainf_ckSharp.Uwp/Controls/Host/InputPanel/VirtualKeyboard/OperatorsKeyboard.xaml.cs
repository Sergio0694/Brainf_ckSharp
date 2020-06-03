using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared;
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
    }
}
