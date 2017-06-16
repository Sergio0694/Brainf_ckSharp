using System;

namespace Brainf_ck_sharp_UWP.FlyoutService.Interfaces
{
    public interface IValidableDialog
    {
        event EventHandler<bool> ValidStateChanged;
    }
}