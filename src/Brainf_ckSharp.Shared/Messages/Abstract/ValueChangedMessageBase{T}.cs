namespace Brainf_ckSharp.Shared.Messages.Abstract
{
    /// <summary>
    /// A base message that signals whenever a specific value has changed
    /// </summary>
    /// <typeparam name="T">The type of value that has changed</typeparam>
    public abstract class ValueChangedMessageBase<T>
    {
        /// <summary>
        /// Gets the value that has changed
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Creates a new instance with the given value
        /// </summary>
        /// <param name="value">The changed value</param>
        protected ValueChangedMessageBase(T value) => Value = value;

        /// <summary>
        /// Unwraps the inner message value directly
        /// </summary>
        /// <param name="message">The value type wrapped by the current instance</param>
        public static implicit operator T(ValueChangedMessageBase<T> message) => message.Value;
    }
}
