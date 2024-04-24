using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="OverflowMode"/> value for the <see cref="SettingsKeys.OverflowMode"/> setting changes
/// </summary>
/// <param name="value">The new setting value</param>
public sealed class OverflowModeSettingChangedMessage(OverflowMode value) : ValueChangedMessage<OverflowMode>(value);
