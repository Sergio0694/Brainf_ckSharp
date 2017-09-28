using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.Actions
{
    /// <summary>
    /// A message that signals whenever the IDE autosave function needs to be executed
    /// </summary>
    public sealed class IDEAutosaveTriggeredMessage
    {
        // Private completion source to signal the autosave completion
        private readonly TaskCompletionSource<Unit> CompletionSource = new TaskCompletionSource<Unit>();

        /// <summary>
        /// Gets a <see cref="Task"/> that completes whenever the autosave process is executed
        /// </summary>
        [NotNull]
        public Task Autosave => CompletionSource.Task;

        /// <summary>
        /// Signals that the autosave operation has been completed correctly
        /// </summary>
        public void ReportAutosaveCompleted() => CompletionSource.SetResult(Unit.Instance);
    }
}
