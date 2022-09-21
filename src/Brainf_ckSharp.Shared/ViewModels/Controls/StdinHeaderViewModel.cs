using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls;

public sealed class StdinHeaderViewModel : ObservableRecipient
{
    /// <summary>
    /// The <see cref="ISettingsService"/> instance currently in use
    /// </summary>
    private readonly ISettingsService SettingsService;

    /// <summary>
    /// Creates a new <see cref="StdinHeaderViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public StdinHeaderViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger)
    {
        SettingsService = settingsService;
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        Messenger.Register<StdinHeaderViewModel, StdinRequestMessage>(this, (r, m) => r.GetStdinBuffer(m));
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
    private void GetStdinBuffer(StdinRequestMessage request)
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
