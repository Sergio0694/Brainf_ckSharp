using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.UWP.Extensions.System.Collections.Generic;
using UICompositionAnimations.Enums;

namespace Brainf_ckSharp.UWP.Controls.Commands
{
    /// <summary>
    /// A custom <see cref="AnimatedCommandBar"/> that uses animations to switch between different visible buttons
    /// </summary>
    /// <remarks>The items in <see cref="CommandBar.PrimaryCommands"/> need to be <see cref="PriorityCommandBarButton"/> instances</remarks>
    public sealed class AnimatedCommandBar : CommandBar
    {
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
            get => (bool)GetValue(IsPrimaryContentDisplayedProperty);
            set => SetValue(IsPrimaryContentDisplayedProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="IsPrimaryContentDisplayed"/>.
        /// </summary>
        public static readonly DependencyProperty IsPrimaryContentDisplayedProperty = DependencyProperty.Register(
            nameof(IsPrimaryContentDisplayed),
            typeof(bool),
            typeof(AnimatedCommandBar),
            new PropertyMetadata(default(bool), OnIsPrimaryContentDisplayedChanged));

        /// <summary>
        /// The <see cref="AsyncMutex"/> instance used to avoid race conditions when switching buttons
        /// </summary>
        private readonly AsyncMutex ContentSwitchMutex = new AsyncMutex();

        /// <summary>
        /// Updates the UI when <see cref="IsPrimaryContentDisplayed"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static async void OnIsPrimaryContentDisplayedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedCommandBar @this = (AnimatedCommandBar)d;
            bool primary = (bool)e.NewValue;

            using (await @this.ContentSwitchMutex.LockAsync())
            {
                @this.IsHitTestVisible = false;

                // Get the outgoing buttons
                IReadOnlyList<PriorityCommandBarButton> pendingButtons = (
                    from button in @this.PrimaryCommands.Cast<PriorityCommandBarButton>()
                    where button.IsPrimary == !primary
                    select button).ToArray();

                // Fade the visible buttons out
                foreach (var item in pendingButtons.Enumerate())
                {
                    item.Value
                        .Animation()
                        .Delay(ButtonsFadeDelayBetweenAnimations * item.Index)
                        .Offset(Axis.X, 0, -ButtonsAnimationOffset, Easing.CircleEaseInOut)
                        .Opacity(1, 0, Easing.CircleEaseInOut)
                        .Duration(ContentAnimationDuration)
                        .Start();
                }

                // Wait for the initial animations to finish
                await Task.Delay((pendingButtons.Count - 1) * ButtonsFadeDelayBetweenAnimations + ContentAnimationDuration);

                // Set the animated buttons to invisible
                foreach (var item in pendingButtons)
                    item.Visibility = Visibility.Collapsed;

                // Get the target buttons
                IReadOnlyList<PriorityCommandBarButton> targetButtons = (
                    from button in @this.PrimaryCommands.Cast<PriorityCommandBarButton>()
                    where button.IsPrimary == primary
                    select button).ToArray();

                // Fade the target buttons in
                foreach (var item in targetButtons.Enumerate())
                {
                    item.Value.Opacity = 0;
                    item.Value.Visibility = Visibility.Visible;
                    item.Value
                        .Animation()
                        .Delay(ButtonsFadeDelayBetweenAnimations * item.Index)
                        .Offset(Axis.X, -ButtonsAnimationOffset, 0, Easing.CircleEaseInOut)
                        .Opacity(0, 1, Easing.CircleEaseInOut)
                        .Duration(ContentAnimationDuration)
                        .Start();
                }

                // Wait for the second animations to finish
                await Task.Delay((targetButtons.Count - 1) * ButtonsFadeDelayBetweenAnimations + ContentAnimationDuration);

                @this.IsHitTestVisible = true;
            }
        }
    }
}
