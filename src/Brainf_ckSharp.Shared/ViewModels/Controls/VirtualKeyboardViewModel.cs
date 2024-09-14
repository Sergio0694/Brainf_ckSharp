using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Messages.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

#pragma warning disable IDE0290

namespace Brainf_ckSharp.Shared.ViewModels.Controls;

/// <summary>
/// A viewmodel for the virtual keyboard control.
/// </summary>
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
        this.isPBrainModeEnabled = settingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        Messenger.Register<VirtualKeyboardViewModel, ShowPBrainButtonsSettingsChangedMessage>(this, (r, m) => r.IsPBrainModeEnabled = m.Value);
    }

    /// <summary>
    /// Gets whether or not the PBrain mode is currently enabled
    /// </summary>
    [ObservableProperty]
    private bool isPBrainModeEnabled;

    /// <summary>
    /// Signals the insertion of a new operator
    /// </summary>
    /// <param name="op">The input Brainf*ck/PBrain operator</param>
    [RelayCommand]
    private void InsertOperator(char op)
    {
        _ = Messenger.Send(new OperatorKeyPressedNotificationMessage(op));
    }
}
