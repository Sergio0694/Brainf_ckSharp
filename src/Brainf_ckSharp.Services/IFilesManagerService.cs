namespace Brainf_ckSharp.Services;

/// <summary>
/// The default <see langword="interface"/> for a service that handles file usage for a multi-instance application
/// </summary>
public interface IFilesManagerService
{
    /// <summary>
    /// Registers a given file to report it's currently in use
    /// </summary>
    /// <param name="file">The file to register, if present</param>
    void RegisterFile(IFile? file);

    /// <summary>
    /// Checks wheher or not a given file is currently in use
    /// </summary>
    /// <param name="file">The file to check for usage</param>
    /// <returns>Whether or not <paramref name="file"/> is currently in use</returns>
    bool IsRegistered(IFile file);

    /// <summary>
    /// Tries to switch to a given file, if possible
    /// </summary>
    /// <param name="file">The file to try to switch to</param>
    /// <returns>Whether or not <paramref name="file"/> was switched to successfully</returns>
    bool TrySwitchTo(IFile file);
}
