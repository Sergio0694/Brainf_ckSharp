using System;
using System.Diagnostics.Contracts;

namespace Brainf_ckSharp.Services
{
    /// <summary>
    /// The default <see langword="interface"/> for the settings manager used in the app
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Assigns a value to a settings key
        /// </summary>
        /// <typeparam name="T">The type of the object bound to the key</typeparam>
        /// <param name="key">The key to check</param>
        /// <param name="value">The value to assign to the setting key</param>
        /// <param name="overwrite">Indicates whether or not to overwrite the setting, if it already exists</param>
        void SetValue<T>(string key, T value, bool overwrite = true);

        /// <summary>
        /// Reads a value from the current <see cref="IServiceProvider"/> instance and returns its casting in the right type
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve</typeparam>
        /// <param name="key">The key associated to the requested object</param>
        /// <param name="fallback">If true, the method returns the default <typeparamref name="T"/> value in case of failure</param>
        [Pure]
        T GetValue<T>(string key, bool fallback = false);

        /// <summary>
        /// Deletes all the existing setting values
        /// </summary>
        void Clear();
    }
}
