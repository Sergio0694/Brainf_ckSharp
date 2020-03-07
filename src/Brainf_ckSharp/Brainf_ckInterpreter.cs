using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Models.Internal;
using Brainf_ckSharp.Models.Opcodes;

namespace Brainf_ckSharp
{
    /// <summary>
    /// A <see langword="class"/> responsible for interpreting and debugging Brainf*ck/PBrain scripts
    /// </summary>
    public static partial class Brainf_ckInterpreter
    {
        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterResult> TryRun(string source)
        {
            return TryRun(source, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, CancellationToken.None);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, CancellationToken executionToken)
        {
            return TryRun(source, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, executionToken);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterResult> TryRun(string source, string stdin)
        {
            return TryRun(source, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, CancellationToken.None);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, CancellationToken executionToken)
        {
            return TryRun(source, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, executionToken);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize)
        {
            return TryRun(source, stdin, memorySize, Specs.DefaultOverflowMode, CancellationToken.None);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize, CancellationToken executionToken)
        {
            return TryRun(source, stdin, memorySize, Specs.DefaultOverflowMode, executionToken);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize, OverflowMode overflowMode)
        {
            return TryRun(source, stdin, memorySize, overflowMode, CancellationToken.None);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize, OverflowMode overflowMode, CancellationToken executionToken)
        {
            Guard.MustBeGreaterThanOrEqualTo(memorySize, Specs.MinimumMemorySize, nameof(memorySize));
            Guard.MustBeLessThanOrEqualTo(memorySize, Specs.MaximumMemorySize, nameof(memorySize));

            using PinnedUnmanagedMemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            TuringMachineState machineState = new TuringMachineState(memorySize, overflowMode);
            InterpreterResult result = Release.RunCore(operations!, stdin, machineState, executionToken);

            return Option<InterpreterResult>.From(validationResult, result);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterResult> TryRun(string source, IReadOnlyTuringMachineState initialState)
        {
            return TryRun(source, string.Empty, initialState, CancellationToken.None);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, IReadOnlyTuringMachineState initialState, CancellationToken executionToken)
        {
            return TryRun(source, string.Empty, initialState, executionToken);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterResult> TryRun(string source, string stdin, IReadOnlyTuringMachineState initialState)
        {
            return TryRun(source, stdin, initialState, CancellationToken.None);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, IReadOnlyTuringMachineState initialState, CancellationToken executionToken)
        {
            DebugGuard.MustBeTrue(initialState is TuringMachineState, nameof(initialState));

            using PinnedUnmanagedMemoryOwner<Brainf_ckOperation>? operations = Brainf_ckParser.TryParse<Brainf_ckOperation>(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            TuringMachineState machineState = (TuringMachineState)initialState.Clone();
            InterpreterResult result = Release.RunCore(operations!, stdin, machineState, executionToken);

            return Option<InterpreterResult>.From(validationResult, result);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, CancellationToken.None, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a second <see cref="CancellationToken"/> to be able to stop triggering breakpoints</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, CancellationToken executionToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, executionToken, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, CancellationToken executionToken, CancellationToken debugToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, executionToken, debugToken);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, CancellationToken.None, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a second <see cref="CancellationToken"/> to be able to stop triggering breakpoints</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, CancellationToken executionToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, executionToken, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, CancellationToken executionToken, CancellationToken debugToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode, executionToken, debugToken);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, memorySize, Specs.DefaultOverflowMode, CancellationToken.None, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a second <see cref="CancellationToken"/> to be able to stop triggering breakpoints</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, CancellationToken executionToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, memorySize, Specs.DefaultOverflowMode, executionToken, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, CancellationToken executionToken, CancellationToken debugToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, memorySize, Specs.DefaultOverflowMode, executionToken, debugToken);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a <see cref="CancellationToken"/> to prevent issues with non-halting scripts</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, OverflowMode overflowMode)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, memorySize, overflowMode, CancellationToken.None, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        /// <remarks>Consider using an overload with a second <see cref="CancellationToken"/> to be able to stop triggering breakpoints</remarks>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, OverflowMode overflowMode, CancellationToken executionToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, memorySize, overflowMode, executionToken, CancellationToken.None);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, OverflowMode overflowMode, CancellationToken executionToken, CancellationToken debugToken)
        {
            return Debug.TryCreateSessionCore(source, breakpoints, stdin, memorySize, overflowMode, executionToken, debugToken);
        }
    }
}
