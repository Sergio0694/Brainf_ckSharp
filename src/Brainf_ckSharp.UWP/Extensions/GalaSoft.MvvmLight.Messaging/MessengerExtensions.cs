using System.Runtime.CompilerServices;

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
    }
}
