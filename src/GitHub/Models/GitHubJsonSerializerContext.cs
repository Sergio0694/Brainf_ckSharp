using System.Text.Json.Serialization;

namespace GitHub.Models;

/// <summary>
/// The JSON context for GitHub models.
/// </summary>
[JsonSerializable(typeof(User), GenerationMode = JsonSourceGenerationMode.Metadata)]
internal sealed partial class GitHubJsonSerializerContext : JsonSerializerContext
{
}
