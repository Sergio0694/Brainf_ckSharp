using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.InputPanel;

/// <summary>
/// A request message for the current stdin buffer
/// </summary>
public sealed class StdinRequestMessage : RequestMessage<string>
{
    /// <summary>
    /// Creates a new <see cref="StdinRequestMessage"/> instance with the specified parameters
    /// </summary>
    /// <param name="isFromBackgroundExecution">Indicates whether or not this request is from a background execution</param>
    public StdinRequestMessage(bool isFromBackgroundExecution)
    {
        IsFromBackgroundExecution = isFromBackgroundExecution;
    }

    /// <summary>
    /// Indicates whether or not this request is from a background execution
    /// </summary>
    public bool IsFromBackgroundExecution { get; }
}
