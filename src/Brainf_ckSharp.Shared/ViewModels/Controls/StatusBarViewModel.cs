using Brainf_ckSharp.Shared.ViewModels.Views;
using Brainf_ckSharp.Shared.ViewModels.Views.Abstract;
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

        private WorkspaceViewModelBase? _WorkspaceViewModel;

        /// <summary>
        /// Gets the <see cref="WorkspaceViewModelBase"/> instance in use
        /// </summary>
        public WorkspaceViewModelBase? WorkspaceViewModel
        {
            get => _WorkspaceViewModel;
            private set => Set(ref _WorkspaceViewModel, value);
        }

        /// <summary>
        /// Assigns <see cref="WorkspaceViewModel"/> and <see cref="IdeViewModel"/> when the current view model changes
        /// </summary>
        /// <param name="message">The input <see cref="PropertyChangedMessage{T}"/> message to check</param>
        private void SetupActiveViewModel(PropertyChangedMessage<bool> message)
        {
            if (message.PropertyName != nameof(IsActive) ||
                !message.NewValue ||
                !(message.Sender is WorkspaceViewModelBase))
            {
                return;
            }

            WorkspaceViewModel = (WorkspaceViewModelBase)message.Sender;
        }
    }
}
