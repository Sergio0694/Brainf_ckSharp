namespace Brainf_ckSharp.Models.Internal
{ 
    /// <summary>
    /// A model that represents an RLE-compressed operator
    /// </summary>
    internal readonly struct Brainf_ckOperation
    {
        /// <summary>
        /// The operator to execute
        /// </summary>
        public readonly byte Operator;

        /// <summary>
        /// The number of times to repeat the operator
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// Creates a new <see cref="Brainf_ckOperation"/> instance with the specified values
        /// </summary>
        /// <param name="op"></param>
        /// <param name="count"></param>
        public Brainf_ckOperation(byte op, int count)
        {
            Operator = op;
            Count = count;
        }
    }
}
