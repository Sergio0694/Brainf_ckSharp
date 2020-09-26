using System.Collections.Generic;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections
{
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
            _OverflowMode = SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode);
            _MemorySize = SettingsService.GetValue<int>(SettingsKeys.MemorySize);
        }

        private OverflowMode _OverflowMode;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.OverflowMode"/> setting
        /// </summary>
        public OverflowMode OverflowMode
        {
            get => _OverflowMode;
            set => SetProperty<OverflowMode, OverflowModeSettingChangedMessage>(ref _OverflowMode, value);
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
            get => _MemorySize;
            set => SetProperty<int, MemorySizeSettingChangedMessage>(ref _MemorySize, value);
        }
    }
}
