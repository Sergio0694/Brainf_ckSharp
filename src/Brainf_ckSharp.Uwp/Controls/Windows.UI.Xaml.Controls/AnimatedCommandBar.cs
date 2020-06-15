using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Brainf_ckSharp.Uwp.Extensions.System.Collections.Generic;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Nito.AsyncEx;

namespace Brainf_ckSharp.Uwp.Controls.Windows.UI.Xaml.Controls
{
    /// <summary>
    /// A custom <see cref="AnimatedCommandBar"/> that uses animations to switch between different visible buttons
    /// </summary>
    /// <remarks>The items in <see cref="CommandBar.PrimaryCommands"/> need to use the <see cref="FrameworkElement.Tag"/> with a <see cref="bool"/> value</remarks>
    public sealed class AnimatedCommandBar : CommandBar
    {
        /// <summary>
        /// The <see cref="AsyncLock"/> instance used to avoid race conditions when switching buttons
        /// </summary>
        private readonly AsyncLock ContentSwitchLock = new AsyncLock();

        /// <summary>
        /// The duration of each button animation
        /// </summary>
        private const int ContentAnimationDuration = 150;

        /// <summary>
        /// The time interval between each button animation
        /// </summary>
        private const int ButtonsFadeDelayBetweenAnimations = 25;

        /// <summary>
        /// The horizontal target offset of the buttons animations
        /// </summary>
        private const int ButtonsAnimationOffset = 30;

        /// <summary>
        /// Gets or sets whether or not the primary buttons are currently displayed
        /// </summary>
        public bool IsPrimaryContentDisplayed
        {
            get => (bool)(bool?)GetValue(IsPrimaryContentDisplayedProperty);
            set => SetValue(IsPrimaryContentDisplayedProperty, (bool?)value);
        }

        /// <summary>
        /// The dependency property for <see cref="IsPrimaryContentDisplayed"/>.
        /// </summary>
        public static readonly DependencyProperty IsPrimaryContentDisplayedProperty = DependencyProperty.Register(
            nameof(IsPrimaryContentDisplayed),
            typeof(bool?),
            typeof(AnimatedCommandBar),
            new PropertyMetadata(null, OnIsPrimaryContentDisplayedChanged));

        /// <summary>
        /// Updates the UI when <see cref="IsPrimaryContentDisplayed"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static async void OnIsPrimaryContentDisplayedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedCommandBar @this = (AnimatedCommandBar)d;
            bool primary = (bool)e.NewValue;

            // If this is the initial setup, skip all animations
            if (e.OldValue is null)
            {
                foreach (var item in @this.PrimaryCommands.Cast<FrameworkElement>())
                {
                    item.Visibility = (Visibility)((bool)item.Tag != primary).ToInt();
                }

                return;
            }

            // Creates and starts a storyboard animation for the specified transition
            static void StartButtonAnimation(FrameworkElement button, int delay, int startX, int endX, double startOpacity, double endOpacity)
            {
                DoubleAnimation translationAnimation = new DoubleAnimation
                {
                    From = startX,
                    To = endX,
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut },
                    Duration = new Duration(TimeSpan.FromMilliseconds(ContentAnimationDuration))
                };

                if (!(button.RenderTransform is TranslateTransform))
                    button.RenderTransform = new TranslateTransform();

                Storyboard.SetTarget(translationAnimation, button.RenderTransform);
                Storyboard.SetTargetProperty(translationAnimation, nameof(TranslateTransform.X));

                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    From = startOpacity,
                    To = endOpacity,
                    EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut },
                    Duration = new Duration(TimeSpan.FromMilliseconds(ContentAnimationDuration))
                };

                Storyboard.SetTarget(opacityAnimation, button);
                Storyboard.SetTargetProperty(opacityAnimation, nameof(Opacity));

                Storyboard storyboard = new Storyboard
                {
                    BeginTime = TimeSpan.FromMilliseconds(delay),
                    Children =
                    {
                        translationAnimation,
                        opacityAnimation
                    }
                };

                storyboard.Begin();
            }

            using (await @this.ContentSwitchLock.LockAsync())
            {
                @this.IsHitTestVisible = false;

                // Get the outgoing buttons
                IReadOnlyList<FrameworkElement> pendingElements = (
                    from button in @this.PrimaryCommands.Cast<FrameworkElement>()
                    where button.Tag is bool flag && flag != primary
                    select button).ToArray();

                // Fade the visible buttons out
                foreach (var item in pendingElements.Enumerate())
                {
                    int delay = ButtonsFadeDelayBetweenAnimations * item.Index;

                    StartButtonAnimation(item.Value, delay, 0, -ButtonsAnimationOffset, 1, 0);
                }

                // Wait for the initial animations to finish
                await Task.Delay((pendingElements.Count - 1) * ButtonsFadeDelayBetweenAnimations + ContentAnimationDuration);

                // Set the animated buttons to invisible
                foreach (var item in pendingElements)
                    item.Visibility = Visibility.Collapsed;

                // Get the target buttons
                IReadOnlyList<FrameworkElement> targetElements = (
                    from button in @this.PrimaryCommands.Cast<FrameworkElement>()
                    where button.Tag is bool flag && flag == primary
                    select button).ToArray();

                // Display the target buttons with transparent opacity
                foreach (var item in targetElements)
                {
                    item.Opacity = 0;
                    item.Visibility = Visibility.Visible;
                }

                // Fade the target buttons in
                foreach (var item in targetElements.Reverse().Enumerate())
                {
                    int delay = ButtonsFadeDelayBetweenAnimations * item.Index;

                    StartButtonAnimation(item.Value, delay, -ButtonsAnimationOffset, 0, 0, 1);
                }

                // Wait for the second animations to finish
                await Task.Delay((targetElements.Count - 1) * ButtonsFadeDelayBetweenAnimations + ContentAnimationDuration);

                @this.IsHitTestVisible = true;
            }
        }
    }
}
