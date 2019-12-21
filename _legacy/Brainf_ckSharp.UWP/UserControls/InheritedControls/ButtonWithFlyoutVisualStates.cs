using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.InheritedControls
{
    /// <summary>
    /// A custom <see cref="Button"/> that keeps being highlighted whenever a linked flyout is open
    /// </summary>
    public class ButtonWithFlyoutVisualStates : Button
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("RootGrid") is Grid layoutRoot)
            {
                ButtonWithFlyoutStatesVisualManager manager = new ButtonWithFlyoutStatesVisualManager(() => ExternalFlyoutOpen);
                VisualStateManager.SetCustomVisualStateManager(layoutRoot, manager);
            }
        }

        private bool _ExternalFlyoutOpen;

        /// <summary>
        /// Gets or sets a value that indicates when this control caused a flyout to open, and should therefore stay highlighted until the flyout closes
        /// </summary>
        public bool ExternalFlyoutOpen
        {
            get => _ExternalFlyoutOpen;
            set
            {
                if (_ExternalFlyoutOpen == value) return;
                _ExternalFlyoutOpen = value;
                if (!value) VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        // Custom state manager to control the highlight state
        private class ButtonWithFlyoutStatesVisualManager : VisualStateManager
        {
            /// <summary>
            /// Gets or sets the optional test function for the animation direction
            /// </summary>
            /// <remarks>If the function is null, the current user setting will be used to decide the animation direction</remarks>
            [NotNull]
            private Func<bool> FlyoutOpenCheck { get; set; }

            public ButtonWithFlyoutStatesVisualManager([NotNull] Func<bool> check) => FlyoutOpenCheck = check;

            protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
            {
                if ((string.IsNullOrWhiteSpace(stateName) || stateName.Equals("Normal")) && FlyoutOpenCheck()) stateName = "PointerOver";
                return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
            }
        }
    }
}
