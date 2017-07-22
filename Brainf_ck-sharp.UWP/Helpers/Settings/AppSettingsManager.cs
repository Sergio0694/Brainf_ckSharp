using System;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Storage;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Settings
{
    /// <summary>
    /// A simple class to manage the user application settings
    /// </summary>
    public class AppSettingsManager
    {
        #region Fields and initialization

        // Settings containers
        private readonly IPropertySet LocalSettings = ApplicationData.Current.LocalSettings.Values;
        private readonly IPropertySet RoamingSettings = ApplicationData.Current.RoamingSettings.Values;

        // Private constructor that initializes the settings containers
        private AppSettingsManager() { }

        #endregion

        /// <summary>
        /// Gets the singleton instance of the manager to use in the app
        /// </summary>
        public static AppSettingsManager Instance { get; } = new AppSettingsManager();

        #region Shared settings

        /// <summary>
        /// Assigns a value to a database key
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key</typeparam>
        /// <param name="key">The key to check</param>
        /// <param name="keyValue">The value to assign to the dictionary key</param>
        /// <param name="mode">Indicates whether or not to overwrite the setting, if already present</param>
        public void SetValue<T>(String key, T keyValue, SettingSaveMode mode)
        {
            // Roaming
            bool existing = false;
            object roaming = keyValue;
            if (!RoamingSettings.ContainsKey(key))
            {
                RoamingSettings.Add(key, keyValue);
            }
            else if (mode == SettingSaveMode.OverwriteIfExisting) RoamingSettings[key] = keyValue;
            else existing = RoamingSettings.TryGetValue(key, out roaming);

            // Local
            if (!LocalSettings.ContainsKey(key))
            {
                LocalSettings.Add(key, existing ? roaming : keyValue);
            }
            else if (mode == SettingSaveMode.OverwriteIfExisting) LocalSettings[key] = keyValue;
            else if (existing) LocalSettings[key] = roaming;
        }

        /// <summary>
        /// Gets the setting with the specified key and throws an <see cref="ArgumentException"/> if it isn't found
        /// </summary>
        /// <typeparam name="T">The type of setting to retrieve</typeparam>
        /// <param name="key">The key of the setting to get</param>
        public T GetValue<T>([NotNull] String key)
        {
            bool found = TryGetValue(key, out T value);
            if (!found) throw new ArgumentException("The setting with the given key was not present");
            return value;
        }

        /// <summary>
        /// Reads a value from the dictionary and obtains it if present
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <param name="key">The key associated to the requested object</param>
        /// <param name="value">The desired value, if found in the settings</param>
        public bool TryGetValue<T>([NotNull] String key, out T value)
        {
            // Check the roaming settings
            if (RoamingSettings.ContainsKey(key))
            {
                T temp = (T)RoamingSettings[key];
                if (!LocalSettings.ContainsKey(key)) LocalSettings.Add(key, temp);
                else LocalSettings[key] = temp;
                value = temp;
                return true;
            }

            // Check the local settings
            if (LocalSettings.ContainsKey(key))
            {
                T temp = (T)LocalSettings[key];
                RoamingSettings.Add(key, temp);
                value = temp;
                return true;
            }
            value = default(T);
            return false;
        }

        #endregion

        /// <summary>
        /// Initializes the default app settings
        /// </summary>
        public void InitializeSettings()
        {
            SetValue(nameof(AppSettingsKeys.WelcomeMessageShown), false, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.ByteOverflowModeEnabled), false, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.OverflowToggleMessageShown), false, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.ReviewPromptShown), false, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.AppStartups), 0u, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.InAppBlurMode), 0, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.AutoIndentBrackets), true, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.BracketsStyle), 0, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.TabLength), 4, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.SelectedIDETheme), 0, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.AutosaveDocuments), false, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.SelectedFontName), String.Empty, SettingSaveMode.SkipIfExisting);
            SetValue(nameof(AppSettingsKeys.ShowStatusBar), false, SettingSaveMode.SkipIfExisting);
        }

        /// <summary>
        /// Increments the current shared counter of startups for the app
        /// </summary>
        public void IncrementStartupsCount()
        {
            if (TryGetValue(nameof(AppSettingsKeys.AppStartups), out uint startups))
                SetValue(nameof(AppSettingsKeys.AppStartups), startups + 1, SettingSaveMode.OverwriteIfExisting);
        }

        /// <summary>
        /// Gets the app current version in the format "Major.Minor.Build.Revision"
        /// </summary>
        [NotNull]
        public static String AppVersion
        {
            get
            {
                PackageVersion currentVersion = Package.Current.Id.Version;
                return $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}.{currentVersion.Revision}";
            }
        }
    }
}
