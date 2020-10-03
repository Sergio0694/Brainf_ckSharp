using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Opcodes.Interfaces;

namespace Brainf_ckSharp.Opcodes
{
    /// <summary>
    /// A model that represents an RLE-compressed operator
    /// </summary>
    [DebuggerDisplay("('{Operator}', {Count})")]
    internal readonly struct Brainf_ckOperation : IOpcode
    {
        /// <summary>
        /// Creates a new <see cref="Brainf_ckOperation"/> instance with the specified values
        /// </summary>
        /// <param name="op"></param>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Brainf_ckOperation(byte op, ushort count)
        {
            Operator = op;
            Count = count;
        }

        /// <inheritdoc/>
        public byte Operator { get; }

        /// <summary>
        /// Gets the number of times to repeat the operator
        /// </summary>
        public ushort Count { get; }
    }
}
