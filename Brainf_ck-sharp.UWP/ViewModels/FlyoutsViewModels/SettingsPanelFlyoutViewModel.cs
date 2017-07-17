using System;
using System.Collections.Generic;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages.UI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations.Helpers;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class SettingsPanelFlyoutViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the collection of the available blur modes
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<String> BlurModeOptions { get; } = new[]
        {
            LocalizationManager.GetResource("BackgroundBlur"),
            LocalizationManager.GetResource("InAppBlur")
        };

        private int _BlurModeSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.InAppBlurMode));

        /// <summary>
        /// Gets or sets the selected index for the custom blur mode
        /// </summary>
        public int BlurModeSelectedIndex
        {
            get => _BlurModeSelectedIndex;
            set
            {
                if (Set(ref _BlurModeSelectedIndex, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.InAppBlurMode), value, SettingSaveMode.OverwriteIfExisting);
                    Messenger.Default.Send(new BlurModeChangedMessage(value));
                }
            }
        }

        private bool _AutoindentBrackets = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutoIndentBrackets));

        /// <summary>
        /// Gets or sets whether or not the IDE should automatically indents new brackets
        /// </summary>
        public bool AutoindentBrackets
        {
            get => _AutoindentBrackets;
            set
            {
                if (Set(ref _AutoindentBrackets, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.AutoIndentBrackets), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets the collection of the available tab lengths
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<int> TabLengthOptions { get; } = new[] { 4, 6, 8, 10, 12 };

        private int _TabLengthSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength));

        /// <summary>
        /// Gets or sets the selected index for the tab length setting
        /// </summary>
        public int TabLengthSelectedIndex
        {
            get => _TabLengthSelectedIndex;
            set
            {
                if (Set(ref _TabLengthSelectedIndex, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.TabLength), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets the collection of the available brackets styles
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<String> BracketsStyleOptions { get; } = new[]
        {
            LocalizationManager.GetResource("NewLine"),
            LocalizationManager.GetResource("SameLine")
        };

        private int _BracketsStyleSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.BracketsStyle));

        /// <summary>
        /// Gets or sets the selected index for the custom brackets style
        /// </summary>
        public int BracketsStyleSelectedIndex
        {
            get => _BracketsStyleSelectedIndex;
            set
            {
                if (Set(ref _BracketsStyleSelectedIndex, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.BracketsStyle), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets the collection of the available IDE themes
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<SelectableIDEThemeInfo> AvailableIDEThemes { get; } = new[]
        {
            new SelectableIDEThemeInfo(CodeThemes.Default),
            new SelectableIDEThemeInfo(CodeThemes.Monokai),
            new SelectableIDEThemeInfo(CodeThemes.Dracula),
            new SelectableIDEThemeInfo(CodeThemes.Vim)
        };

        private int _IDEThemeSelectedIndex = 0;

        /// <summary>
        /// Gets or sets the selected index for the IDE theme
        /// </summary>
        public int IDEThemeSelectedIndex
        {
            get => _IDEThemeSelectedIndex;
            set
            {
                if (Set(ref _IDEThemeSelectedIndex, value))
                {
                    //AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.BracketsStyle), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current device is not a mobile phone
        /// </summary>
        public bool HostBlurOptionSupported => !ApiInformationHelper.IsMobileDevice;
    }
}
