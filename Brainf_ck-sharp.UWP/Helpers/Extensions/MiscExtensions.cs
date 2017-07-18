using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// A simple static class with some useful extension methods
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        /// Performs a direct cast on the given object to a specific type
        /// </summary>
        /// <typeparam name="T">The tye to return</typeparam>
        /// <param name="o">The object to cast</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>([CanBeNull] this object o) => (T)o;

        /// <summary>
        /// Applies a function to a given input and returns it (useful to edit items in a method chain)
        /// </summary>
        /// <param name="value">The item to edit and return</param>
        /// <param name="updater">The function to use to edit the input item</param>
        public static T With<T>(this T value, [NotNull] Action<T> updater)
        {
            updater(value);
            return value;
        }

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
    }
}
