using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.UWP.Controls.Commands
{
    /// <summary>
    /// A custom <see cref="PriorityCommandBarButton"/> that indicates whether or not it is a primary button
    /// </summary>
    public sealed class PriorityCommandBarButton : AppBarButton
    {
        /// <summary>
        /// Gets or sets whether or not the current <see cref="PriorityCommandBarButton"/> is a primary button
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
