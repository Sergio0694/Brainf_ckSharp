using System.Collections.Generic;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections
{
    public sealed class InterpreterSettingsSectionViewModel : SettingsSectionViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="InterpreterSettingsSectionViewModel"/> instance
        /// </summary>
        public InterpreterSettingsSectionViewModel()
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
