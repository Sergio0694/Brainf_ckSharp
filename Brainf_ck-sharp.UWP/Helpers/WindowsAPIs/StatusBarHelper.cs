using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
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

        /// <summary>
        /// Gets the current status bar, if available
        /// </summary>
        private static StatusBar GetCurrentStatusBar() => ApiInformation.IsTypePresent(StatusBarString) ? StatusBar.GetForCurrentView() : null;

        /// <summary>
        /// Tries to hide the status bar, if present
        /// </summary>
        public static Task HideAsync() => GetCurrentStatusBar()?.HideAsync().AsTask() ?? Task.CompletedTask;

        /// <summary>
        /// Gets the occluded height of the status bar, if displayed
        /// </summary>
        public static double OccludedHeight => GetCurrentStatusBar()?.OccludedRect.Height ?? 0;
    }
}