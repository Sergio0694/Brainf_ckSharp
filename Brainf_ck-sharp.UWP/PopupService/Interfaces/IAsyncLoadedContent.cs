using System;

namespace Brainf_ck_sharp_UWP.PopupService.Interfaces
{
    /// <summary>
    /// An interface for a control that has to load some data before being displayed
    /// </summary>
    public interface IAsyncLoadedContent
    {
        /// <summary>
        /// Raised whenever the control finishes its initial loading process
        /// </summary>
        event EventHandler LoadingCompleted;

        /// <summary>
        /// Gets whether or not the loading event has still to be raised
        /// </summary>
        bool LoadingPending { get; }
    }
}