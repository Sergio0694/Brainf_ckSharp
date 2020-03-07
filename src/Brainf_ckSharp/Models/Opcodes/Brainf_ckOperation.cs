using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Brainf_ckSharp.Models.Opcodes.Interfaces;

namespace Brainf_ckSharp.Models.Opcodes
{
    /// <summary>
    /// A model that represents an RLE-compressed operator
    /// </summary>
    [DebuggerDisplay("('{Operator}', {Count})")]
    [StructLayout(LayoutKind.Explicit, Size = 4)]
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
            Unsafe.As<Brainf_ckOperation, byte>(ref this) = op;

            ref ushort r0 = ref Unsafe.As<Brainf_ckOperation, ushort>(ref this);
            ref ushort r1 = ref Unsafe.Add(ref r0, 1);

            r1 = count;
        }

        /// <inheritdoc/>
        public byte Operator
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref Brainf_ckOperation r0 = ref Unsafe.AsRef(this);
                ref byte r1 = ref Unsafe.As<Brainf_ckOperation, byte>(ref r0);

                return r1;
            }
        }

        /// <summary>
        /// Gets the number of times to repeat the operator
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref Brainf_ckOperation r0 = ref Unsafe.AsRef(this);
                ref ushort r1 = ref Unsafe.As<Brainf_ckOperation, ushort>(ref r0);
                ref ushort r2 = ref Unsafe.Add(ref r1, 1);

                return r2;
            }
        }
    }
}
