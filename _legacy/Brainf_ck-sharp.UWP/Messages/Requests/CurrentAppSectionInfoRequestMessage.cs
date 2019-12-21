using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Messages.Requests.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// A message used to request the current app section being displayed to the user
    /// </summary>
    public sealed class CurrentAppSectionInfoRequestMessage : RequestMessageBase<AppSection> { }
}
