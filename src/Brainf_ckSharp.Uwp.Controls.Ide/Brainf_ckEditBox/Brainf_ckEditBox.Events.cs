using Windows.Foundation;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// Raised whenever the <see cref="Text"/> property changes
        /// </summary>
        public new event TypedEventHandler<Brainf_ckEditBox, TextChangedEventArgs>? TextChanged;

        /// <summary>
        /// Rasised when the cursor position changes
        /// </summary>
        public event TypedEventHandler<Brainf_ckEditBox, CursorPositionChangedEventArgs>? CursorPositionChanged;
    }
}
