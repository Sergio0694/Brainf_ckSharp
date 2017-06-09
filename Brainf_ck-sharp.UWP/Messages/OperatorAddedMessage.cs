namespace Brainf_ck_sharp_UWP.Messages
{
    /// <summary>
    /// A simple message that signals when a new operator is being added by the user
    /// </summary>
    public sealed class OperatorAddedMessage
    {
        /// <summary>
        /// Gets the new operator to add
        /// </summary>
        public char Operator { get; }

        /// <summary>
        /// Creates a new instance that wraps the given operator
        /// </summary>
        /// <param name="operator">The new operator to add</param>
        public OperatorAddedMessage(char @operator) => Operator = @operator;
    }
}
