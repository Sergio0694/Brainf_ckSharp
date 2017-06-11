using System;
using Windows.UI.Xaml;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A simple class with some static helper methods
    /// </summary>
    public class StaticHelper
    {
        /// <summary>
        /// Retrieves the value of a given XAML resource
        /// </summary>
        /// <typeparam name="T">The Type of the resource to get</typeparam>
        /// <param name="resourceName">The name of the resource</param>
        public static T GetResourceValue<T>([NotNull] String resourceName)
        {
            return Application.Current.Resources[resourceName].To<T>();
        }
    }
}
