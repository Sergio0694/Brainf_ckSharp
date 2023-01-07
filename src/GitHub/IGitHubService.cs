using System.Threading.Tasks;
using GitHub.Models;

namespace GitHub;

/// <summary>
/// A basic GitHub service that uses the public APIs.
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Gets a single GitHub user.
    /// </summary>
    /// <param name="username">The name of the user to retrieve.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="username"/> is <see langword="null"/>.</exception>
    Task<User> GetUserAsync(string username);
}
