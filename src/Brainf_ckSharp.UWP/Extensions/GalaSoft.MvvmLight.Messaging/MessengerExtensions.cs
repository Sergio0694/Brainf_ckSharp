using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GalaSoft.MvvmLight.Messaging
{
    /// <summary>
    /// An class with some extension methods for the <see cref="IMessenger"/> type
    /// </summary>
    public static class MessengerExtensions
    {
        /// <summary>
        /// Sends a message of a specified type by creating an instance with the default constructor
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Send<TMessage>(this IMessenger messenger) where TMessage : new()
        {
            messenger.Send(new TMessage());
        }

        /// <summary>
        /// Registers a recipient for given type of messages
        /// </summary>
        /// <typeparam name="TMessage">The type of message to receive</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
        /// <param name="recipient">The recipient that will receive the messages</param>
        /// <param name="handler">The handler to invoke when a message is received</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<TMessage>(this IMessenger messenger, object recipient, Func<Task> handler)
        {
            messenger.Register<TMessage>(recipient, _ => handler());
        }
    }
}
