using Brainf_ck_sharp_UWP.Messages.Requests.Abstract;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// A request message to get the current contents of the stdin buffer
    /// </summary>
    public sealed class StdinBufferRequestMessage : RequestMessageBase<string> { }
}
