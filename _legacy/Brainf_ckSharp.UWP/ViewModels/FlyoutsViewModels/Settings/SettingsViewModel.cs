using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Services.Store;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc.Themes;
using Brainf_ck_sharp.Legacy.UWP.Helpers;
using Brainf_ck_sharp.Legacy.UWP.Helpers.CodeFormatting;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Settings;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ck_sharp.Legacy.UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp.Legacy.UWP.Messages.Settings;
using Brainf_ck_sharp.Legacy.UWP.Messages.UI;
using Brainf_ck_sharp.Legacy.UWP.PopupService;
using Brainf_ck_sharp.Legacy.UWP.PopupService.Misc;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

#if !DEBUG
using System.Linq; //Used to check the license
#endif

namespace Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            IDEThemeSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme));
            AvailableIDEThemes[IDEThemeSelectedIndex].IsSelected = true;
            string fontName = AppSettingsManager.Instance.GetValue<string>(nameof(AppSettingsKeys.SelectedFontName));
            int index = AvailableFonts.IndexOf(f => f.Name.Equals(string.IsNullOrEmpty(fontName) ? "Segoe UI" : fontName));
            _FontFamilySelectedIndex = index != -1 ? index : AvailableFonts.IndexOf(f => f.Name.Equals("Segoe UI")); // Fallback when the selected font isn't available
            if (!ThemesSelectorEnabled)
            {
                // Update the add-on license when needed
                UpdateLicenseInfoAsync().Forget();
            }
        }

        private bool _AutoindentBrackets = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutoIndentBrackets));

        /// <summary>
        /// Gets or sets whether or not the IDE should automatically indents new brackets
        /// </summary>
        [UsedImplicitly]
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
        [UsedImplicitly, NotNull]
        public IReadOnlyCollection<int> TabLengthOptions { get; } = new[] { 4, 6, 8, 10, 12 };

        private int _TabLengthSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength));

        /// <summary>
        /// Gets or sets the selected index for the tab length setting
        /// </summary>
        [UsedImplicitly]
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
        [UsedImplicitly, NotNull]
        public IReadOnlyCollection<string> BracketsStyleOptions { get; } = new[]
        {
            LocalizationManager.GetResource("NewLine"),
            LocalizationManager.GetResource("SameLine")
        };

        private int _BracketsStyleSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.BracketsStyle));

        /// <summary>
        /// Gets or sets the selected index for the custom brackets style
        /// </summary>
        [UsedImplicitly]
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

        private bool _RenderWhitespaces = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));

        /// <summary>
        /// Gets or sets whether or not the IDE should render the control characters too
        /// </summary>
        [UsedImplicitly]
        public bool RenderWhitespaces
        {
            get => _RenderWhitespaces;
            set
            {
                if (Set(ref _RenderWhitespaces, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.RenderWhitespaces), value, SettingSaveMode.OverwriteIfExisting);
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
        [UsedImplicitly]
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
        private const string ThemesPackID = "9p4q63ccfpbm";

        // The Store instance to use
        private static StoreContext _StoreContext;

        // Updates the license for the themes pack
        private async Task UpdateLicenseInfoAsync()
        {
            // Roaming check
            if (AppSettingsManager.Instance.TryGetValue(ThemesPackID, out bool backupLicense) && backupLicense)
            {
                ThemesSelectorEnabled = true;
                return;
            }

            // Setup the async loading
            ThemesPackLicenseLoading = true;
            bool iapAvailable;
#if DEBUG
            await Task.Delay(500);
            iapAvailable = true;
#else
            if (_StoreContext == null) _StoreContext = await Task.Run(() => StoreContext.GetDefault()); // Avoid UI hangs
            StoreAppLicense license = await _StoreContext.GetAppLicenseAsync();
            iapAvailable = license?.AddOnLicenses.FirstOrDefault(pair => pair.Key.Equals(ThemesPackID)).Value?.IsActive == true;
#endif

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            ThemesSelectorEnabled = iapAvailable;
            ThemesPackLicenseLoading = false;
        }

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
                    AppSettingsManager.Instance.SetValue(ThemesPackID, true, SettingSaveMode.OverwriteIfExisting);
                    break;
                case StorePurchaseStatus.NotPurchased:
                    NotificationsManager.Instance.ShowDefaultErrorNotification(LocalizationManager.GetResource("PurchaseCanceled"), LocalizationManager.GetResource("PurchaseCanceledBody"));
                    break;
                case StorePurchaseStatus.AlreadyPurchased:
                    ThemesSelectorEnabled = true;
                    AppSettingsManager.Instance.SetValue(ThemesPackID, true, SettingSaveMode.OverwriteIfExisting);
                    break;
                default:
                    // Error
                    NotificationsManager.Instance.ShowDefaultErrorNotification($"{LocalizationManager.GetResource("SomethingBadHappened")} :'(", LocalizationManager.GetResource("PurchaseErrorBody"));
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

        private bool _TimelineLoggingEnabled = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.EnableTimeline));

        /// <summary>
        /// Gets or sets whether or not the IDE should render the control characters too
        /// </summary>
        [UsedImplicitly]
        public bool TimelineLoggingEnabled
        {
            get => _TimelineLoggingEnabled;
            set
            {
                if (Set(ref _TimelineLoggingEnabled, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.EnableTimeline), value, SettingSaveMode.OverwriteIfExisting);
                    TimelineManager.IsEnabled = value;
                }
            }
        }

        private bool _AutosaveDocuments = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutosaveDocuments));

        /// <summary>
        /// Gets or sets whether or not the IDE should automatically save the current document when leaving the app
        /// </summary>
        [UsedImplicitly]
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

        private bool _ProtectUnsavedChanges = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ProtectUnsavedChanges));

        /// <summary>
        /// Gets or sets whether or not to ask for confirmation when deleting unsaved changes in the IDE
        /// </summary>
        [UsedImplicitly]
        public bool ProtectUnsavedChanges
        {
            get => _ProtectUnsavedChanges;
            set
            {
                if (Set(ref _ProtectUnsavedChanges, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ProtectUnsavedChanges), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        private bool _ShowPBrainButtons = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ShowPBrainButtons));

        /// <summary>
        /// Gets or sets whether or not the PBrain buttons should be visible in the virtual keyboard
        /// </summary>
        [UsedImplicitly]
        public bool ShowPBrainButtons
        {
            get => _ShowPBrainButtons;
            set
            {
                if (Set(ref _ShowPBrainButtons, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ShowPBrainButtons), value, SettingSaveMode.OverwriteIfExisting);
                    Messenger.Default.Send(new PBrainButtonsVisibilityChangedMessage(value));
                }
            }
        }

        private bool _ClearStdinBufferOnExecution = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ClearStdinBufferOnExecution));

        /// <summary>
        /// Gets or sets whether or not to clear the Stdin buffer when executing a script
        /// </summary>
        [UsedImplicitly]
        public bool ClearStdinBufferOnExecution
        {
            get => _ClearStdinBufferOnExecution;
            set
            {
                if (Set(ref _ClearStdinBufferOnExecution, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ClearStdinBufferOnExecution), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets the two options for the initial page
        /// </summary>
        [UsedImplicitly, NotNull]
        public IReadOnlyCollection<string> StartingPageOptions { get; } = new[]
        {
            "Console",
            "IDE"
        };

        private int _StartingPageSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.StartingPage));

        /// <summary>
        /// Gets or sets the selected index for the app starting page
        /// </summary>
        [UsedImplicitly]
        public int StartingPageSelectedIndex
        {
            get => _StartingPageSelectedIndex;
            set
            {
                if (Set(ref _StartingPageSelectedIndex, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.StartingPage), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        /// <summary>
        /// Gets the collection of the available memory sizes
        /// </summary>
        [UsedImplicitly, NotNull]
        public IReadOnlyCollection<int> MemorySizeOptions { get; } = new[] { 32, 48, 64 };

        private int _MemorySizeSelectedIndex = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.InterpreterMemorySize));

        /// <summary>
        /// Gets or sets the selected index for the interpreter memory size
        /// </summary>
        [UsedImplicitly]
        public int MemorySizeSelectedIndex
        {
            get => _MemorySizeSelectedIndex;
            set
            {
                if (Set(ref _MemorySizeSelectedIndex, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.InterpreterMemorySize), value, SettingSaveMode.OverwriteIfExisting);
                }
            }
        }

        private bool _AutorunCodeInBackground = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.AutorunCodeInBackground));

        /// <summary>
        /// Gets or sets whether or not the app should periodically execute the code in the background
        /// </summary>
        [UsedImplicitly]
        public bool AutorunCodeInBackground
        {
            get => _AutorunCodeInBackground;
            set
            {
                if (Set(ref _AutorunCodeInBackground, value))
                {
                    AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.AutorunCodeInBackground), value, SettingSaveMode.OverwriteIfExisting);
                    Brainf_ckBackgroundExecutor.Instance.IsEnabled = value;
                    if (!value) Messenger.Default.Send(new BackgroundExecutionDisabledMessage());
                }
            }
        }
    }
}
