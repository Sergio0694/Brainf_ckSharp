using Brainf_ckSharp.Uwp.Controls.Ide.Models.Abstract;

namespace Brainf_ckSharp.Uwp.Controls.Ide.Models;

/// <summary>
/// A model for a loop indicator to display in the IDE
/// </summary>
internal sealed class BlockIndicator : IndentationIndicatorBase
{
    /// <summary>
    /// Gets or sets the current indentation level for the new block
    /// </summary>
    public int Depth { get; set; }

    /// <summary>
    /// Gets or sets whether or not the loop is nested in a function declaration
    /// </summary>
    public bool IsWithinFunction { get; set; }
}
