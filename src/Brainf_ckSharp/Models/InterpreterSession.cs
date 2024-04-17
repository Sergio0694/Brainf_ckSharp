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
    private readonly MemoryOwner<Brainf_ckOperator> Opcodes;

    /// <summary>
    /// The table of breakpoints for the current executable
    /// </summary>
    private readonly MemoryOwner<bool> Breakpoints;

    /// <summary>
    /// The jump table for loops and function declarations
    /// </summary>
    private readonly MemoryOwner<int> JumpTable;

    /// <summary>
    /// The mapping of functions for the current execution
    /// </summary>
    private readonly MemoryOwner<Range> Functions;

    /// <summary>
    /// The lookup table to check which functions are defined
    /// </summary>
    private readonly MemoryOwner<ushort> Definitions;

    /// <summary>
    /// The sequence of stack frames for the current execution
    /// </summary>
    private readonly MemoryOwner<StackFrame> StackFrames;

    /// <summary>
    /// The target <see cref="TuringMachineState"/> instance to execute the code on
    /// </summary>
    private readonly TuringMachineState MachineState;

    /// <summary>
    /// The input buffer to read characters from
    /// </summary>
    private StdinBuffer StdinBuffer;

    /// <summary>
    /// The output buffer to write characters to
    /// </summary>
    private StdoutBuffer StdoutBuffer;

    /// <summary>
    /// The current stack depth
    /// </summary>
    private int _Depth;

    /// <summary>
    /// The total number of executed opcodes
    /// </summary>
    private int _TotalOperations;

    /// <summary>
    /// The total number of defined functions
    /// </summary>
    private int _TotalFunctions;

    /// <summary>
    /// A <see cref="CancellationToken"/> that can be used to halt the execution
    /// </summary>
    private readonly CancellationToken ExecutionToken;

    /// <summary>
    /// A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints
    /// </summary>
    private readonly CancellationToken DebugToken;

    /// <summary>
    /// A stopwatch used to keep track of the elapsed time during the execution of the script
    /// </summary>
    private readonly Stopwatch Stopwatch;

    /// <summary>
    /// The original source code for the interpreted script
    /// </summary>
    private readonly string SourceCode;

    /// <summary>
    /// Indicates whether or not the current instance has already been disposed
    /// </summary>
    private bool _Disposed;

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
        this.Opcodes = opcodes;
        this.Breakpoints = breakpoints;
        this.JumpTable = jumpTable;
        this.Functions = functions;
        this.Definitions = definitions;
        this.StackFrames = stackFrames;
        this.MachineState = machineState;
        this.StdinBuffer = new StdinBuffer(stdin);
        this.StdoutBuffer = StdoutBuffer.Allocate();
        this.ExecutionToken = executionToken;
        this.DebugToken = debugToken;
        this.Stopwatch = new Stopwatch();
        this.SourceCode = Brainf_ckParser.ExtractSource(opcodes.Span);
    }

    private InterpreterResult? _Current;

    /// <inheritdoc/>
    public InterpreterResult Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this._Current ?? ThrowHelper.ThrowInvalidOperationException<InterpreterResult>("The session has not been initialized yet");
    }

    /// <inheritdoc/>
    object IEnumerator.Current => Current;

    /// <inheritdoc/>
    public bool MoveNext()
    {
        // Check whether the current session can go ahead by one step
        if (this._Current != null &&
            (this._Current.ExitCode.HasFlag(ExitCode.Failure) ||
             !this._Current.ExitCode.HasFlag(ExitCode.BreakpointReached)))
        {
            return false;
        }

        // Execute the mode specific implementation
        switch (this.MachineState.Mode)
        {
            case OverflowMode.ByteWithOverflow: MoveNext<TuringMachineState.ByteWithOverflowExecutionContext>(); break;
            case OverflowMode.ByteWithNoOverflow: MoveNext<TuringMachineState.ByteWithNoOverflowExecutionContext>(); break;
            case OverflowMode.UshortWithOverflow: MoveNext<TuringMachineState.UshortWithOverflowExecutionContext>(); break;
            case OverflowMode.UshortWithNoOverflow: MoveNext<TuringMachineState.UshortWithNoOverflowExecutionContext>(); break;
            default: ThrowHelper.ThrowArgumentOutOfRangeException(nameof(this.MachineState.Mode), $"Invalid execution mode: {this.MachineState.Mode}"); break;
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

        using (TuringMachineState.ExecutionSession<TExecutionContext> session = this.MachineState.CreateExecutionSession<TExecutionContext>())
        {
            this.Stopwatch.Start();

            // Setup the stdin and stdout readers and writers
            StdinBuffer.Reader stdinReader = this.StdinBuffer.CreateReader();
            StdoutBuffer.Writer stdoutWriter = this.StdoutBuffer.CreateWriter();

            // Execute the new interpreter debug step
            exitCode = Brainf_ckInterpreter.Debug.Run(
                ref Unsafe.AsRef(session.ExecutionContext),
                ref this.Opcodes.DangerousGetReference(),
                ref this.Breakpoints.DangerousGetReference(),
                ref this.JumpTable.DangerousGetReference(),
                ref this.Functions.DangerousGetReference(),
                ref this.Definitions.DangerousGetReference(),
                ref this.StackFrames.DangerousGetReference(),
                ref this._Depth,
                ref this._TotalOperations,
                ref this._TotalFunctions,
                ref stdinReader,
                ref stdoutWriter,
                this.ExecutionToken,
                this.DebugToken);

            // Synchronize the buffers
            this.StdinBuffer.Synchronize(ref stdinReader);
            this.StdoutBuffer.Synchronize(ref stdoutWriter);

            this.Stopwatch.Stop();
        }

        // Prepare the debug info
        HaltedExecutionInfo? debugInfo = Brainf_ckInterpreter.LoadDebugInfo(
            this.Opcodes.Span,
            this.StackFrames.Span,
            this._Depth);

        // Build the collection of defined functions
        FunctionDefinition[] functionDefinitions = Brainf_ckInterpreter.LoadFunctionDefinitions(
            this.Opcodes.Span,
            this.Functions.Span,
            this.Definitions.Span,
            this._TotalFunctions);

        // Update the current interpreter result
        this._Current = new InterpreterResult(
            this.SourceCode,
            exitCode,
            debugInfo,
            (TuringMachineState)this.MachineState.Clone(),
            functionDefinitions,
            this.StdinBuffer.ToString(),
            this.StdoutBuffer.ToString(),
            this.Stopwatch.Elapsed,
            this._TotalOperations);
    }

    /// <inheritdoc/>
    public void Reset() => throw new NotSupportedException("An interpreter session can't be reset");

    ~InterpreterSession() => Dispose();

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this._Disposed) return;

        this._Disposed = true;

        this.Opcodes.Dispose();
        this.Breakpoints.Dispose();
        this.JumpTable.Dispose();
        this.Functions.Dispose();
        this.Definitions.Dispose();
        this.StackFrames.Dispose();
        this.StackFrames.Dispose();
        this.StdoutBuffer.Dispose();
        this.Stopwatch.Stop();
    }
}
