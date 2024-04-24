using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Opcodes.Interfaces;

namespace Brainf_ckSharp.Opcodes;

/// <summary>
/// A model that represents a Brainf*ck/PBrain opcode
/// </summary>
/// <param name="op">The input operator for the new instance</param>
[DebuggerDisplay("'{Operator}'")]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct Brainf_ckOperator(byte op) : IOpcode
{
    /// <inheritdoc/>
    public byte Operator { get; } = op;

    /// <summary>
    /// Creates a new <see cref="Brainf_ckOperator"/> instance from a specified operator
    /// </summary>
    /// <param name="op">The input operator to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Brainf_ckOperator(byte op) => new(op);
}
