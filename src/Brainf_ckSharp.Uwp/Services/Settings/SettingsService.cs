using System;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using Brainf_ckSharp.Uwp.Constants;
using Brainf_ckSharp.Uwp.Messages.Services;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Brainf_ckSharp.Uwp.Services.Settings
{
    /// <summary>
    /// A simple <see langword="class"/> that handles the local app settings
    /// </summary>
    public sealed class SettingsService : ISettingsService
    {
        // Default settings
        public SettingsService() => EnsureDefaults();

        /// <inheritdoc/>
        public void EnsureDefaults()
        {
            SetValue(SettingsKeys.Theme, 0, false);
            SetValue(SettingsKeys.ClearStdinBufferOnRequest, true, false);
        }

        /// <summary>
        /// The <see cref="IPropertySet"/> with the settings targeted by the current instance
        /// </summary>
        private readonly IPropertySet SettingsStorage = ApplicationData.Current.LocalSettings.Values;

        /// <inheritdoc/>
        public void SetValue<T>(string key, T value, bool overwrite = true, bool notify = false)
        {
            // Convert the value
            object serializable;
            if (typeof(T).IsEnum)
            {
                Type type = Enum.GetUnderlyingType(typeof(T));
                serializable = Convert.ChangeType(value, type);
            }
            else if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
            {
                serializable = value;
            }
            else if (typeof(T) == typeof(DateTime))
            {
                serializable = Unsafe.As<T, DateTime>(ref value).ToBinary();
            }
            else throw new ArgumentException($"Invalid setting of type {typeof(T)}", nameof(value));

            // Store the new value
            if (!SettingsStorage.ContainsKey(key)) SettingsStorage.Add(key, serializable);
            else if (overwrite) SettingsStorage[key] = serializable;

            // Notify if needed
            if (notify) Messenger.Default.Send(new SettingChangedNotificationMessage<T>(key, value));
        }

        /// <inheritdoc/>
        public T GetValue<T>(string key, bool fallback = false)
        {
            // Try to get the setting value
            if (!SettingsStorage.TryGetValue(key, out object value))
            {
                if (fallback) return default;
                throw new InvalidOperationException($"The setting {key} doesn't exist");
            }

            // Cast and return the retrieved setting
            if (typeof(T) == typeof(DateTime)) value = DateTime.FromBinary(value.To<long>());
            return value.To<T>();
        }

        /// <inheritdoc/>
        public void Clear() => SettingsStorage.Clear();
    }
}
