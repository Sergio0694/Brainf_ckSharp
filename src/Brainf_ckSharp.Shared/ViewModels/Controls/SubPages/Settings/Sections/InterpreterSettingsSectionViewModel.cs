using System.Collections.Generic;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

public sealed class InterpreterSettingsSectionViewModel : SettingsSectionViewModelBase
{
    /// <summary>
    /// Creates a new <see cref="InterpreterSettingsSectionViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public InterpreterSettingsSectionViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger, settingsService)
    {
        this._OverflowMode = this.SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode);
        this._MemorySize = this.SettingsService.GetValue<int>(SettingsKeys.MemorySize);
    }

    /// <summary>
    /// Gets the available overflow modes.
    /// </summary>
    public IReadOnlyCollection<OverflowMode> OverflowModes { get; } = (OverflowMode[])typeof(OverflowMode).GetEnumValues();

    private OverflowMode _OverflowMode;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.OverflowMode"/> setting
    /// </summary>
    public OverflowMode OverflowMode
    {
        get => this._OverflowMode;
        set => SetProperty<OverflowMode, OverflowModeSettingChangedMessage>(ref this._OverflowMode, value);
    }

    /// <summary>
    /// Gets the collection of the available tab lengths
    /// </summary>
    public IReadOnlyCollection<int> MemorySizeOptions { get; } = new[] { 32, 64, 128, 256 };

    private int _MemorySize;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.MemorySize"/> setting
    /// </summary>
    public int MemorySize
    {
        get => this._MemorySize;
        set => SetProperty<int, MemorySizeSettingChangedMessage>(ref this._MemorySize, value);
    }
}
