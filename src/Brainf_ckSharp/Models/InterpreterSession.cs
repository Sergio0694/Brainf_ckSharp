using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Brainf_ckSharp.Buffers;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Opcodes;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using Range = Brainf_ckSharp.Models.Internal.Range;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Brainf_ckSharp.Models;

/// <summary>
/// A <see langword="class"/> that represents an interpreter session executing on a given script
/// </summary>
public sealed class InterpreterSession : IEnumerator<InterpreterResult>
{
    /// <summary>
    /// The sequence of parsed opcodes to execute
    /// </summary>
    private readonly MemoryOwner<Brainf_ckOperator> opcodes;

    /// <summary>
    /// The table of breakpoints for the current executable
    /// </summary>
    private readonly MemoryOwner<bool> breakpoints;

    /// <summary>
    /// The jump table for loops and function declarations
    /// </summary>
    private readonly MemoryOwner<int> jumpTable;

    /// <summary>
    /// The mapping of functions for the current execution
    /// </summary>
    private readonly MemoryOwner<Range> functions;

    /// <summary>
    /// The lookup table to check which functions are defined
    /// </summary>
    private readonly MemoryOwner<ushort> definitions;

    /// <summary>
    /// The sequence of stack frames for the current execution
    /// </summary>
    private readonly MemoryOwner<StackFrame> stackFrames;

    /// <summary>
    /// The target <see cref="TuringMachineState"/> instance to execute the code on
    /// </summary>
    private readonly TuringMachineState machineState;

    /// <summary>
    /// The input buffer to read characters from
    /// </summary>
    private StdinBuffer stdinBuffer;

    /// <summary>
    /// The output buffer to write characters to
    /// </summary>
    private StdoutBuffer stdoutBuffer;

    /// <summary>
    /// The current stack depth
    /// </summary>
    private int depth;

    /// <summary>
    /// The total number of executed opcodes
    /// </summary>
    private int totalOperations;

    /// <summary>
    /// The total number of defined functions
    /// </summary>
    private int totalFunctions;

    /// <summary>
    /// A <see cref="CancellationToken"/> that can be used to halt the execution
    /// </summary>
    private readonly CancellationToken executionToken;

    /// <summary>
    /// A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints
    /// </summary>
    private readonly CancellationToken debugToken;

    /// <summary>
    /// A stopwatch used to keep track of the elapsed time during the execution of the script
    /// </summary>
    private readonly Stopwatch stopwatch;

    /// <summary>
    /// The original source code for the interpreted script
    /// </summary>
    private readonly string sourceCode;

    /// <summary>
    /// Indicates whether or not the current instance has already been disposed
    /// </summary>
    private bool disposed;

    /// <summary>
    /// The backing field for <see cref="Current"/>.
    /// </summary>
    private InterpreterResult? current;

    /// <summary>
    /// Creates a new <see cref="InterpreterSession"/> with the specified parameters
    /// </summary>
    /// <param name="opcodes">The sequence of parsed opcodes to execute</param>
    /// <param name="breakpoints">The table of breakpoints for the current executable</param>
    /// <param name="jumpTable">The jump table for loops and function declarations</param>
    /// <param name="functions">The mapping of functions for the current execution</param>
    /// <param name="definitions">The lookup table to check which functions are defined</param>
    /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
    /// <param name="stdin">The input <see cref="ReadOnlyMemory{T}"/> to read characters from</param>
    /// <param name="machineState">The target machine state to use to run the script</param>
    /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
    /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
    internal InterpreterSession(
        MemoryOwner<Brainf_ckOperator> opcodes,
        MemoryOwner<bool> breakpoints,
        MemoryOwner<int> jumpTable,
        MemoryOwner<Range> functions,
        MemoryOwner<ushort> definitions,
        MemoryOwner<StackFrame> stackFrames,
        ReadOnlyMemory<char> stdin,
        TuringMachineState machineState,
        CancellationToken executionToken,
        CancellationToken debugToken)
    {
        this.opcodes = opcodes;
        this.breakpoints = breakpoints;
        this.jumpTable = jumpTable;
        this.functions = functions;
        this.definitions = definitions;
        this.stackFrames = stackFrames;
        this.machineState = machineState;
        this.stdinBuffer = new StdinBuffer(stdin);
        this.stdoutBuffer = StdoutBuffer.Allocate();
        this.executionToken = executionToken;
        this.debugToken = debugToken;
        this.stopwatch = new Stopwatch();
        this.sourceCode = Brainf_ckParser.ExtractSource(opcodes.Span);
    }

