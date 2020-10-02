using System;
using System.Collections.Generic;
using System.Threading;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Opcodes;

namespace Brainf_ckSharp.Memory.Interfaces
{
    /// <summary>
    /// An <see langword="interface"/> that represents a machine state
    /// </summary>
    internal interface IMachineState : IReadOnlyMachineState
    {
        /// <summary>
        /// Runs a given Brainf*ck/PBrain executable with the given parameters
        /// </summary>
        /// <param name="opcodes">The executable to run</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <returns>An <see cref="InterpreterResult"/> instance with the results of the execution</returns>
        InterpreterResult Run(
            Span<Brainf_ckOperation> opcodes,
            string stdin,
            CancellationToken executionToken);

        /// <summary>
        /// Creates a new Brainf*ck/PBrain session with the given parameters
        /// </summary>
        /// <param name="source">The source code to parse and execute</param>
        /// <param name="breakpoints">The sequence of indices for the breakpoints to apply to the script</param>
        /// <param name="stdin">The input buffer to read data from</param>
        /// <param name="executionToken">A <see cref="CancellationToken"/> that can be used to halt the execution</param>
        /// <param name="debugToken">A <see cref="CancellationToken"/> that is used to ignore/respect existing breakpoints</param>
        /// <returns>An <see cref="Option{T}"/> of <see cref="InterpreterSession"/> instance with the results of the execution</returns>
        Option<IEnumerator<InterpreterResult>> TryCreateSession(
            ReadOnlySpan<char> source,
            ReadOnlySpan<int> breakpoints,
            string stdin,
            CancellationToken executionToken,
            CancellationToken debugToken);
    }
}