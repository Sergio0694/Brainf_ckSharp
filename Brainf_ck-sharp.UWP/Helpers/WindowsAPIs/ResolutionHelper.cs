using Windows.UI.ViewManagement;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A simple helper class to get the current size of the application window
    /// </summary>
    public static class ResolutionHelper
    {
        /// <summary>
        /// Gets the current width of the application window
        /// </summary>
        public static double CurrentWidth => ApplicationView.GetForCurrentView().VisibleBounds.Width;

        /// <summary>
        /// Gets the current height of the application window
        /// </summary>
        public static double CurrentHeight => ApplicationView.GetForCurrentView().VisibleBounds.Height;
    }
}
