using System.Text.Json.Serialization;

namespace GitHub.Models
{
    /// <summary>
    /// A model for a GitHub user
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Gets the name of the current contributor
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; internal set; }

        /// <summary>
        /// Gets the URL of the contributor profile image
        /// </summary>
        [JsonPropertyName("avatar_url")]
        public string? ProfileImageUrl { get; internal set; }

        /// <summary>
        /// Gets the URL of the contributor profile page
        /// </summary>
        [JsonPropertyName("html_url")]
        public string? ProfilePageUrl { get; internal set; }

        /// <summary>
        /// Gets the user bio
        /// </summary>
        [JsonPropertyName("bio")]
        public string? Bio { get; internal set; }
    }
}
