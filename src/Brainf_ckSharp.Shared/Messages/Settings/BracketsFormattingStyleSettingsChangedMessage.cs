using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums.Settings;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="BracketsFormattingStyle"/> value for the <see cref="SettingsKeys.BracketsFormattingStyle"/> setting changes
/// </summary>
public sealed class BracketsFormattingStyleSettingsChangedMessage : ValueChangedMessage<BracketsFormattingStyle>
{
    /// <summary>
    /// Creates a new <see cref="BracketsFormattingStyleSettingsChangedMessage"/> instance with the specified parameters
    /// </summary>
    /// <param name="value">The new setting value</param>
    public BracketsFormattingStyleSettingsChangedMessage(BracketsFormattingStyle value) : base(value)
    {
    }
}
