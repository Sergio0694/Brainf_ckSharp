using System;
using Windows.UI.Xaml;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A simple class that gets/sets values to and from XAML resources in the app
    /// </summary>
    public class XAMLResourcesHelper
    {
        /// <summary>
        /// Retrieves the value of a given XAML resource
        /// </summary>
        /// <typeparam name="T">The Type of the resource to get</typeparam>
        /// <param name="resourceName">The name of the resource</param>
        public static T GetResourceValue<T>([NotNull] string resourceName)
        {
            return Application.Current.Resources[resourceName].To<T>();
        }

        /// <summary>
        /// Assigns the given value to a XAML resource
        /// </summary>
        /// <typeparam name="T">The Type of the resource</typeparam>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="value">The new value to use</param>
        /// <param name="weaklyTyped">Indicates whether or not a resource can be overridden with a different type</param>
        public static void SetResourceValue<T>([NotNull] string resourceName, T value, bool weaklyTyped = false)
        {
            // Parameter check
            if (resourceName.Length == 0) throw new ArgumentException("The resource name is not valid");

            // Safe cast to be sure the target resource has the Type of the new value
            if (!weaklyTyped && Application.Current.Resources.ContainsKey(resourceName) &&
                Application.Current.Resources[resourceName].GetType() != typeof(T))
            {
                throw new InvalidOperationException("The target resource has a different type");
            }

            // Finally assign the new value to the resource
            Application.Current.Resources[resourceName] = value;
        }
    }
}
