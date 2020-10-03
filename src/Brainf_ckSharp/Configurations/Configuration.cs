using System;
using System.Threading;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;

#pragma warning disable CS0282

namespace Brainf_ckSharp.Configurations
{
    /// <summary>
    /// A model for a DEBUG configuration being built
    /// </summary>
    public readonly ref partial struct DebugConfiguration
    {
        /// <summary>
        /// The source code to parse and execute
        /// </summary>
        public readonly ReadOnlyMemory<char>? Source;

        /// <summary>
        /// The (optional) stdin buffer to use to run the script
        /// </summary>
        public readonly ReadOnlyMemory<char>? Stdin;

        /// <summary>
        /// The (optional) initial machine state to use to execute the script
        /// </summary>
        public readonly IReadOnlyMachineState? InitialState;

        /// <summary>
        /// The (optional) memory size for the machine state to use
        /// </summary>
        public readonly int? MemorySize;

        /// <summary>
        /// The (optional) overflow mode to use to run the script
        /// </summary>
        public readonly OverflowMode? OverflowMode;

        /// <summary>
        /// The token to cancel a long running execution
        /// </summary>
        public readonly CancellationToken ExecutionToken;
    }

    /// <summary>
    /// A model for a RELEASE configuration being built
    /// </summary>
    public readonly ref partial struct ReleaseConfiguration
    {
        /// <summary>
        /// The source code to parse and execute
        /// </summary>
        public readonly ReadOnlyMemory<char>? Source;

        /// <summary>
        /// The (optional) stdin buffer to use to run the script
        /// </summary>
        public readonly ReadOnlyMemory<char>? Stdin;

        /// <summary>
        /// The (optional) initial machine state to use to execute the script
        /// </summary>
        public readonly IReadOnlyMachineState? InitialState;

        /// <summary>
        /// The (optional) memory size for the machine state to use
        /// </summary>
        public readonly int? MemorySize;

        /// <summary>
        /// The (optional) overflow mode to use to run the script
        /// </summary>
        public readonly OverflowMode? OverflowMode;

        /// <summary>
        /// The token to cancel a long running execution
        /// </summary>
        public readonly CancellationToken ExecutionToken;
    }
}
