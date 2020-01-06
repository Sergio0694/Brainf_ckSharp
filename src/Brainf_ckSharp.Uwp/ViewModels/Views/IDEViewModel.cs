using System;
using Brainf_ckSharp.Uwp.Messages.InputPanel;
using Brainf_ckSharp.Uwp.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Views
{
    /// <summary>
    /// A view model for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed class IdeViewModel : ReactiveViewModelBase
    {
        /// <summary>
        /// Raised whenever a new operator is requested to be added to the current text
        /// </summary>
        public event EventHandler<char>? OperatorAdded; 

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Messenger.Default.Register<OperatorKeyPressedNotificationMessage>(this, m => OperatorAdded?.Invoke(this, m));
        }
    }
}
