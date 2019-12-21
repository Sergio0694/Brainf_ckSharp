using Windows.UI.ViewManagement;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A simple helper class that exposes methods that use the <see cref="ApplicationView"/> APIs
    /// </summary>
    public static class ApplicationViewHelper
    {
        /// <summary>
        /// Gets the current width of the application window
        /// </summary>
        public static double CurrentWidth => ApplicationView.GetForCurrentView().VisibleBounds.Width;

        /// <summary>
        /// Gets the current height of the application window
        /// </summary>
        public static double CurrentHeight => ApplicationView.GetForCurrentView().VisibleBounds.Height;

        /// <summary>
        /// Gets whether or not the app window is either in full screen, or maximized and in tablet mode
        /// </summary>
        public static bool IsFullScreenOrTabletMode
        {
            get
            {
                ApplicationView view = ApplicationView.GetForCurrentView();
                return view.IsFullScreenMode || view.AdjacentToLeftDisplayEdge && view.AdjacentToRightDisplayEdge &&
                       UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch;
            }
        }
    }
}
