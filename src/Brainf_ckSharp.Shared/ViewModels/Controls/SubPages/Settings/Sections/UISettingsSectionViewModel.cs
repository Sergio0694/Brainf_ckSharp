using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections.Abstract;

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages.Settings.Sections
{
    public sealed class UISettingsSectionViewModel : SettingsSectionViewModelBase
    {
        /// <summary>
        /// Creates a new <see cref="UISettingsSectionViewModel"/> instance
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
        public UISettingsSectionViewModel(ISettingsService settingsService) : base(settingsService)
        {
            _ClearStdinBufferOnRequest = SettingsService.GetValue<bool>(SettingsKeys.ClearStdinBufferOnRequest);
            _ShowPBrainButtons = SettingsService.GetValue<bool>(SettingsKeys.ShowPBrainButtons);
        }

        private bool _ClearStdinBufferOnRequest;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ClearStdinBufferOnRequest"/> setting
        /// </summary>
        public bool ClearStdinBufferOnRequest
        {
            get => _ClearStdinBufferOnRequest;
            set => SetProperty(ref _ClearStdinBufferOnRequest, value);
        }

        private bool _ShowPBrainButtons;

        /// <summary>
        /// Exposes the <see cref="SettingsKeys.ShowPBrainButtons"/> setting
        /// </summary>
        public bool ShowPBrainButtons
        {
            get => _ShowPBrainButtons;
            set => SetProperty<bool, ShowPBrainButtonsSettingsChangedMessage>(ref _ShowPBrainButtons, value);
        }
    }
}
