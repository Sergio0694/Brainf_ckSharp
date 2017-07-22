using System;
using System.Collections.Generic;
#if !DEBUG
using System.Linq;
#endif
using System.Threading.Tasks;
using Windows.Services.Store;
using Brainf_ck_sharp_UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using UICompositionAnimations.Helpers;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class SettingsPanelFlyoutViewModel : ViewModelBase
    {
        public SettingsPanelFlyoutViewModel()
        {
            IDEThemeSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme));
            AvailableIDEThemes[IDEThemeSelectedIndex].IsSelected = true;
            String fontName = AppSettingsManager.Instance.GetValue<String>(nameof(AppSettingsKeys.SelectedFontName));
            _FontFamilySelectedIndex = AvailableFonts.IndexOf(f => f.Name.Equals(String.IsNullOrEmpty(fontName) ? "Segoe UI" : fontName));
            if (!ThemesSelectorEnabled)
            {
                // Update the add-on license when needed
                UpdateLicenseInfoAsync().Forget();
            }
        }

        private bool _AutosaveDocuments = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutosaveDocuments));

        /// <summary>
        /// Gets or sets whether or not the IDE should automatically save the current document when leaving the app
        /// </summary>
        public bool AutosaveDocuments
        {
            get => _AutosaveDocuments;
            set
            {
                if (Set(ref _AutosaveDocuments, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.AutosaveDocuments), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

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
        /// Gets the collection of the available font families for the IDE
        /// </summary>
        [NotNull]
        public IReadOnlyList<InstalledFont> AvailableFonts { get; } = InstalledFont.Fonts;

        private int _FontFamilySelectedIndex;

        /// <summary>
        /// Gets or sets the selected index for the IDE font family
        /// </summary>
        public int FontFamilySelectedIndex
        {
            get => _FontFamilySelectedIndex;
            set
            {
                if (Set(ref _FontFamilySelectedIndex, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.SelectedFontName), AvailableFonts[value].Name, SettingSaveMode.OverwriteIfExisting);
                    Messenger.Default.Send(new IDEThemePreviewFontChangedMessage(AvailableFonts[value]));
                }
            }
        }

        /// <summary>
        /// Gets the collection of the available IDE themes
        /// </summary>
        [NotNull]
        public IReadOnlyList<SelectableIDEThemeInfo> AvailableIDEThemes { get; } = new[]
        {
            new SelectableIDEThemeInfo(CodeThemes.Default),
            new SelectableIDEThemeInfo(CodeThemes.Monokai),
            new SelectableIDEThemeInfo(CodeThemes.Dracula),
            new SelectableIDEThemeInfo(CodeThemes.Vim),
            new SelectableIDEThemeInfo(CodeThemes.OneDark),
            new SelectableIDEThemeInfo(CodeThemes.Base16),
            new SelectableIDEThemeInfo(CodeThemes.VisualStudioCode)
        };

        private static bool _ThemesSelectorEnabled;

        /// <summary>
        /// Gets whether or not the themes selector is enabled
        /// </summary>
        public bool ThemesSelectorEnabled
        {
            get => _ThemesSelectorEnabled;
            private set => Set(ref _ThemesSelectorEnabled, value);
        }

        private static bool _ThemesUnlockButtonEnabled;

        /// <summary>
        /// Gets whether or not the button to unlock the themes pack is enabled
        /// </summary>
        public bool ThemesUnlockButtonEnabled
        {
            get => _ThemesUnlockButtonEnabled;
            private set => Set(ref _ThemesUnlockButtonEnabled, value);
        }

        private bool _ThemesPackLicenseLoading;

        /// <summary>
        /// Gets whether or not the themes pack license is currently available
        /// </summary>
        public bool ThemesPackLicenseLoading
        {
            get => _ThemesPackLicenseLoading;
            private set => Set(ref _ThemesPackLicenseLoading, value);
        }

        // The Store ID of the themes pack
        private const String ThemesPackID = "9p4q63ccfpbm";

        // The Store instance to use
        private static StoreContext _StoreContext;

        // Updates the license for the themes pack
#if DEBUG
        private async Task UpdateLicenseInfoAsync()
        {
            ThemesPackLicenseLoading = true;
            await Task.Delay(500);
            ThemesSelectorEnabled = true;
            ThemesUnlockButtonEnabled = false;
            ThemesPackLicenseLoading = false;
        }
#else
        private async Task UpdateLicenseInfoAsync()
        {
            ThemesPackLicenseLoading = true;
            if (_StoreContext == null) _StoreContext = StoreContext.GetDefault();
            StoreAppLicense license = await _StoreContext.GetAppLicenseAsync();
            ThemesSelectorEnabled = license?.AddOnLicenses.FirstOrDefault(pair => pair.Key.Equals(ThemesPackID)).Value?.IsActive == true;
            ThemesUnlockButtonEnabled = !ThemesSelectorEnabled;
            ThemesPackLicenseLoading = false;
        }
#endif

        /// <summary>
        /// Tries to purchase the additional themes pack
        /// </summary>
        public async void TryPurchaseThemesPackAsync()
        {
            // Try to purchase the item
            Messenger.Default.Send(new AppLoadingStatusChangedMessage(true));
            StorePurchaseResult result;
            if (_StoreContext == null) _StoreContext = StoreContext.GetDefault();
            try
            {
                result = await _StoreContext.RequestPurchaseAsync(ThemesPackID);
            }
            catch
            {
                NotificationsManager.Instance.ShowDefaultErrorNotification(
                    LocalizationManager.GetResource("StoreConnectionError"), LocalizationManager.GetResource("StoreConnectionErrorBody"));
                return;
            }
            finally
            {
                Messenger.Default.Send(new AppLoadingStatusChangedMessage(false));
            }

            // Display the result
            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                    NotificationsManager.Instance.ShowNotification(0xEC24.ToSegoeMDL2Icon(), LocalizationManager.GetResource("ThemesUnlocked"),
                        LocalizationManager.GetResource("DonationCompletedBody"), NotificationType.Default);
                    ThemesSelectorEnabled = true;
                    break;
                case StorePurchaseStatus.NotPurchased:
                    NotificationsManager.Instance.ShowDefaultErrorNotification(LocalizationManager.GetResource("PurchaseCanceled"), LocalizationManager.GetResource("PurchaseCanceledBody"));
                    break;
                case StorePurchaseStatus.AlreadyPurchased:
                    ThemesSelectorEnabled = true;
                    break;
                default:
                    // Error
                    NotificationsManager.Instance.ShowDefaultErrorNotification($"{LocalizationManager.GetResource("SomethingBadHappened")} :'(", LocalizationManager.GetResource("DonationErrorBody"));
                    break;
            }
        }

        private int _IDEThemeSelectedIndex;

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
                    for (int i = 0; i < AvailableIDEThemes.Count; i++)
                        AvailableIDEThemes[i].IsSelected = i == value;
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.SelectedIDETheme), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current device is not a mobile phone
        /// </summary>
        public bool HostBlurOptionSupported => !ApiInformationHelper.IsMobileDevice;

        private bool _ShowStatusBar = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ShowStatusBar));

        /// <summary>
        /// Gets or sets whether or not the status bar should be displayed on mobile phones
        /// </summary>
        public bool ShowStatusBar
        {
            get => _ShowStatusBar;
            set
            {
                if (Set(ref _ShowStatusBar, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ShowStatusBar), value, SettingSaveMode.OverwriteIfExisting);
                    if (value) StatusBarHelper.TryShowAsync().Forget();
                    else StatusBarHelper.HideAsync().Forget();
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current device is a mobile phone
        /// </summary>
        public bool StatusBarSupported => ApiInformationHelper.IsMobileDevice;
    }
}
