using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.UWP.Messages.Abstract;

namespace GalaSoft.MvvmLight.Messaging
{
    /// <summary>
    /// An class with some extension methods for the <see cref="IMessenger"/> type
    /// </summary>
    public static class MessengerExtensions
    {
        /// <summary>
        /// Requests a result of a given type, using a specified message
        /// </summary>
        /// <typeparam name="TMessage">The type of message to send</typeparam>
        /// <typeparam name="TResult">The returned type</typeparam>
        /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
        [Pure]
        public static TResult Request<TMessage, TResult>(this IMessenger messenger) where TMessage : RequestMessageBase<TResult>, new()
        {
            TMessage message = new TMessage();
            messenger.Send(message);

            if (!message.ResponseReceived) throw new InvalidOperationException("No response was received for the message");

            return message.Result;
        }

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
