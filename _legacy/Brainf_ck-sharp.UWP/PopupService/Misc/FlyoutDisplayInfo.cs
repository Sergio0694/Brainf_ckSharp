using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService.UI;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.PopupService.Misc
{
    /// <summary>
    /// Contains all the necessary info on a <see cref="Popup"/> control to display and its content
    /// </summary>
    public sealed class FlyoutDisplayInfo
    {
        /// <summary>
        /// Gets the current <see cref="Popup"/> control
        /// </summary>
        [NotNull]
        public Popup Popup { get; }

        /// <summary>
        /// Gets the content presenter inside the popup
        /// </summary>
        [NotNull]
        public FlyoutContainer Container => Popup.Child.To<FlyoutContainer>();

        /// <summary>
        /// Gets the current display mode requested for the popup
        /// </summary>
        public FlyoutDisplayMode DisplayMode { get; }

        /// <summary>
        /// Creates a new instance for a popup to display
        /// </summary>
        /// <param name="popup">The popup to show to the user</param>
        /// <param name="mode">The desired display mode</param>
        public FlyoutDisplayInfo([NotNull] Popup popup, FlyoutDisplayMode mode)
        {
            Popup = popup;
            DisplayMode = mode;
        }
    }
}
