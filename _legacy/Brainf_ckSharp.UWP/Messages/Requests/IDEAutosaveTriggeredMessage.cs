using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using Brainf_ck_sharp.Legacy.UWP.Messages.Requests.Abstract;

namespace Brainf_ck_sharp.Legacy.UWP.Messages.Requests
{
    /// <summary>
    /// A message that signals whenever the IDE autosave function needs to be executed
    /// </summary>
    public sealed class IDEAutosaveTriggeredMessage : RequestMessageBase<Unit> { }
}
