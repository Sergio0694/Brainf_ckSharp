using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

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
        this._ClearStdinBufferOnRequest = this.SettingsService.GetValue<bool>(SettingsKeys.ClearStdinBufferOnRequest);
        this._ShowPBrainButtons = this.SettingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);
    }

    private bool _ClearStdinBufferOnRequest;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.ClearStdinBufferOnRequest"/> setting
    /// </summary>
    public bool ClearStdinBufferOnRequest
    {
        get => this._ClearStdinBufferOnRequest;
        set => SetProperty(ref this._ClearStdinBufferOnRequest, value);
    }

    private bool _ShowPBrainButtons;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.ShowPBrainButtons"/> setting
    /// </summary>
    public bool ShowPBrainButtons
    {
        get => this._ShowPBrainButtons;
        set => SetProperty<bool, ShowPBrainButtonsSettingsChangedMessage>(ref this._ShowPBrainButtons, value);
    }
}
