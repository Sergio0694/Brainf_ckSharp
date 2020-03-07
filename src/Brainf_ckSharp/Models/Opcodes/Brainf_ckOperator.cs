using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Models.Opcodes.Interfaces;

namespace Brainf_ckSharp.Models.Opcodes
{
    /// <summary>
    /// A model that represents a Brainf*ck/PBrain opcode
    /// </summary>
    [DebuggerDisplay("'{Operator}')")]
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public readonly struct Brainf_ckOperator : IOpcode
    {
        /// <summary>
        /// Creates a new <see cref="Brainf_ckOperation"/> instance with the specified values
        /// </summary>
        /// <param name="op"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Brainf_ckOperator(byte op)
        {
            Unsafe.As<Brainf_ckOperator, byte>(ref this) = op;
        }

        /// <inheritdoc/>
        public byte Operator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref Brainf_ckOperator r0 = ref Unsafe.AsRef(this);
                ref byte r1 = ref Unsafe.As<Brainf_ckOperator, byte>(ref r0);

                return r1;
            }
        }
    }
}
