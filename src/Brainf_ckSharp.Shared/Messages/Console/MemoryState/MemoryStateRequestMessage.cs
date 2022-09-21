using Brainf_ckSharp.Memory.Interfaces;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Console.MemoryState;

/// <summary>
/// A request message for the current memory state used in the console
/// </summary>
public sealed class MemoryStateRequestMessage : RequestMessage<IReadOnlyMachineState> { }
