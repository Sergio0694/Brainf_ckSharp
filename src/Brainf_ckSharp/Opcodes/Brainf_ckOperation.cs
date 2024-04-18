using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Opcodes.Interfaces;

namespace Brainf_ckSharp.Opcodes;

/// <summary>
/// A model that represents an RLE-compressed operator
/// </summary>
/// <param name="op"></param>
/// <param name="count"></param>
[DebuggerDisplay("('{Operator}', {Count})")]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct Brainf_ckOperation(byte op, ushort count) : IOpcode
{
    /// <inheritdoc/>
    public byte Operator { get; } = op;

    /// <summary>
    /// Gets the number of times to repeat the operator
    /// </summary>
    public ushort Count { get; } = count;
}
