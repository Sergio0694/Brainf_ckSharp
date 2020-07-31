using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls
{
    public sealed class StdinHeaderViewModel : ObservableRecipient
    {
        /// <summary>
        /// The <see cref="ISettingsService"/> instance currently in use
        /// </summary>
        private readonly ISettingsService SettingsService = Ioc.Default.GetRequiredService<ISettingsService>();

        /// <summary>
        /// Creates a new <see cref="StdinHeaderViewModel"/> instance
        /// </summary>
        public StdinHeaderViewModel()
        {
            Messenger.Register<StdinRequestMessage>(this, ExtractStdinBuffer);
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

        /// <summary>
        /// Handles a request for the current stdin buffer
        /// </summary>
        /// <param name="request">The input request message for the stdin buffer</param>
        private void ExtractStdinBuffer(StdinRequestMessage request)
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
