using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="bool"/> value for the <see cref="SettingsKeys.ShowPBrainButtons"/> setting changes
/// </summary>
/// <param name="value">The new setting value</param>
public sealed class ShowPBrainButtonsSettingsChangedMessage(bool value) : ValueChangedMessage<bool>(value);
