using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A simple static class with some useful extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Performs a direct cast on the given object to a specific type
        /// </summary>
        /// <typeparam name="T">The tye to return</typeparam>
        /// <param name="o">The object to cast</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>([CanBeNull] this object o) => (T)o;

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it
        /// </summary>
        /// <param name="task">The Task returned by the async call</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this Task task) { }

        /// <summary>
        /// Suppresses the warnings when calling an async method without awaiting it
        /// </summary>
        /// <param name="action">The IAsyncAction returned by the async call</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Forget(this IAsyncAction action) { }

        /// <summary>
        /// Returns the first parent item of a given type for the input object
        /// </summary>
        /// <typeparam name="T">The type of the parent to look for</typeparam>
        /// <param name="target">The source item</param>
        /// <param name="forceLevelUp">If true and the input item is valid, it will be skipped</param>
        public static T FindParent<T>(this DependencyObject target, bool forceLevelUp = false) where T : UIElement
        {
            if (target is T && !forceLevelUp) return target.To<T>();
            DependencyObject parent;
            while ((parent = VisualTreeHelper.GetParent(target)) != null)
            {
                if (parent is T) return parent.To<T>();
                target = parent;
            }
            return null;
        }

        /// <summary>
        /// Tries to find the target resource starting from the given element
        /// </summary>
        /// <typeparam name="T">The type of the resource to retrieve</typeparam>
        /// <param name="element">The starting element</param>
        /// <param name="name">The name of the target resource</param>
        public static T FindResource<T>([NotNull] this FrameworkElement element, [NotNull] String name)
        {
            while (element != null)
            {
                object result;
                if (element.Resources.TryGetValue(name, out result)) return result.To<T>();
                element = element.FindParent<FrameworkElement>(true);
            }
            return (T)Application.Current.Resources[name];
        }
    }
}
