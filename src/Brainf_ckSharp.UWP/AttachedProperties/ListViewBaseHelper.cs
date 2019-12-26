using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Extensions;

#nullable enable

namespace Brainf_ckSharp.UWP.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to control the auto scrolling on a target <see cref="ListViewBase"/> control
    /// </summary>
    public static class ListViewBaseHelper
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
            typeof(ListViewBaseHelper),
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

        /// <summary>
        /// Gets the value of <see cref="IsFluentRearrangingEnabledProperty"/> for a given <see cref="ListViewBase"/>
        /// </summary>
        /// <param name="element">The input <see cref="ListViewBase"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="IsFluentRearrangingEnabledProperty"/> property for the input <see cref="ListViewBase"/> instance</returns>
        public static bool GetIsFluentRearrangingEnabled(ListViewBase element)
        {
            return (bool)element.GetValue(IsFluentRearrangingEnabledProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="IsFluentRearrangingEnabledProperty"/> for a given <see cref="ListViewBase"/>
        /// </summary>
        /// <param name="element">The input <see cref="UIElement"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="bool"/> property</param>
        public static void SetIsFluentRearrangingEnabled(ListViewBase element, bool value)
        {
            element.SetValue(IsFluentRearrangingEnabledProperty, value);
        }

        /// <summary>
        /// An attached property that indicates whether a given element has an active blinking animation
        /// </summary>
        public static readonly DependencyProperty IsFluentRearrangingEnabledProperty = DependencyProperty.RegisterAttached(
            "IsFluentRearrangingEnabled",
            typeof(bool),
            typeof(ListViewBaseHelper),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnIsFluentRearrangingEnabledPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="IsFluentRearrangingEnabledProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnIsFluentRearrangingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListViewBase @this = (ListViewBase)d;
            bool value = (bool)e.NewValue;

            if (value) @this.ContainerContentChanging += FluentGridView_ContainerContentChanging;
            else throw new InvalidOperationException("This attached property doesn't support removal");
        }

        /// <summary>
        /// The offset animation for each item
        /// </summary>
        private static AnimationCollection? _FluentAnimationCollection;

        /// <summary>
        /// Sets the implicit offset animation when resizing the target <see cref="ListViewBase"/> instance
        /// </summary>
        /// <param name="sender">The source <see cref="ListViewBase"/> instance</param>
        /// <param name="args">The args for the current event</param>
        private static void FluentGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Initialize the animation, if needed
            if (_FluentAnimationCollection == null)
            {
                Vector3Animation animation = new Vector3Animation { Target = "Offset", Duration = TimeSpan.FromMilliseconds(500) };
                _FluentAnimationCollection = new AnimationCollection { animation };
            }

            Implicit.SetAnimations(args.ItemContainer, _FluentAnimationCollection);
        }
    }
}
