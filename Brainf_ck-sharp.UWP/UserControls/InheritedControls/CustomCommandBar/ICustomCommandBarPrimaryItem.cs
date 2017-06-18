using System;
using Windows.UI.Xaml;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.InheritedControls.CustomCommandBar
{
    /// <summary>
    /// An interface for a custom button/separator/item placed in an animated command bar
    /// </summary>
    public interface ICustomCommandBarPrimaryItem : IDisposable
    {
        /// <summary>
        /// Indicates whether the button should be visible by default or when the AutoHideCommandBar changes display mode
        /// </summary>
        bool DefaultButton { get; }

        /// <summary>
        /// Raised when the value of the ExtraCondition parameter changes
        /// </summary>
        event EventHandler<bool> ExtraConditionStateChanged;

        /// <summary>
        /// An additional condition that is required to be true in order for the control to be visible
        /// </summary>
        bool ExtraCondition { get; }

        /// <summary>
        /// Gets or sets the desired opacity value to use when rendering the current button
        /// </summary>
        double DesiredOpacity { get; }

        /// <summary>
        /// Gets the underlying control that is implementing the interface
        /// </summary>
        [NotNull]
        FrameworkElement Control { get; }
    }
}