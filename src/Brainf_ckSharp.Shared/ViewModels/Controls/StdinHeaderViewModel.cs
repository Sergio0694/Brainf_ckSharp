using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls
{
    public sealed class StdinHeaderViewModel : ObservableRecipient, IRecipient<StdinRequestMessage>
    {
        /// <summary>
        /// The <see cref="ISettingsService"/> instance currently in use
        /// </summary>
        private readonly ISettingsService SettingsService;

        /// <summary>
        /// Creates a new <see cref="StdinHeaderViewModel"/> instance
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
        public StdinHeaderViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;
        }

        private string _Text = string.Empty;

        /// <summary>
        /// Gets or sets the current text in the stdin buffer
        /// </summary>
        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }

        /// <inheritdoc/>
        void IRecipient<StdinRequestMessage>.Receive(StdinRequestMessage request)
        {
            request.Reply(Text);

            // Clear the buffer if requested, and if not from a background execution
            if (!request.IsFromBackgroundExecution &&
                SettingsService.GetValue<bool>(SettingsKeys.ClearStdinBufferOnRequest))
            {
                Text = string.Empty;
            }
        }
    }
}
