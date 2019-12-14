using System;
using System.Diagnostics.Contracts;
using Brainf_ck_sharp.NET.Buffers;
using Brainf_ck_sharp.NET.Enums;
using Brainf_ck_sharp.NET.Extensions;

namespace Brainf_ck_sharp.NET.Models
{
    /// <summary>
    /// A <see langword="class"/> that holds the working data for the internal interpreter function
    /// </summary>
    internal sealed class InterpreterWorkingData
    {
        /// <summary>
        /// Gets the exit code of the interpreter working data
        /// </summary>
        public InterpreterExitCode ExitCode { get; }

        /// <summary>
        /// Gets the stack frames for the working data
        /// </summary>
        public ReadOnlyMemory<UnsafeMemory<Brainf_ckBinaryItem>> StackFrames { get; }

        /// <summary>
        /// Gets the code position relative to the current working data
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the number of total evaluated operators in the current working data
        /// </summary>
        public int TotalOperations { get; }

        /// <summary>
        /// Creates a new <see cref="InterpreterWorkingData"/> instance with the specified parameters
        /// </summary>
        /// <param name="code">The exit code for the executed script</param>
        /// <param name="frame">The stack frame that was being used when creating the partial execution result</param>
        /// <param name="position">The offset into the executable that was reached when creating the partial result</param>
        /// <param name="operations">The total number of executed operators up to this point</param>
        public InterpreterWorkingData(InterpreterExitCode code, UnsafeMemory<Brainf_ckBinaryItem> frame, int position, int operations)
            : this(code, new[] { frame }, position, operations)
        { }

        /// <summary>
        /// Creates a new <see cref="InterpreterWorkingData"/> instance with the specified parameters
        /// </summary>
        /// <param name="code">The exit code for the executed script</param>
        /// <param name="frames">The stack frames that were being used when creating the partial execution result</param>
        /// <param name="position">The offset into the executable that was reached when creating the partial result</param>
        /// <param name="operations">The total number of executed operators up to this point</param>
        private InterpreterWorkingData(InterpreterExitCode code, ReadOnlyMemory<UnsafeMemory<Brainf_ckBinaryItem>> frames, int position, int operations)
        {
            ExitCode = code;
            StackFrames = frames;
            Position = position;
            TotalOperations = operations;
        }

        /// <summary>
        /// Creates a new <see cref="InterpreterWorkingData"/> instance by stacking a new frame to the current one
        /// </summary>
        /// <param name="stackFrame">The parent stack frame to add to the current instance</param>
        /// <returns>A new <see cref="InterpreterWorkingData"/> instance with a new parent stack frame</returns>
        [Pure]
        public InterpreterWorkingData WithParentStackFrame(UnsafeMemory<Brainf_ckBinaryItem> stackFrame)
        {
            ReadOnlyMemory<UnsafeMemory<Brainf_ckBinaryItem>> frames = StackFrames.Concat(stackFrame);

            return new InterpreterWorkingData(ExitCode, frames, Position, TotalOperations);
        }
    }
}
