using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Messages.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls;

public sealed partial class VirtualKeyboardViewModel : ObservableRecipient
{
    /// <summary>
    /// Creates a new <see cref="VirtualKeyboardViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public VirtualKeyboardViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger)
    {
        this._IsPBrainModeEnabled = settingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        Messenger.Register<VirtualKeyboardViewModel, ShowPBrainButtonsSettingsChangedMessage>(this, (r, m) => r.IsPBrainModeEnabled = m.Value);
    }

    private bool _IsPBrainModeEnabled;

    /// <summary>
    /// Gets whether or not the PBrain mode is currently enabled
    /// </summary>
    public bool IsPBrainModeEnabled
    {
        get => this._IsPBrainModeEnabled;
        private set => SetProperty(ref this._IsPBrainModeEnabled, value);
    }

    /// <summary>
    /// Signals the insertion of a new operator
    /// </summary>
    /// <param name="op">The input Brainf*ck/PBrain operator</param>
    [RelayCommand]
    private void InsertOperator(char op)
    {
        Messenger.Send(new OperatorKeyPressedNotificationMessage(op));
    }
}
