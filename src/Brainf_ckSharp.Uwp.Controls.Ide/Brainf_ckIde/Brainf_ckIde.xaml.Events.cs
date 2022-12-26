using Windows.Foundation;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide;

public sealed partial class Brainf_ckIde
{
    /// <summary>
    /// Raised whenever the <see cref="Text"/> property changes
    /// </summary>
    public event TypedEventHandler<Brainf_ckIde, TextChangedEventArgs>? TextChanged;

    /// <summary>
    /// Rasised when the cursor position changes
    /// </summary>
    public event TypedEventHandler<Brainf_ckIde, CursorPositionChangedEventArgs>? CursorPositionChanged;

    /// <summary>
    /// Raised whenever a new breakpoint is added
    /// </summary>
    public event TypedEventHandler<Brainf_ckIde, BreakpointToggleEventArgs>? BreakpointAdded;

    /// <summary>
    /// Raised whenever a breakpoint is removed
    /// </summary>
    public event TypedEventHandler<Brainf_ckIde, BreakpointToggleEventArgs>? BreakpointRemoved;

    /// <summary>
    /// Raised whenever all breakpoints are removed
    /// </summary>
    public event TypedEventHandler<Brainf_ckIde, int>? BreakpointsCleared;
}
