using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A static class that retrieves localized resources at runtime
    /// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// Gets the current ResourceLoader
        /// </summary>
        private static readonly ResourceLoader Loader = ResourceLoader.GetForCurrentView();

        /// <summary>
        /// Returns the string with the given resource key
        /// </summary>
        /// <param name="resource">The key of the resource to retrieve</param>
        [NotNull]
        public static String GetResource([NotNull] String resource)
        {
            try
            {
                return Loader.GetString(resource);
            }
            catch
            {
                // Whops!
#if DEBUG
                Debug.WriteLine($"[RESOURCE MISSING] Key: {resource}");
#endif
                return String.Empty;
            }
        }
    }
}
