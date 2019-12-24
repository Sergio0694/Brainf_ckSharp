using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace Brainf_ckSharp.UWP.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to control the auto scrolling on a target <see cref="ListViewBase"/> control
    /// </summary>
    public static class AutoScrollingListViewHelper
    {
        /// <summary>
        /// Gets the value of <see cref="IsAutoScrollEnabledProperty"/> for a given <see cref="ListViewBase"/>
        /// </summary>
        /// <param name="element">The input <see cref="ListViewBase"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="IsAutoScrollEnabledProperty"/> property for the input <see cref="ListViewBase"/> instance</returns>
        public static INotifyCollectionChanged GetIsAutoScrollEnabled(ListViewBase element)
        {
            return (INotifyCollectionChanged)element.GetValue(IsAutoScrollEnabledProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="IsAutoScrollEnabledProperty"/> for a given <see cref="ListViewBase"/>
        /// </summary>
        /// <param name="element">The input <see cref="UIElement"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="INotifyCollectionChanged"/> property</param>
        public static void SetIsAutoScrollEnabled(ListViewBase element, INotifyCollectionChanged value)
        {
            element.SetValue(IsAutoScrollEnabledProperty, value);
        }

        /// <summary>
        /// An attached property that indicates whether a given element has an active blinking animation
        /// </summary>
        public static readonly DependencyProperty IsAutoScrollEnabledProperty = DependencyProperty.RegisterAttached(
            "IsAutoScrollEnabled",
            typeof(INotifyCollectionChanged),
            typeof(AutoScrollingListViewHelper),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnIsAutoScrollEnabledPropertyChanged));

        /// <summary>
        /// A table that maps existing <see cref="INotifyCollectionChanged"/> items to target <see cref="ListViewBase"/>
        /// </summary>
        private static readonly ConditionalWeakTable<INotifyCollectionChanged, ListViewBase> ControlsMap = new ConditionalWeakTable<INotifyCollectionChanged, ListViewBase>();

        /// <summary>
        /// A table that maps existing <see cref="ListViewBase"/> controls to target <see cref="INotifyCollectionChanged"/>
        /// </summary>
        private static readonly ConditionalWeakTable<ListViewBase, INotifyCollectionChanged> CollectionsMap = new ConditionalWeakTable<ListViewBase, INotifyCollectionChanged>();

        /// <summary>
        /// Updates the UI when <see cref="IsAutoScrollEnabledProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnIsAutoScrollEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListViewBase @this = (ListViewBase)d;
            INotifyCollectionChanged value = (INotifyCollectionChanged)e.NewValue;

            // Remove the old handler, if necessary
            if (CollectionsMap.TryGetValue(@this, out INotifyCollectionChanged old))
            {
                ControlsMap.Remove(old);
                old.CollectionChanged -= INotifyCollectionChanged_CollectionChanged;
            }

            // Register the new collection
            CollectionsMap.AddOrUpdate(@this, value);
            ControlsMap.Add(value, @this);
            value.CollectionChanged += INotifyCollectionChanged_CollectionChanged;
        }

        /// <summary>
        /// Executes the auto scroll animation on a given <see cref="ListViewBase"/> control, when needed
        /// </summary>
        /// <param name="sender">The source <see cref="INotifyCollectionChanged"/> instance</param>
        /// <param name="e">The events info for the current invocation</param>
        private static async void INotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;

            if (!ControlsMap.TryGetValue((INotifyCollectionChanged)sender, out ListViewBase control)) throw new InvalidOperationException("Can't find target control");

            // Wait for the new item to be displayed, then scroll down
            await Task.Delay(250);
            ScrollViewer scroller = control.FindDescendant<ScrollViewer>();
            scroller?.ChangeView(null, scroller.ScrollableHeight, null, false);
        }
    }
}

