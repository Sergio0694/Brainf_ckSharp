using System;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Constants;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Helpers;
using Brainf_ck_sharp.NET.Interfaces;
using Brainf_ck_sharp.NET.Models;
using Brainf_ck_sharp.NET.Models.Base;
using Brainf_ck_sharp.NET.Models.Internal;

namespace Brainf_ck_sharp.NET
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
        public static Option<InterpreterResult> TryRun(string source)
        {
            return TryRun(source, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin)
        {
            return TryRun(source, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize)
        {
            return TryRun(source, stdin, memorySize, Specs.DefaultOverflowMode);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <param name="overflowMode">The overflow mode to use in the state machine used to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, int memorySize, OverflowMode overflowMode)
        {
            Guard.MustBeGreaterThanOrEqualTo(memorySize, 32, nameof(memorySize));
            Guard.MustBeLessThanOrEqualTo(memorySize, 1024, nameof(memorySize));

            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            TuringMachineState machineState = new TuringMachineState(memorySize, overflowMode);
            InterpreterResult result = RunCore(operators!, stdin, machineState);

            return Option<InterpreterResult>.From(validationResult, result);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, IReadOnlyTuringMachineState initialState)
        {
            return TryRun(source, string.Empty, initialState);
        }

        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="initialState">The initial state machine to use to start running the script from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        public static Option<InterpreterResult> TryRun(string source, string stdin, IReadOnlyTuringMachineState initialState)
        {
            DebugGuard.MustBeTrue(initialState is TuringMachineState, nameof(initialState));

            using UnsafeMemoryBuffer<byte>? operators = Brainf_ckParser.TryParse(source, out SyntaxValidationResult validationResult);

            if (!validationResult.IsSuccess) return Option<InterpreterResult>.From(validationResult);

            TuringMachineState machineState = (TuringMachineState)initialState.Clone();
            InterpreterResult result = RunCore(operators!, stdin, machineState);

            return Option<InterpreterResult>.From(validationResult, result);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints)
        {
            return TryCreateSessionCore(source, breakpoints, string.Empty, Specs.DefaultMemorySize, Specs.DefaultOverflowMode);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin)
        {
            return TryCreateSessionCore(source, breakpoints, stdin, Specs.DefaultMemorySize, Specs.DefaultOverflowMode);
        }

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="memorySize">The size of the state machine to create to run the script</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize)
        {
            return TryCreateSessionCore(source, breakpoints, stdin, memorySize, Specs.DefaultOverflowMode);
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
        public static Option<InterpreterSession> TryCreateSession(string source, ReadOnlySpan<int> breakpoints, string stdin, int memorySize, OverflowMode overflowMode)
        {
            return TryCreateSessionCore(source, breakpoints, stdin, memorySize, overflowMode);
        }
    }
}
