using System;
using Windows.Foundation;
using Windows.System;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A small class with some wrapper methods that use the <see cref="Launcher"/> class
    /// </summary>
    public static class LauncherHelper
    {
        // The current ProductId for Brainf*ck#
        private const String ProductId = "9nblgggzhvq5";

        /// <summary>
        /// Opens the Store review page for the app
        /// </summary>
        public static IAsyncOperation<bool> OpenStoreAppReviewPageAsync()
        {
            return Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={ProductId}"));
        }
    }
}
