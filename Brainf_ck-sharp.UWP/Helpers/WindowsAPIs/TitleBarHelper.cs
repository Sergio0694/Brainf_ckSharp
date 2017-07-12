using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A simple class that manages the UI of the title bar for the app
    /// </summary>
    public static class TitleBarHelper
    {
        /// <summary>
        /// Hides the title bar and expands the app UI
        /// </summary>
        public static void StyleAppTitleBar()
        {
            // Tweak the colors of the title bar
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ForegroundColor = Colors.White;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0x50, 0, 0, 0);
            titleBar.ButtonPressedForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0x10, 0xFF, 0xFF, 0xFF);
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Handle the title bar state
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }
    }
}
