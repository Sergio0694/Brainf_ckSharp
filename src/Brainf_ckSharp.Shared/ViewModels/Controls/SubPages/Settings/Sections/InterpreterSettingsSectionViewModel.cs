using System;
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
        this.dataType = this.SettingsService.GetValue<DataType>(SettingsKeys.DataType);
        this.executionOptions = this.SettingsService.GetValue<ExecutionOptions>(SettingsKeys.ExecutionOptions);
        this.memorySize = this.SettingsService.GetValue<int>(SettingsKeys.MemorySize);
    }

    /// <summary>
    /// Gets the available data types
    /// </summary>
    public IReadOnlyCollection<DataType> DataTypes { get; } = (DataType[])Enum.GetValues(typeof(DataType));

    private DataType dataType;

    /// <summary>
    /// Exposes the <see cref="SettingsKeys.DataType"/> setting
    /// </summary>
    public DataType DataType
    {
        get => this.dataType;
        set => SetProperty<DataType, DataTypeSettingChangedMessage>(ref this.dataType, value);
    }

    private ExecutionOptions executionOptions;

    /// <summary>
    /// Exposes the <see cref="ExecutionOptions.AllowOverflow"/> setting
    /// </summary>
    public bool IsOverflowEnabled
    {
        get => this.executionOptions.HasFlag(ExecutionOptions.AllowOverflow);
        set
        {
            if (SetProperty(
                ref this.executionOptions,
                this.executionOptions | (value ? ExecutionOptions.AllowOverflow : ExecutionOptions.None)))
            {
                _ = Messenger.Send(new ExecutionOptionsSettingChangedMessage(this.executionOptions));
            }
        }
    }

    /// <summary>
    /// Gets the collection of the available tab lengths
    /// </summary>
    public IReadOnlyCollection<int> MemorySizeOptions { get; } = [32, 64, 128, 256];

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
