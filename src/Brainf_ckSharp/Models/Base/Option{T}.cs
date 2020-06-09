namespace Brainf_ckSharp.Models.Base
{
    /// <summary>
    /// A <see langword="class"/> that represents an optional result of type <typeparamref name="T"/> for a given Brainf*ck/PBrain script
    /// </summary>
    /// <typeparam name="T">The type of execution result for the current script</typeparam>
    public sealed class Option<T> where T : class
    {
        /// <summary>
        /// Gets the <see cref="SyntaxValidationResult"/> instance that indicates the parsing result for the input script
        /// </summary>
        public SyntaxValidationResult ValidationResult { get; }

        /// <summary>
        /// Gets the <typeparamref name="T"/> result for the current execution, if available
        /// </summary>
        /// <remarks>This property will only be not <see langword="null"/> if the parsing is successful</remarks>
        public T? Value { get; }

        /// <summary>
        /// Creates a new <see cref="Option{T}"/> instance with the specified parameters
        /// </summary>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance that indicates the parsing result for the input script</param>
        /// <param name="value">The <typeparamref name="T"/> result for the current execution, if available</param>
        private Option(SyntaxValidationResult validationResult, T? value)
        {
            ValidationResult = validationResult;
            Value = value;
        }

        /// <summary>
        /// Creates a new <see cref="Option{T}"/> instance for an invalid script
        /// </summary>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance that indicates the parsing result for the input script</param>
        /// <returns>An <see cref="Option{T}"/> instance for a script not parsed successfully</returns>
        internal static Option<T> From(SyntaxValidationResult validationResult)
        {
            return new Option<T>(validationResult, null);
        }

        /// <summary>
        /// Creates a new <see cref="Option{T}"/> instance for a script parsed successfully
        /// </summary>
        /// <param name="validationResult">The <see cref="SyntaxValidationResult"/> instance that indicates the parsing result for the input script</param>
        /// <param name="value">The <typeparamref name="T"/> result for the current execution</param>
        /// <returns>An <see cref="Option{T}"/> instance for a script parsed successfully</returns>
        internal static Option<T> From(SyntaxValidationResult validationResult, T value)
        {
            return new Option<T>(validationResult, value);
        }
    }
}
