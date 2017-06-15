using System;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers.Extensions
{
    /// <summary>
    /// A simple class with UI-related extension methods
    /// </summary>
    public static class UIExtensions
    {
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
                if (element.Resources.TryGetValue(name, out object result)) return result.To<T>();
                element = element.FindParent<FrameworkElement>(true);
            }
            return (T)Application.Current.Resources[name];
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

        /// <summary>
        /// Adds an event handler to all the pointer events of the target control
        /// </summary>
        /// <param name="control">The control to monitor</param>
        /// <param name="action"><para>An action to call every time a pointer event is raised. The bool parameter</para>
        /// <para>indicates whether the control is gaining or losing focus</para></param>
        public static void ManageControlPointerStates(this UIElement control, Action<PointerDeviceType, bool> action)
        {
            // Nested functions that adds the actual handlers
            void AddHandler(RoutedEvent @event, bool state, Func<PointerDeviceType, bool> predicate)
            {
                control.AddHandler(@event, new PointerEventHandler((_, e) =>
                {
                    if (predicate == null || predicate(e.Pointer.PointerDeviceType))
                    {
                        action(e.Pointer.PointerDeviceType, state);
                    }
                }), true);
            }

            // Add handlers
            AddHandler(UIElement.PointerExitedEvent, false, null);
            AddHandler(UIElement.PointerCaptureLostEvent, false, null);
            AddHandler(UIElement.PointerCanceledEvent, false, null);
            AddHandler(UIElement.PointerEnteredEvent, true, p => p != PointerDeviceType.Touch);
            AddHandler(UIElement.PointerReleasedEvent, false, p => p == PointerDeviceType.Touch);
        }
    }
}
