using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Messages.Requests.Abstract;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Messages.Requests
{
    /// <summary>
    /// An class with some extension methods for the <see cref="IMessenger"/> type
    /// </summary>
    public static class MessengerRequestExtensions
    {
        /// <summary>
        /// Requests an operation, for the specified request message
        /// </summary>
        /// <typeparam name="TRequest">The type of request message to send</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
        public static Task RequestAsync<TRequest>([NotNull] this IMessenger messenger)
            where TRequest : RequestMessageBase<Unit>, new()
            => RequestAsync<Unit, TRequest>(messenger);

        /// <summary>
        /// Requests a result of a given type, for the specified request message
        /// </summary>
        /// <typeparam name="TResult">The result type for the specified request</typeparam>
        /// <typeparam name="TRequest">The type of request message to send</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
        public static Task<TResult> RequestAsync<TResult, TRequest>([NotNull] this IMessenger messenger)
            where TRequest : RequestMessageBase<TResult>, new()
        {
            TRequest request = new TRequest();
            messenger.Send(request);
            return request.Task;
        }
    }
}
