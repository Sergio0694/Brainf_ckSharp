using System;
using System.Collections.Generic;
using System.Text;
using Brainf_ck_sharp.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Helpers
{
    /// <summary>
    /// A lass that holds the working data for the internal interpreter function
    /// </summary>
    internal sealed class InterpreterWorkingData
    {
        /// <summary>
        /// Gets the exit code of the interpreter working set
        /// </summary>
        public InterpreterExitCode ExitCode { get; }

        /// <summary>
        /// Gets the stack frames for the working set
        /// </summary>
        [CanBeNull]
        public IEnumerable<IEnumerable<char>> StackFrames { get; }

        /// <summary>
        /// Gets the code position relative to the current working set
        /// </summary>
        public uint Position { get; }

        /// <summary>
        /// Gets whether or not a breakpoint has been reached in the current working set
        /// </summary>
        public bool BreakpointReached { get; }

        /// <summary>
        /// Gets the number of total evaluated operators in the current working set
        /// </summary>
        public uint TotalOperations { get; }

        // Internal constructor
        public InterpreterWorkingData(InterpreterExitCode code, [CanBeNull] IEnumerable<IEnumerable<char>> frames, uint position, bool reached, uint operations)
        {
            ExitCode = code;
            StackFrames = frames;
            Position = position;
            BreakpointReached = reached;
            TotalOperations = operations;
        }
    }
}
