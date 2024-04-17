using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

/// <summary>
/// A viewmodel for the UI settings section.
/// </summary>
public sealed class UISettingsSectionViewModel : SettingsSectionViewModelBase
{
    /// <summary>
    /// Creates a new <see cref="UISettingsSectionViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public UISettingsSectionViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger, settingsService)
    {
        this.clearStdinBufferOnRequest = this.SettingsService.GetValue<bool>(SettingsKeys.ClearStdinBufferOnRequest);
        this.showPBrainButtons = this.SettingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);
    }

    private bool clearStdinBufferOnRequest;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.ClearStdinBufferOnRequest"/> setting
    /// </summary>
    public bool ClearStdinBufferOnRequest
    {
        get => this.clearStdinBufferOnRequest;
        set => SetProperty(ref this.clearStdinBufferOnRequest, value);
    }

    private bool showPBrainButtons;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.ShowPBrainButtons"/> setting
    /// </summary>
    public bool ShowPBrainButtons
    {
        get => this.showPBrainButtons;
        set => SetProperty<bool, ShowPBrainButtonsSettingsChangedMessage>(ref this.showPBrainButtons, value);
    }
}
