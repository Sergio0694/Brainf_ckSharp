using System;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;
using Brainf_ckSharp.Services;
using CommunityToolkit.Diagnostics;

namespace Brainf_ckSharp.Uwp.Services.Settings;

/// <summary>
/// A simple <see langword="class"/> that handles the local app settings
/// </summary>
public sealed class SettingsService : ISettingsService
{
    /// <summary>
    /// The <see cref="IPropertySet"/> with the settings targeted by the current instance
    /// </summary>
    private readonly IPropertySet SettingsStorage = ApplicationData.Current.LocalSettings.Values;

    /// <inheritdoc/>
    public void SetValue<T>(string key, T value, bool overwrite = true)
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
            serializable = value!;
        }
        else if (typeof(T) == typeof(DateTime))
        {
            serializable = Unsafe.As<T, DateTime>(ref value).ToBinary();
        }
        else
        {
            ThrowHelper.ThrowArgumentException("Invalid setting type");

            return;
        }

        // Store the new value
        if (!this.SettingsStorage.ContainsKey(key)) this.SettingsStorage.Add(key, serializable);
        else if (overwrite) this.SettingsStorage[key] = serializable;
    }

    /// <inheritdoc/>
    public T GetValue<T>(string key, bool fallback = false)
    {
        // Try to get the setting value
        if (!this.SettingsStorage.TryGetValue(key, out object value))
        {
            if (fallback) return default!;

            ThrowHelper.ThrowArgumentException("The setting with the given key does not exist");
        }

        // Cast and return the retrieved setting
        if (typeof(T) == typeof(DateTime)) value = DateTime.FromBinary((long)value);

        return (T)value;
    }

    /// <inheritdoc/>
    public void Clear() => this.SettingsStorage.Clear();
}
