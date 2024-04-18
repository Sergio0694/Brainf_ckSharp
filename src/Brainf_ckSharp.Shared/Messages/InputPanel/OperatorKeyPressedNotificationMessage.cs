using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.InputPanel;

/// <summary>
/// A message that notifies whenever a key for a Brainf*ck/PBrain operator is pressed
/// </summary>
/// <param name="value">The input operator</param>
public sealed class OperatorKeyPressedNotificationMessage(char value) : ValueChangedMessage<char>(value);
