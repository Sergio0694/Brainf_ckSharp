using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Opcodes.Interfaces;

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A container for execution parameters to use to run a script.
/// </summary>
/// <typeparam name="TOpcode">The type of opcode to execute.</typeparam>
/// <param name="opcodes"><inheritdoc cref="Opcodes" path="/node()"/></param>
/// <param name="jumpTable"><inheritdoc cref="JumpTable" path="/node()"/></param>
/// <param name="functions"><inheritdoc cref="Functions" path="/node()"/></param>
/// <param name="definitions"><inheritdoc cref="Definitions" path="/node()"/></param>
/// <param name="stackFrames"><inheritdoc cref="StackFrames" path="/node()"/></param>
/// <param name="depth"><inheritdoc cref="Depth" path="/node()"/></param>
/// <param name="totalOperations"><inheritdoc cref="TotalOperations" path="/node()"/></param>
/// <param name="totalFunctions"><inheritdoc cref="TotalFunctions" path="/node()"/></param>
/// <param name="stdinReader"><inheritdoc cref="StdinReader" path="/node()"/></param>
/// <param name="stdoutWriter"><inheritdoc cref="StdoutWriter" path="/node()"/></param>
/// <param name="executionToken"><inheritdoc cref="ExecutionToken" path="/node()"/></param>
internal readonly unsafe ref struct ExecutionParameters<TOpcode>(
    ref TOpcode opcodes,
    ref int jumpTable,
    ref Range functions,
    ref ushort definitions,
    ref StackFrame stackFrames,
    ref int depth,
    ref int totalOperations,
    ref int totalFunctions,
    ref StdinBuffer.Reader stdinReader,
    ref StdoutBuffer.Writer stdoutWriter,
    CancellationToken executionToken)
    where TOpcode : unmanaged, IOpcode
{
    /// <summary>
    /// The sequence of parsed opcodes to execute.
    /// </summary>
    public readonly ref TOpcode Opcodes = ref opcodes;

    /// <summary>
    /// The jump table for loops and function declarations.
    /// </summary>
    public readonly ref int JumpTable = ref jumpTable;

    /// <summary>
    /// The mapping of functions for the current execution.
    /// </summary>
    public readonly ref Range Functions = ref functions;

    /// <summary>
    /// The lookup table to check which functions are defined.
    /// </summary>
    public readonly ref ushort Definitions = ref definitions;

    /// <summary>
    /// The sequence of stack frames for the current execution.
    /// </summary>
    public readonly ref StackFrame StackFrames = ref stackFrames;

    /// <summary>
    /// The current stack depth.
    /// </summary>
    public readonly ref int Depth = ref depth;

    /// <summary>
    /// The total number of executed opcodes.
    /// </summary>
    public readonly ref int TotalOperations = ref totalOperations;

    /// <summary>
    /// The total number of defined functions.
    /// </summary>
    public readonly ref int TotalFunctions = ref totalFunctions;

    /// <summary>
    /// The input buffer to read characters from.
    /// </summary>
    public readonly StdinBuffer.Reader* StdinReader = (StdinBuffer.Reader*)Unsafe.AsPointer(ref stdinReader);

    /// <summary>
    /// The output buffer to write characters to.
    /// </summary>
    public readonly StdoutBuffer.Writer* StdoutWriter = (StdoutBuffer.Writer*)Unsafe.AsPointer(ref stdoutWriter);

    /// <summary>
    /// The <see cref="CancellationToken"/> that can be used to halt the execution.
    /// </summary>
    public readonly CancellationToken ExecutionToken = executionToken;
}
