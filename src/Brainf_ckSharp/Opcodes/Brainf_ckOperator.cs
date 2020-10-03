using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Opcodes.Interfaces;

namespace Brainf_ckSharp.Opcodes
{
    /// <summary>
    /// A model that represents a Brainf*ck/PBrain opcode
    /// </summary>
    [DebuggerDisplay("'{Operator}'")]
    internal readonly struct Brainf_ckOperator : IOpcode
    {
        /// <summary>
        /// Creates a new <see cref="Brainf_ckOperator"/> instance with the specified values
        /// </summary>
        /// <param name="op">The input operator for the new instance</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Brainf_ckOperator(byte op)
        {
            Operator = op;
        }

        /// <inheritdoc/>
        public byte Operator { get; }

        /// <summary>
        /// Creates a new <see cref="Brainf_ckOperator"/> instance from a specified operator
        /// </summary>
        /// <param name="op">The input operator to convert</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Brainf_ckOperator(byte op) => new Brainf_ckOperator(op);
    }
}
