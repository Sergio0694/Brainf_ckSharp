using Brainf_ckSharp.Shared.Constants;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings
{
    /// <summary>
    /// A messsage that signals whenever the <see cref="int"/> value for the <see cref="SettingsKeys.MemorySize"/> setting changes
    /// </summary>
    public sealed class MemorySizeSettingChangedMessage : ValueChangedMessage<int>
    {
        /// <summary>
        /// Creates a new <see cref="MemorySizeSettingChangedMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="value">The new setting value</param>
        public MemorySizeSettingChangedMessage(int value) : base(value)
        {
        }
    }
}
