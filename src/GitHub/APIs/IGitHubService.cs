using System.Threading.Tasks;
using GitHub.Models;
using Refit;

namespace GitHub.APIs;

/// <summary>
/// A basic GitHub service that uses the public APIs
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Gets a single GitHub user
    /// </summary>
    /// <param name="username">The name of the user to retrieve</param>
    [Get("/users/{username}")]
    Task<User> GetUserAsync(string username);
}
