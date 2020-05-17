using Brainf_ckSharp.Shared.ViewModels.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.ViewModels.Controls
{
    public sealed class StatusBarViewModel : ViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="StatusBarViewModel"/> instance
        /// </summary>
        public StatusBarViewModel()
        {
            Messenger.Register<PropertyChangedMessage<bool>>(this, SetupActiveViewModel);
        }

        /// <summary>
        /// Gets the <see cref="Views.ConsoleViewModel"/> instance in use, if it's active
        /// </summary>
        public ConsoleViewModel? ConsoleViewModel { get; private set; }

        /// <summary>
        /// Gets the <see cref="Views.IdeViewModel"/> instance in use, if it's active
        /// </summary>
        public IdeViewModel? IdeViewModel { get; private set; }

        /// <summary>
        /// Assigns <see cref="ConsoleViewModel"/> and <see cref="IdeViewModel"/> when the current view model changes
        /// </summary>
        /// <param name="message">The input <see cref="PropertyChangedMessage{T}"/> message to check</param>
        private void SetupActiveViewModel(PropertyChangedMessage<bool> message)
        {
            if (message.PropertyName != nameof(IsActive) || !message.NewValue)
            {
                return;
            }

            ConsoleViewModel = message.Sender as ConsoleViewModel;
            IdeViewModel = message.Sender as IdeViewModel;

            OnPropertyChanged(nameof(ConsoleViewModel));
            OnPropertyChanged(nameof(IdeViewModel));
        }
    }
}
