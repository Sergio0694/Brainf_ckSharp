using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GitHub.HttpHandlers;
using GitHub.Models;

namespace GitHub;

/// <summary>
/// The default <see cref="IGitHubService"/> implementation.
/// </summary>
public sealed class GitHubService : IGitHubService
{
    /// <summary>
    /// Gets the base URL for the service
    /// </summary>
    public const string BaseUrl = "https://api.github.com/";

    /// <summary>
    /// The <see cref="HttpClient"/> instance to use.
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// Creates a new <see cref="GitHubService"/> instance with the specified parameters.
    /// </summary>
    /// <param name="userAgent">The user agent to use.</param>
    public GitHubService(string userAgent)
    {
        this.httpClient = new HttpClient(new UnauthenticatedHttpClientHandler(userAgent)) { BaseAddress = new Uri(BaseUrl) };
    }

    /// <inheritdoc/>
    public async Task<User> GetUserAsync(string username)
    {
        User? user;

        using (Stream stream = await this.httpClient.GetStreamAsync($"/users/{username}"))
        {
            user = await JsonSerializer.DeserializeAsync(stream, GitHubJsonSerializerContext.Default.User);
        }

        return user ?? throw new JsonException("Failed to deserialize a GitHub user.");
    }
}
