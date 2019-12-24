using System;
using System.Runtime.CompilerServices;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Brainf_ckSharp.UWP.AttachedProperties
{
    /// <summary>
    /// A <see langword="class"/> with an attached XAML property to display a blinking animation on a target <see cref="UIElement"/>
    /// </summary>
    public static class BlinkingAnimationHelper
    { 
        /// <summary>
        /// Gets the value of <see cref="IsBlinkingProperty"/> for a given <see cref="UIElement"/>
        /// </summary>
        /// <param name="element">The input <see cref="UIElement"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="IsBlinkingProperty"/> property for the input <see cref="UIElement"/> instance</returns>
        public static bool GetIsBlinking(UIElement element)
        {
            return (bool)element.GetValue(IsBlinkingProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="IsBlinkingProperty"/> for a given <see cref="UIElement"/>
        /// </summary>
        /// <param name="element">The input <see cref="UIElement"/> for which to set the property value</param>
        /// <param name="value">The valaue to set for the <see cref="IsBlinkingProperty"/> property</param>
        public static void SetIsBlinking(UIElement element, bool value)
        {
            element?.SetValue(IsBlinkingProperty, value);
        }

        /// <summary>
        /// An attached property that indicates whether a given element has an active blinking animation
        /// </summary>
        public static readonly DependencyProperty IsBlinkingProperty = DependencyProperty.RegisterAttached(
            "IsBlinking",
            typeof(bool),
            typeof(BlinkingAnimationHelper),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnIsBlinkingPropertyChanged));

        /// <summary>
        /// A table that maps existing, running <see cref="Storyboard"/> items to target <see cref="UIElement"/>
        /// </summary>
        private static readonly ConditionalWeakTable<UIElement, Storyboard> StoryboardTable = new ConditionalWeakTable<UIElement, Storyboard>();

        /// <summary>
        /// Updates the UI when <see cref="IsBlinkingProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnIsBlinkingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement @this = (UIElement)d;
            bool value = (bool)e.NewValue;

            if (value)
            {
                // Create the new blinking animation
                DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames
                {
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromMilliseconds(250),
                    Duration = TimeSpan.FromMilliseconds(750),
                    KeyFrames =
                    {
                        new DiscreteDoubleKeyFrame
                        {
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(250)),
                            Value = 1
                        },
                        new DiscreteDoubleKeyFrame
                        {
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500)),
                            Value = 0
                        }
                    }
                };
                Storyboard.SetTarget(animation, @this);
                Storyboard.SetTargetProperty(animation, nameof(UIElement.Opacity));

                // Create the storyboard and start it
                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                storyboard.Begin();

                // Track the storyboard in the table
                StoryboardTable.Add(@this, storyboard);
            }
            else
            {
                // Retrieve and remove the current animation
                StoryboardTable.TryGetValue(@this, out Storyboard storyboard);
                StoryboardTable.Remove(@this);

                // Stop the running animation
                storyboard.Stop();
            }
        }
    }
}
