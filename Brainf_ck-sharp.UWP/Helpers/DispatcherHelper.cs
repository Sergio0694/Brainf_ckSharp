using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.Helpers
{
    /// <summary>
    /// A static class that manages the UI dispatching from background threads
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Gets the current CoreDispatcher instance
        /// </summary>
        private static CoreDispatcher _CoreDispatcher;

        /// <summary>
        /// Checks whether or not the current thread has access to the UI
        /// </summary>
        public static bool HasUIThreadAccess => (_CoreDispatcher ?? (_CoreDispatcher = CoreApplication.MainView.CoreWindow.Dispatcher)).HasThreadAccess;

        /// <summary>
        /// Executes a given action on the UI thread without awaiting the operation
        /// </summary>
        /// <param name="callback">The action to execute on the UI thread</param>
        public static void RunOnUIThread(Action callback)
        {
            if (HasUIThreadAccess) callback();
            else
            {
                _CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    callback();
                }).Forget();
            }
        }

        /// <summary>
        /// Executes a given action on the UI thread and waits for it to be completed
        /// </summary>
        /// <param name="callback">The action to execute on the UI thread</param>
        public static async Task RunOnUIThreadAsync(Action callback)
        {
            if (HasUIThreadAccess) callback();
            else
            {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                _CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    callback();
                    tcs.SetResult(null);
                }).Forget();
                await tcs.Task;
            }
        }

        /// <summary>
        /// Executes a given action on the UI thread and waits for it to be completed
        /// </summary>
        /// <param name="asyncCallback">The action to execute on the UI thread</param>
        public static Task RunOnUIThreadAsync(Func<Task> asyncCallback)
        {
            // Check the current thread
            if (HasUIThreadAccess) return asyncCallback();

            // Schedule on the UI thread if necessary
            TaskCompletionSource<Task> tcs = new TaskCompletionSource<Task>();
            _CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                tcs.SetResult(asyncCallback());
            }).Forget();
            return tcs.Task.Unwrap();
        }
    }
}
