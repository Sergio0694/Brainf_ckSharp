using Brainf_ck_sharp_UWP.ViewModels.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// A message used to request a check on whether or not there are unsaved changes in the app
    /// </summary>
    public sealed class IDEUnsavedChangesRequestMessage : RequestMessageBase<bool> { }
}
