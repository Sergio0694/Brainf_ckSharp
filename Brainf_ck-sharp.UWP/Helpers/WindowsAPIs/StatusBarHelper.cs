using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Brainf_ck_sharp_UWP.Helpers.WindowsAPIs
{
    /// <summary>
    /// A static class that manages the status bar on mobile devices
    /// </summary>
    public static class StatusBarHelper
    {
        // Gets the full namespace for the class
        private const String StatusBarString = "Windows.UI.ViewManagement.StatusBar";

        // Returns the current status bar, if available
        private static StatusBar GetCurrentStatusBarAsync()
        {
            return ApiInformation.IsTypePresent(StatusBarString) ? StatusBar.GetForCurrentView() : null;
        }

        /// <summary>
        /// Tries to display the status bar
        /// </summary>
        /// <returns>The occluded height if the operation succedes</returns>
        public static async Task<double> TryShowAsync()
        {
            StatusBar statusBar = GetCurrentStatusBarAsync();
            if (statusBar == null) return 0;
            statusBar.BackgroundColor = null;
            statusBar.ForegroundColor = Colors.White;
            await statusBar.ShowAsync();
            return statusBar.OccludedRect.Height;
        }

        /// <summary>
        /// Tries to hide the status bar, if present
        /// </summary>
        public static IAsyncAction HideAsync() => GetCurrentStatusBarAsync()?.HideAsync();
    }
}
