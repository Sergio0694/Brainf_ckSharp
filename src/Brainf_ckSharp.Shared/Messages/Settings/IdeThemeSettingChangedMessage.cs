using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings
{
    /// <summary>
    /// A messsage that signals whenever the <see cref="IdeTheme"/> value for the <see cref="SettingsKeys.IdeTheme"/> setting changes
    /// </summary>
    public sealed class IdeThemeSettingChangedMessage : ValueChangedMessage<IdeTheme>
    {
        /// <summary>
        /// Creates a new <see cref="IdeThemeSettingChangedMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The new setting value</param>
        public IdeThemeSettingChangedMessage(IdeTheme value) : base(value)
        {
        }
    }
}
