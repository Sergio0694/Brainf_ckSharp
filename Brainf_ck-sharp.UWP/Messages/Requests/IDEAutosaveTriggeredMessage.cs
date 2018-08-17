using Brainf_ck_sharp_UWP.DataModels.Misc;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// A message that signals whenever the IDE autosave function needs to be executed
    /// </summary>
    public sealed class IDEAutosaveTriggeredMessage : RequestMessageBase<Unit> { }
}
