using System.Threading.Tasks;

namespace Brainf_ckSharp.Services;

/// <summary>
/// The default <see langword="interface"/> for the a service that listens to the system clipboard and interacts with it
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Tries to copy the input text into the clipboard
    /// </summary>
    /// <param name="text">The text to copy</param>
    /// <param name="flush">Indicates whether or not to let the test remain in clipboard after the app is closed</param>
    bool TryCopy(string text, bool flush = true);

    /// <summary>
    /// Tries to extract text from the clipboard
    /// </summary>
    Task<string?> TryGetTextAsync();
}
