namespace Brainf_ck_sharp.Legacy.UWP.Messages.Abstract
{
    /// <summary>
    /// A base message that signals whenever a specific value has changed
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
    }
}
