﻿using System;
using System.Collections;
using System.Collections.Generic;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value) => value >= 0 ? value : -value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double value) => value >= 0 ? value : -value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsWithDelta(this double value, double comparison, double delta = 0.1) => (value - comparison).Abs() < delta;

        /// <summary>
        /// Performs a loop on the given collection, calling the input delegate for each item
        /// </summary>
        /// <typeparam name="T">The Type to cast the collection items to</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="action">The delegate to call for each item in the collection</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TypedForEach<T>([NotNull] this IEnumerable source, [NotNull] Action<T> action)
        {
            foreach (object item in source) action(item.To<T>());
        }

        /// <summary>
        /// Converts a boolean value to its Visibility equivalent
        /// </summary>
        /// <param name="value">The value to convert</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Visibility ToVisibility(this bool value) => value ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Returns the first element of a specific type in the visual tree of a DependencyObject
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="parent">The object that contains the UIElement to find</param>
        public static T FindChild<T>(this DependencyObject parent) where T : class
        {
            if (parent is T) return parent.To<T>();
            int children = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T))
                {
                    T tChild = FindChild<T>(child);
                    if (tChild != null) return tChild;
                }
                else return child as T;
            }
            return null;
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable</summary>
        ///<param name="items">The enumerable to search</param>
        ///<param name="predicate">The expression to test the items against</param>
        ///<returns>The index of the first matching item, or -1 if no items match</returns>
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in items)
            {
                if (predicate(item)) return index;
                index++;
            }
            return -1;
        }
    }
}
