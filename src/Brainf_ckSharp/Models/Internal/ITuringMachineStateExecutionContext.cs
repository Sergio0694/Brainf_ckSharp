namespace Brainf_ckSharp.Models.Internal
{
    /// <summary>
    /// An <see langword="interface"/> that is used by mode-specific execution contexts
    /// </summary>
    internal interface IMachineStateExecutionContext
    {
        /// <summary>
        /// Gets the value at the current memory position
        /// </summary>
        ushort Current { get; }

        /// <summary>
        /// Tries to move the memory pointer forward
        /// </summary>
        /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>;
        bool TryMoveNext();

        /// <summary>
        /// Tries to move the memory pointer back
        /// </summary>
        /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
        bool TryMoveBack();

        /// <summary>
        /// Tries to increment the current memory location
        /// </summary>
        /// <returns><see langword="true"/> if the memory location was incremented successfully, <see langword="false"/> otherwise</returns>
        bool TryIncrement();

        /// <summary>
        /// Tries to decrement the current memory location
        /// </summary>
        /// <returns><see langword="true"/> if the memory location was decremented successfully, <see langword="false"/> otherwise</returns>
        bool TryDecrement();

        /// <summary>
        /// Tries to set the current memory location to the value of a given character
        /// </summary>
        /// <param name="c">The input charachter to assign to the current memory location</param>
        /// <returns><see langword="true"/> if the input value was read correctly, <see langword="false"/> otherwise</returns>
        bool TryInput(char c);

        /// <summary>
        /// Resets the value in the current memory cell
        /// </summary>
        void ResetCell();
    }
}
