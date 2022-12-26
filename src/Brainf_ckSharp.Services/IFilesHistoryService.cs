using System.Threading.Tasks;

namespace Brainf_ckSharp.Services;

/// <summary>
/// The default <see langword="interface"/> for a service that handles user history when working with files
/// </summary>
public interface IFilesHistoryService
{
    /// <summary>
    /// Logs a new activity, or updates the existing one, for a specified file
    /// </summary>
    /// <param name="file">The <see cref="IFile"/> instance the user is currently working on</param>
    Task LogOrUpdateActivityAsync(IFile file);

    /// <summary>
    /// Dismisses the currently activity, if present
    /// </summary>
    public Task DismissCurrentActivityAsync();

    /// <summary>
    /// Removes an existing activity, if existing
    /// </summary>
    /// <param name="file">The <see cref="IFile"/> instance to remove</param>
    public Task RemoveActivityAsync(IFile file);
}
