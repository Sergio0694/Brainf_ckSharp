using System;

namespace Brainf_ck_sharp_UWP.PopupService.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> to apply to a content to be shown inside a flyout
    /// </summary>
    public interface IEventConfirmedContent
    {
        /// <summary>
        /// Raised when the user interacts with the content in order to confirm the prompt
        /// </summary>
        event EventHandler ContentConfirmed;
    }

    /// <summary>
    /// An <see langword="interface"/> to apply to a content to be shown inside a flyout, which returns a value
    /// </summary>
    public interface IEventConfirmedContent<T>
    {
        /// <summary>
        /// Raised when the user interacts with the content in order to confirm the prompt
        /// </summary>
        event EventHandler<T> ContentConfirmed;

        /// <summary>
        /// Gets the result of the custom Flyout
        /// </summary>
        T Result { get; }
    }
}