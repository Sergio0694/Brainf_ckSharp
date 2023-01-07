using System.Text.Json.Serialization;

namespace GitHub.Models;

/// <summary>
/// A model for a GitHub user
/// </summary>
/// <param name="Name">Gets the name of the current contributor.</param>
/// <param name="ProfileImageUrl">Gets the URL of the contributor profile image.</param>
/// <param name="ProfilePageUrl">Gets the URL of the contributor profile page.</param>
/// <param name="Bio">Gets the user bio.</param>
public sealed record User(
    [property: JsonPropertyName("name"), JsonRequired] string Name,
    [property: JsonPropertyName("avatar_url"), JsonRequired] string ProfileImageUrl,
    [property: JsonPropertyName("html_url"), JsonRequired] string ProfilePageUrl,
    [property: JsonPropertyName("bio"), JsonRequired] string Bio);
