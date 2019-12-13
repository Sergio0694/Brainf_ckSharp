using Brainf_ck_sharp.NET.Helpers;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// Wraps the operators in the source code of a given script
    /// </summary>
    internal readonly struct Brainf_ckBinaryItem
    {
        /// <summary>
        /// Gets the offset of the current operator from the start of the script
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// Gets the wrapped operator in the current instance
        /// </summary>
        public readonly char Operator;

        /// <summary>
        /// Creates a new instance for a given operator
        /// </summary>
        /// <param name="offset">The position in the source code</param>
        /// <param name="op">The operator to wrap</param>
        public Brainf_ckBinaryItem(int offset, char op)
        {
            DebugGuard.MustBeGreaterThanOrEqualTo(offset, 0, nameof(offset));

            Offset = offset;
            Operator = op;
        }
    }
}
