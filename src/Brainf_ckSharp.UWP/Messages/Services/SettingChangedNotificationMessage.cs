namespace Brainf_ckSharp.UWP.Messages.Services
{
    /// <summary>
    /// A message that notifies whenever a given setting has changed
    /// </summary>
    /// <typeparam name="T">The setting type</typeparam>
    public sealed class SettingChangedNotificationMessage<T>
    {
        /// <summary>
        /// Gets the key of the setting that has changed
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the updated setting value
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Creates a new <see cref="SettingChangedNotificationMessage{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="key">The key of the setting that has changed</param>
        /// <param name="value">The updated setting value</param>
        public SettingChangedNotificationMessage(string key, T value)
        {
            Key = key;
            Value = value;
        }
    }
}
