using Brainf_ckSharp.Enums;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Settings;

/// <summary>
/// A messsage that signals whenever the <see cref="ExecutionOptions"/> value for the various execution option settings changes.
/// </summary>
/// <param name="value">The new setting value</param>
public sealed class ExecutionOptionsSettingChangedMessage(ExecutionOptions value) : ValueChangedMessage<ExecutionOptions>(value);
