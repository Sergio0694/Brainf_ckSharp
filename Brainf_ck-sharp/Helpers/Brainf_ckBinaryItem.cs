namespace Brainf_ck_sharp.Helpers
{
    /// <summary>
    /// Wraps the operators in the source code of a given script
    /// </summary>
    internal struct Brainf_ckBinaryItem
    {
        /// <summary>
        /// Gets the offset of the current operator from the start of the script
        /// </summary>
        public uint Offset { get; }

        /// <summary>
        /// Gets the wrapped operator in the current instance
        /// </summary>
        public char Operator { get; }

        /// <summary>
        /// Creates a new instance for a given operator
        /// </summary>
        /// <param name="offset">The position in the source code</param>
        /// <param name="operator">The operator to wrap</param>
        public Brainf_ckBinaryItem(uint offset, char @operator)
        {
            Offset = offset;
            Operator = @operator;
        }
    }
}