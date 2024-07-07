using System.Threading;

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A container for debug parameters to use to run a script.
/// </summary>
/// <param name="breakpoints"><inheritdoc cref="Breakpoints" path="/node()"/></param>
/// <param name="totalOperations"><inheritdoc cref="TotalOperations" path="/node()"/></param>
/// <param name="debugToken"><inheritdoc cref="DebugToken" path="/node()"/></param>
internal readonly ref struct DebugParameters(
    ref bool breakpoints,
    ref int totalOperations,
    CancellationToken debugToken)
{
    /// <summary>
    /// The table of breakpoints for the current executable
    /// </summary>
    public readonly ref bool Breakpoints = ref breakpoints;

    /// <summary>
    /// The total number of executed opcodes.
    /// </summary>
    public readonly ref int TotalOperations = ref totalOperations;

    /// <summary>
    /// The <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints
    /// </summary>
    public readonly CancellationToken DebugToken = debugToken;
}
