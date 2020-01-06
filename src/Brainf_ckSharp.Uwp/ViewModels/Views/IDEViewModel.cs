using System;
using System.Threading.Tasks;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Uwp.Messages.Ide;
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
        /// Raised whenever a new character is requested to be added to the current text
        /// </summary>
        public event EventHandler<char>? CharacterAdded;

        /// <summary>
        /// Raised whenever a character is requested to be deleted
        /// </summary>
        public event EventHandler? CharacterDeleted;

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Messenger.Default.Register<OperatorKeyPressedNotificationMessage>(this, m => CharacterAdded?.Invoke(this, m));
            Messenger.Default.Register<InsertNewLineRequestMessage>(this, _ => CharacterAdded?.Invoke(this, Characters.CarriageReturn));
            Messenger.Default.Register<DeleteCharacterRequestMessage>(this, _ => CharacterDeleted?.Invoke(this, EventArgs.Empty));
        }

        private async Task TryOpenFileAsync()
        {

        }
    }
}
