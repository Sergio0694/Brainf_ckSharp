using Brainf_ckSharp.Shared.Models.Ide;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Brainf_ckSharp.Shared.Messages.Ide;

/// <summary>
/// A message that signals whenever the user requests to open a file
/// </summary>
/// <param name="sourceCode">The <see cref="SourceCode"/> instance to load</param>
public sealed class LoadSourceCodeRequestMessage(SourceCode sourceCode) : ValueChangedMessage<SourceCode>(sourceCode);
