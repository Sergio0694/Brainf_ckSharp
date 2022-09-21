using Brainf_ckSharp.Shared.Constants;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="bool"/> value for the <see cref="SettingsKeys.ShowPBrainButtons"/> setting changes
/// </summary>
public sealed class ShowPBrainButtonsSettingsChangedMessage : ValueChangedMessage<bool>
{
    /// <summary>
    /// Creates a new <see cref="ShowPBrainButtonsSettingsChangedMessage"/> instance with the specified parameters
    /// </summary>
    /// <param name="value">The new setting value</param>
    public ShowPBrainButtonsSettingsChangedMessage(bool value) : base(value)
    {
    }
}
