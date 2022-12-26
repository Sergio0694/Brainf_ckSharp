using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="OverflowMode"/> value for the <see cref="SettingsKeys.OverflowMode"/> setting changes
/// </summary>
public sealed class OverflowModeSettingChangedMessage : ValueChangedMessage<OverflowMode>
{
    /// <summary>
    /// Creates a new <see cref="OverflowModeSettingChangedMessage"/> instance with the specified parameters
    /// </summary>
    /// <param name="value">The new setting value</param>
    public OverflowModeSettingChangedMessage(OverflowMode value) : base(value)
    {
    }
}
