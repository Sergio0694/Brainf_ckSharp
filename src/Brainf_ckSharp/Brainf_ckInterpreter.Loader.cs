using System;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Opcodes.Interfaces;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using StackFrame = Brainf_ckSharp.Models.Internal.StackFrame;
using Range = Brainf_ckSharp.Models.Internal.Range;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp;

public static partial class Brainf_ckInterpreter
{
    /// <summary>
    /// Loads the function definitions with the given executable and parameters
    /// </summary>
    /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
    /// <param name="opcodes">The sequence of parsed opcodes to execute</param>
    /// <param name="functions">The mapping of functions for the current execution</param>
    /// <param name="definitions">The lookup table to check which functions are defined</param>
    /// <param name="totalFunctions">The total number of defined functions</param>
    /// <returns>An array of <see cref="FunctionDefinition"/> instance with the defined functions</returns>
    internal static FunctionDefinition[] LoadFunctionDefinitions<TOpcode>(
        Span<TOpcode> opcodes,
        Span<Range> functions,
        Span<ushort> definitions,
        int totalFunctions)
        where TOpcode : unmanaged, IOpcode
    {
        Assert(opcodes.Length >= 0);
        Assert(functions.Length == ushort.MaxValue);
        Assert(definitions.Length >= 0);
        Assert(definitions.Length <= opcodes.Length / 3);
        Assert(totalFunctions >= 0);

        // No declared functions
        if (totalFunctions == 0) return Array.Empty<FunctionDefinition>();

        FunctionDefinition[] result = new FunctionDefinition[totalFunctions];
        ref FunctionDefinition r0 = ref result.DangerousGetReference();

        // Process all the declared functions
        for (int i = 0; i < totalFunctions; i++)
        {
            ushort key = definitions[i];
            Range range = functions[key];
            Span<TOpcode> slice = opcodes.Slice(range.Start, range.Length);

            string body = Brainf_ckParser.ExtractSource(slice);

            int offset = range.Start - 1; // The range starts at the first function operator

            Unsafe.Add(ref r0, i) = new FunctionDefinition(key, i, offset, body);
        }

        return result;
    }

    /// <summary>
    /// Loads the jump table for loops and functions from a given executable
    /// </summary>
    /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
    /// <param name="opcodes">The sequence of parsed opcodes to inspect</param>
    /// <param name="jumpTable">The jump table to populate</param>
    /// <param name="functionsCount">The total number of declared functions in the input sequence of opcodes</param>
    private static void LoadJumpTable<TOpcode>(
        Span<TOpcode> opcodes,
        Span<int> jumpTable,
        out int functionsCount)
        where TOpcode : unmanaged, IOpcode
    {
        Assert(opcodes.Length >= 0);

        // Temporarily allocate two buffers to store the indirect indices to build the jump table.
        // The two temporary buffers are initialized with a size of half the length of the input
        // executable, because that is the maximum number of open square brackets in a valid source file.
        // The two temporary buffers are used to implement an indirect indexing system while building
        // the table, which allows to reduce the complexity of the operation from O(N^2) to O(N).
        int tempBuffersLength = (opcodes.Length / 2) + 1;
        using SpanOwner<int> rootTempIndices = SpanOwner<int>.Allocate(tempBuffersLength);
        using SpanOwner<int> functionTempIndices = SpanOwner<int>.Allocate(tempBuffersLength);
        ref int rootTempIndicesRef = ref rootTempIndices.DangerousGetReference();
        ref int functionTempIndicesRef = ref functionTempIndices.DangerousGetReference();
        ref int jumpTableRef = ref jumpTable.DangerousGetReference();
        functionsCount = 0;

        // Go through the executable to build the jump table for each open parenthesis or square bracket
        for (int r = 0, f = -1, i = 0; i < opcodes.Length; i++)
        {
            switch (opcodes[i].Operator)
            {
                // When a loop start, the current index is stored in the right
                // temporary buffer, depending on whether or not the current
                // part of the executable is within a function definition
                case Operators.LoopStart:
                    if (f == -1) Unsafe.Add(ref rootTempIndicesRef, r++) = i;
                    else Unsafe.Add(ref functionTempIndicesRef, f++) = i;
                    break;

                // When a loop ends, the index of the corresponding open square
                // bracket is retrieved from the right temporary buffer, and the
                // current index is stored at that location in the final jump table
                // being built. The inverse mapping is stored too, so that each
                // closing square bracket can reference the corresponding open
                // bracket at the start of the loop.
                case Operators.LoopEnd:
                    int start = f == -1
                        ? Unsafe.Add(ref rootTempIndicesRef, --r)
                        : Unsafe.Add(ref functionTempIndicesRef, --f);
                    Unsafe.Add(ref jumpTableRef, start) = i;
                    Unsafe.Add(ref jumpTableRef, i) = start;
                    break;

                // When a function definition starts, the offset into the
                // temporary buffer for the function indices is set to 1.
                // This is because in this case a 1-based indexing is used:
                // the first location in the temporary buffer is used to store
                // the index of the open parenthesis for the function definition.
                case Operators.FunctionStart:
                    f = 1;
                    functionTempIndicesRef = i;
                    functionsCount++;
                    break;
                case Operators.FunctionEnd:
                    f = -1;
                    Unsafe.Add(ref jumpTableRef, functionTempIndicesRef) = i;
                    Unsafe.Add(ref jumpTableRef, i) = functionTempIndicesRef;
                    break;
            }
        }
    }

