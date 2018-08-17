using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// A base class for request messages
    /// </summary>
    /// <typeparam name="T">The type of result for the current rrequest</typeparam>
    public abstract class RequestMessageBase<T>
    {
        // Private completion source to signal the autosave completion
        private readonly TaskCompletionSource<T> CompletionSource = new TaskCompletionSource<T>();

        /// <summary>
        /// Gets a <see cref="Task{T}"/> that indicates the result of the current request message
        /// </summary>
        [NotNull]
        public Task<T> Task => CompletionSource.Task;

        /// <summary>
        /// Reports a result for the current request message
        /// </summary>
        public void ReportResult(T result) => CompletionSource.SetResult(result);
    }
}
