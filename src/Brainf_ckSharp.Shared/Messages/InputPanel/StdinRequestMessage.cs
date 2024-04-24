using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.InputPanel;

/// <summary>
/// A request message for the current stdin buffer
/// </summary>
/// <param name="isFromBackgroundExecution">Indicates whether or not this request is from a background execution</param>
public sealed class StdinRequestMessage(bool isFromBackgroundExecution) : RequestMessage<string>
{
    /// <summary>
    /// Indicates whether or not this request is from a background execution
    /// </summary>
    public bool IsFromBackgroundExecution { get; } = isFromBackgroundExecution;
}
