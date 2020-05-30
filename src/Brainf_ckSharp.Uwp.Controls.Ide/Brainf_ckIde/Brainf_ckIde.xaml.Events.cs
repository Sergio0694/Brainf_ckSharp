using Windows.Foundation;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
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
    }
}
