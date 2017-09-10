using System.Collections.Generic;
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
        public IEnumerable<IEnumerable<Brainf_ckBinaryItem>> StackFrames { get; }

        /// <summary>
        /// Gets the code position relative to the current working set
        /// </summary>
        public uint Position { get; }

        /// <summary>
        /// Gets the number of total evaluated operators in the current working set
        /// </summary>
        public uint TotalOperations { get; }

        // Internal constructor
        public InterpreterWorkingData(InterpreterExitCode code, [CanBeNull] IEnumerable<IEnumerable<Brainf_ckBinaryItem>> frames, uint position, uint operations)
        {
            ExitCode = code;
            StackFrames = frames;
            Position = position;
            TotalOperations = operations;
        }
    }
}
