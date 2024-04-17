using System.Collections.Generic;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using CommunityToolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections;

/// <summary>
/// A viewmodel for the interpreter settings section.
/// </summary>
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
        this.overflowMode = this.SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode);
        this.memorySize = this.SettingsService.GetValue<int>(SettingsKeys.MemorySize);
    }

    /// <summary>
    /// Gets the available overflow modes.
    /// </summary>
    public IReadOnlyCollection<OverflowMode> OverflowModes { get; } = (OverflowMode[])typeof(OverflowMode).GetEnumValues();

    private OverflowMode overflowMode;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.OverflowMode"/> setting
    /// </summary>
    public OverflowMode OverflowMode
    {
        get => this.overflowMode;
        set => SetProperty<OverflowMode, OverflowModeSettingChangedMessage>(ref this.overflowMode, value);
    }

    /// <summary>
    /// Gets the collection of the available tab lengths
    /// </summary>
    public IReadOnlyCollection<int> MemorySizeOptions { get; } = new[] { 32, 64, 128, 256 };

    private int memorySize;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.MemorySize"/> setting
    /// </summary>
    public int MemorySize
    {
        get => this.memorySize;
        set => SetProperty<int, MemorySizeSettingChangedMessage>(ref this.memorySize, value);
    }
}