    /// <summary>
    /// Loads the <see cref="HaltedExecutionInfo"/> instance for a halted execution of a script, if available
    /// </summary>
    /// <typeparam name="TOpcode">The type of opcode to process</typeparam>
    /// <param name="opcodes">The sequence of parsed opcodes to execute</param>
    /// <param name="stackFrames">The sequence of stack frames for the current execution</param>
    /// <param name="depth">The current stack depth</param>
    /// <returns>An <see cref="HaltedExecutionInfo"/> instance, if the input script was halted during its execution</returns>
    internal static HaltedExecutionInfo? LoadDebugInfo<TOpcode>(
        Span<TOpcode> opcodes,
        Span<StackFrame> stackFrames,
        int depth)
        where TOpcode : unmanaged, IOpcode
    {
        Assert(opcodes.Length > 0);
        Assert(stackFrames.Length == Specs.MaximumStackSize);
        Assert(depth >= -1);

        // No exception info for scripts completed successfully
        if (depth == -1) return null;

        string[] stackTrace = new string[depth + 1];
        ref string r0 = ref stackTrace.DangerousGetReference();

        // Process all the stack frames
        for (int i = 0, j = depth; j >= 0; i++, j--)
        {
            StackFrame frame = stackFrames[j];

            // Adjust the offset and process the current range.
            // This is needed because in case of a partial execution, no matter
            // if it's a breakpoint or a crash, the stored offset in the top stack
            // frame will be the operator currently being executed, which needs to
            // be included in the processed string. For stack frames below that
            // instead, the offset already refers to the operator immediately after
            // the function call operator, so the offset doesn't need to be shifted
            // ahead before extracting the processed string. Doing this with a
            // reinterpret cast saves a conditional jump in the asm code.
            int
                start = frame.Range.Start,
                offset = frame.Offset + (i == 0).ToByte(),
                length = offset - start;
            Span<TOpcode> span = opcodes.Slice(start, length);

            string body = Brainf_ckParser.ExtractSource(span);

            Unsafe.Add(ref r0, i) = body;
        }

        // Extract the additional info
        int errorOffset = stackFrames[depth].Offset;
        char opcode = Brainf_ckParser.GetCharacterFromOpcode(opcodes[errorOffset]);

        return new(stackTrace, opcode, errorOffset);
    }
}
