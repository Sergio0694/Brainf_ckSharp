using Brainf_ckSharp.Shared.Constants;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="int"/> value for the <see cref="SettingsKeys.MemorySize"/> setting changes
/// </summary>
/// <param name="value">The new setting value</param>
public sealed class MemorySizeSettingChangedMessage(int value) : ValueChangedMessage<int>(value);
