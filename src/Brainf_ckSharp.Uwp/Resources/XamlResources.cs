using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

#nullable enable

namespace Brainf_ckSharp.Uwp.Resources
{
    /// <summary>
    /// A small <see langword="class"/> that exposes some commonly used XAML resources
    /// </summary>
    public static class XamlResources
    {
        /// <summary>
        /// Gets a resource with a specified key
        /// </summary>
        /// <typeparam name="T">The type of resource to retrieve</typeparam>
        /// <param name="key">The key of the resource to retrieve</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(string key) => (T)Application.Current.Resources[key];

        /// <summary>
        /// Safely tries to retrieve a resource with a specified key
        /// </summary>
        /// <typeparam name="T">The type of resource to retrieve</typeparam>
        /// <param name="key">The key of the resource to retrieve</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T TryGet<T>(string key) => Application.Current.Resources.TryGetValue(key, out object value) ? (T)value : default;

        /// <summary>
        /// Assigns or creates a resource value with a specified key
        /// </summary>
        /// <typeparam name="T">The type of resource to set</typeparam>
        /// <param name="key">The key of the resource to create or update</param>
        /// <param name="value">The resource value to set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(string key, T value) => Application.Current.Resources[key] = value;

        /// <summary>
        /// Gets the current system accent color
        /// </summary>
        public static Color AccentColor
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Get<Color>("SystemAccentColor");
        }

        /// <summary>
        /// Gets the current system accent color brush
        /// </summary>
        public static Brush AccentBrush
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Get<SolidColorBrush>("SystemControlHighlightAccentBrush");
        }

        /// <summary>
        /// A <see langword="class"/> with some hardcoded icons from the Segoe MDL2 Assets charset
        /// </summary>
        public static class Icons
        {
            public static readonly string AddToFavorites = Get();
            public static readonly string RemoveFromFavorites = Get();

            /// <summary>
            /// A helper function that returns the appropriate icon from the XAML resource dictionary
            /// </summary>
            /// <param name="name">The name of the icon to retrieve</param>
            [Pure]
            private static string Get([CallerMemberName] string? name = null)
            {
                Guard.MustBeNotNull(name, nameof(name));

                return Get<string>($"{name}Icon");
            }
        }
    }
}