    /// <inheritdoc/>
    public InterpreterResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.current ?? ThrowHelper.ThrowInvalidOperationException<InterpreterResult>("The session has not been initialized yet");
    }

    /// <inheritdoc/>
    object IEnumerator.Current => Current;

    /// <inheritdoc/>
    public bool MoveNext()
    {
        // Check whether the current session can go ahead by one step
        if (this.current != null &&
            (this.current.ExitCode.HasFlag(ExitCode.Failure) ||
             !this.current.ExitCode.HasFlag(ExitCode.BreakpointReached)))
        {
            return false;
        }

        // Execute the mode specific implementation
        switch (this.machineState.Mode)
        {
            case OverflowMode.ByteWithOverflow: MoveNext<TuringMachineState.ByteWithOverflowExecutionContext>(); break;
            case OverflowMode.ByteWithNoOverflow: MoveNext<TuringMachineState.ByteWithNoOverflowExecutionContext>(); break;
            case OverflowMode.UshortWithOverflow: MoveNext<TuringMachineState.UshortWithOverflowExecutionContext>(); break;
            case OverflowMode.UshortWithNoOverflow: MoveNext<TuringMachineState.UshortWithNoOverflowExecutionContext>(); break;
            default: ThrowHelper.ThrowArgumentOutOfRangeException(nameof(this.machineState.Mode), $"Invalid execution mode: {this.machineState.Mode}"); break;
        };

        return true;
    }

    /// <summary>
    /// Implements the <see cref="MoveNext"/> logic with a specific execution mode
    /// </summary>
    /// <typeparam name="TExecutionContext">The type implementing <see cref="IMachineStateExecutionContext"/> to use</typeparam>
    private void MoveNext<TExecutionContext>()
        where TExecutionContext : struct, IMachineStateExecutionContext
    {
        ExitCode exitCode;

        using (TuringMachineState.ExecutionSession<TExecutionContext> session = this.machineState.CreateExecutionSession<TExecutionContext>())
        {
            this.stopwatch.Start();

            // Setup the stdin and stdout readers and writers
            StdinBuffer.Reader stdinReader = this.stdinBuffer.CreateReader();
            StdoutBuffer.Writer stdoutWriter = this.stdoutBuffer.CreateWriter();

            // Execute the new interpreter debug step
            exitCode = Brainf_ckInterpreter.Debug.Run(
                ref Unsafe.AsRef(session.ExecutionContext),
                ref this.opcodes.DangerousGetReference(),
                ref this.breakpoints.DangerousGetReference(),
                ref this.jumpTable.DangerousGetReference(),
                ref this.functions.DangerousGetReference(),
                ref this.definitions.DangerousGetReference(),
                ref this.stackFrames.DangerousGetReference(),
                ref this.depth,
                ref this.totalOperations,
                ref this.totalFunctions,
                ref stdinReader,
                ref stdoutWriter,
                this.executionToken,
                this.debugToken);

            // Synchronize the buffers
            this.stdinBuffer.Synchronize(ref stdinReader);
            this.stdoutBuffer.Synchronize(ref stdoutWriter);

            this.stopwatch.Stop();
        }

        // Prepare the debug info
        HaltedExecutionInfo? debugInfo = Brainf_ckInterpreter.LoadDebugInfo(
            this.opcodes.Span,
            this.stackFrames.Span,
            this.depth);

        // Build the collection of defined functions
        FunctionDefinition[] functionDefinitions = Brainf_ckInterpreter.LoadFunctionDefinitions(
            this.opcodes.Span,
            this.functions.Span,
            this.definitions.Span,
            this.totalFunctions);

        // Update the current interpreter result
        this.current = new InterpreterResult(
            this.sourceCode,
            exitCode,
            debugInfo,
            (TuringMachineState)this.machineState.Clone(),
            functionDefinitions,
            this.stdinBuffer.ToString(),
            this.stdoutBuffer.ToString(),
            this.stopwatch.Elapsed,
            this.totalOperations);
    }

    /// <inheritdoc/>
    public void Reset()
    {
        throw new NotSupportedException("An interpreter session can't be reset");
    }

    /// <summary>
    /// Disposes resources when the instance is finalized.
    /// </summary>
    ~InterpreterSession() => Dispose();

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.disposed) return;

        this.disposed = true;

        this.opcodes.Dispose();
        this.breakpoints.Dispose();
        this.jumpTable.Dispose();
        this.functions.Dispose();
        this.definitions.Dispose();
        this.stackFrames.Dispose();
        this.stackFrames.Dispose();
        this.stdoutBuffer.Dispose();
        this.stopwatch.Stop();
    }
}
