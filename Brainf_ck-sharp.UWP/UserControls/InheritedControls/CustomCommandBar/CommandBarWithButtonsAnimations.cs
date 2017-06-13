using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Brainf_ck_sharp_UWP.Helpers;
using JetBrains.Annotations;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls.CustomCommandBar
{
    /// <summary>
    /// A custom CommandBar that uses an animation to show/hide groups of primary buttons inside it
    /// </summary>
    public class CommandBarWithButtonsAnimations : CommandBar, INotifyPropertyChanged, IDisposable
    {
        #region Costants

        /// <summary>
        /// Gets the duration of each button animation
        /// </summary>
        private const int ContentAnimationDuration = 150;

        /// <summary>
        /// Gets the time interval between each button animation
        /// </summary>
        private const int ButtonsFadeDelayBetweenAnimations = 25;

        /// <summary>
        /// Gets the horizontal target offset of the buttons animations
        /// </summary>
        private const int ButtonsAnimationOffset = 30;

        #endregion

        private bool _Disposed;

        public virtual void Dispose()
        {
            // Remove the handlers
            if (_Disposed) return;
            _Disposed = true;
            try
            {
                PrimaryCommands.TypedForEach<CustomCommandBarButton>(button =>
                {
                    button.ExtraConditionStateChanged -= Button_ExtraConditionStateChanged;
                });
            }
            catch
            {
                // Can never be too sure with event handlers..
            }

            // Clear the buttons
            PrimaryCommands.Clear();
        }

        public CommandBarWithButtonsAnimations()
        {
            // Set additional options
            this.IsDynamicOverflowEnabled = false;

            // Hide the secondary buttons and the buttons that require additional conditions
            this.Loaded += (s, e) =>
            {
                PrimaryCommands.TypedForEach<CustomCommandBarButton>(button =>
                {
                    button.ExtraConditionStateChanged += Button_ExtraConditionStateChanged;
                    if (!button.DefaultButton || !button.ExtraCondition)
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                });
            };
        }

        private void Button_ExtraConditionStateChanged(object sender, bool e)
        {
            CustomCommandBarButton button = sender.To<CustomCommandBarButton>();
            button.Visibility = e ? (button.DefaultButton == PrimaryContentEnabled).ToVisibility() : Visibility.Collapsed;
        }

        private bool _PrimaryContentEnabled = true;

        /// <summary>
        /// Gets or sets which group of buttons should be visible inside the control
        /// </summary>
        public bool PrimaryContentEnabled
        {
            get { return _PrimaryContentEnabled; }
            private set
            {
                if (_PrimaryContentEnabled != value)
                {
                    _PrimaryContentEnabled = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Semaphore used to avoid race conditions when setting the visibility of the buttons
        /// </summary>
        private readonly SemaphoreSlim ButtonsSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Gets whether or not the control is actually performing an animation on its content
        /// </summary>
        public bool TransitionPending => ButtonsSemaphore.CurrentCount == 0;

        /// <summary>
        /// Shows or hides a group of buttons in the CommandBar depending on the given value
        /// </summary>
        /// <param name="primaryContentEnabled">Indicates which group (default buttons or secondary buttons) to show</param>
        public async void SwitchContent(bool primaryContentEnabled)
        {
            // Lock the semaphore
            if (_Disposed) return;
            await ButtonsSemaphore.WaitAsync();

            // Check if the animation is needed
            if (PrimaryContentEnabled == primaryContentEnabled)
            {
                ButtonsSemaphore.Release();
                return;
            }

            this.IsHitTestVisible = false;
            Storyboard fadeOut = null;

            // Get the buttons to hide
            CustomCommandBarButton[] pendingButtons =
                (from control in PrimaryCommands
                let button = Extensions.To<CustomCommandBarButton>(control)
                where button.Visibility == Visibility.Visible
                select button).ToArray();
            if (pendingButtons.Length == 0 || _Disposed)
            {
                ButtonsSemaphore.Release();
                return;
            }

            // Fade out the buttons that are actually visible and get the last Storyboard
            for (int i = 0; i < pendingButtons.Length; i++)
            {
                CustomCommandBarButton pendingButton = pendingButtons[i];
                if (i != pendingButtons.Length - 1)
                {
                    pendingButton.StartXAMLTransformFadeSlideAnimation(null, 0, TranslationAxis.X, 0, -ButtonsAnimationOffset, ContentAnimationDuration, null, null, EasingFunctionNames.CircleEaseOut);
                    await Task.Delay(ButtonsFadeDelayBetweenAnimations);
                }
                else fadeOut = pendingButton.GetXAMLTransformFadeSlideStoryboard(1, 0, TranslationAxis.X, 0, -ButtonsAnimationOffset, ContentAnimationDuration, EasingFunctionNames.CircleEaseOut);
            }

            // Resume the rest of the method when the last Storyboard completes
            Debug.Assert(fadeOut != null, "The target Storyboard shouldn't be null here");
            fadeOut.Completed += async (s, e) =>
            {
                // Collapse the pending buttons
                foreach (CustomCommandBarButton button in pendingButtons) button.Visibility = Visibility.Collapsed;

                // Get the list of buttons to display
                CustomCommandBarButton[] upcomingButtons =
                    (from control in PrimaryCommands
                    let button = control.To<CustomCommandBarButton>()
                    where button.DefaultButton == primaryContentEnabled && button.ExtraCondition
                    select button).ToArray();

                if (upcomingButtons.Length == 0 || _Disposed)
                {
                    ButtonsSemaphore.Release();
                    return;
                }

                // Fade in the pending buttons and get the last Storyboard
                Storyboard fadeIn = null;
                for (int i = upcomingButtons.Length - 1; i >= 0; i--)
                {
                    CustomCommandBarButton pendingButton = upcomingButtons[i];
                    pendingButton.Opacity = 0;
                    pendingButton.Visibility = Visibility.Visible;
                    if (i != upcomingButtons.Length - 1)
                    {
                        pendingButton.StartXAMLTransformFadeSlideAnimation(0, pendingButton.DesiredOpacity, TranslationAxis.X, -ButtonsAnimationOffset, 0, ContentAnimationDuration, null, null, EasingFunctionNames.CircleEaseOut);
                        await Task.Delay(ButtonsFadeDelayBetweenAnimations);
                    }
                    else fadeIn = pendingButton.GetXAMLTransformFadeSlideStoryboard(0, 1, TranslationAxis.X, -ButtonsAnimationOffset, 0, ContentAnimationDuration, EasingFunctionNames.CircleEaseOut);
                }

                // Start the target Storyboard and finally store the new parameter value and release the semaphore when the animation completes
                Debug.Assert(fadeIn != null, "The target Storyboard really shouldn't be null here (we were almost there...)");
                fadeIn.Completed += (sender, eventArgs) =>
                {
                    PrimaryContentEnabled = primaryContentEnabled;
                    ButtonsSemaphore.Release();
                    this.IsHitTestVisible = true;
                };
                fadeIn.Begin();
            };
            fadeOut.Begin();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
