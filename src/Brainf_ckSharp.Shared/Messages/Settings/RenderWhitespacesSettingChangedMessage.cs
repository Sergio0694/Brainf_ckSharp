using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings
{
    /// <summary>
    /// A messsage that signals whenever the <see cref="bool"/> value for the <see cref="SettingsKeys.RenderWhitespaces"/> setting changes
    /// </summary>
    public sealed class RenderWhitespacesSettingChangedMessage : ValueChangedMessage<bool>
    {
        /// <summary>
        /// Creates a new <see cref="RenderWhitespacesSettingChangedMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The new setting value</param>
        public RenderWhitespacesSettingChangedMessage(bool value) : base(value)
        {
        }
    }
}
