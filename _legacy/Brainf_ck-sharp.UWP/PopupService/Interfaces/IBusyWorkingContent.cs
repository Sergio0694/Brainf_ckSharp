using System;

namespace Brainf_ck_sharp_UWP.PopupService.Interfaces
{
    /// <summary>
    /// An interface for a content that can execute some work that requires time and notifies the UI
    /// </summary>
    public interface IBusyWorkingContent
    {
        /// <summary>
        /// Raised whenever the loading status changes for the control
        /// </summary>
        event EventHandler<bool> WorkingStateChanged;
    }
}