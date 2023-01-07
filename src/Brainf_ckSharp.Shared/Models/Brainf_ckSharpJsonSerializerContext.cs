using System.Text.Json.Serialization;
using Brainf_ckSharp.Shared.Models.Ide;

namespace Brainf_ckSharp.Shared.Models;

/// <summary>
/// The <see cref="JsonSerializerContext"/> for the application.
/// </summary>
[JsonSerializable(typeof(IdeState))]
[JsonSerializable(typeof(CodeMetadata))]
public sealed partial class Brainf_ckSharpJsonSerializerContext : JsonSerializerContext
{
}
