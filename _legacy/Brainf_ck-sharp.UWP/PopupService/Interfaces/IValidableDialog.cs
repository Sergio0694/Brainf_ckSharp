using System;

namespace Brainf_ck_sharp.Legacy.UWP.PopupService.Interfaces
{
    /// <summary>
    /// An interface for a control that can change its status to signal when it can be confirmed
    /// </summary>
    public interface IValidableDialog
    {
        /// <summary>
        /// Raised whenever the current validated status for the control changes
        /// </summary>
        event EventHandler<bool> ValidStateChanged;
    }
}