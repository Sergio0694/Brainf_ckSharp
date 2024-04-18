using Brainf_ckSharp.Models;
using Brainf_ckSharp.Shared.Enums;

namespace Brainf_ckSharp.Shared.Models.Ide.Views;

/// <summary>
/// A simple model that associates a specific section to an <see cref="InterpreterResult"/> instance
/// </summary>
public sealed class IdeResultWithSectionInfo
{
    /// <summary>
    /// Gets the current section being targeted
    /// </summary>
    public required IdeResultSection Section { get; init; }

    /// <summary>
    /// Gets the <see cref="InterpreterResult"/> instance currently wrapped
    /// </summary>
    public required InterpreterResult Result { get; init; }
}
