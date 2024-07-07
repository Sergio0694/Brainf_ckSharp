namespace Brainf_ckSharp.Memory.Interfaces;

/// <summary>
/// An <see langword="interface"/> that is used by mode-specific execution contexts
/// </summary>
internal interface IMachineStateExecutionContext
{
    /// <summary>
    /// Gets the current position on the memory buffer
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Gets the value at the current memory position
    /// </summary>
    ushort CurrentValue { get; }

    /// <summary>
    /// Gets the character at the current memory position
    /// </summary>
    char CurrentCharacter { get; }

    /// <summary>
    /// Gets whether the current value is a positive number
    /// </summary>
    bool IsCurrentValuePositive { get; }

    /// <summary>
    /// Tries to move the memory pointer forward
    /// </summary>
    /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>;
    bool TryMoveNext();

    /// <summary>
    /// Tries to move the memory pointer forward
    /// </summary>
    /// <param name="count">The number of times to try to execute the operation</param>
    /// <param name="totalOperations">The total number of executed operators</param>
    /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
    /// <remarks>The parameter <paramref name="totalOperations"/> is incremented by the number of successful operations executed</remarks>
    bool TryMoveNext(int count, ref int totalOperations);

    /// <summary>
    /// Tries to move the memory pointer back
    /// </summary>
    /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
    bool TryMoveBack();

    /// <summary>
    /// Tries to move the memory pointer back
    /// </summary>
    /// <param name="count">The number of times to try to execute the operation</param>
    /// <param name="totalOperations">The total number of executed operators</param>
    /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
    /// <remarks>The parameter <paramref name="totalOperations"/> is incremented by the number of successful operations executed</remarks>
    bool TryMoveBack(int count, ref int totalOperations);

    /// <summary>
    /// Tries to increment the current memory location
    /// </summary>
    /// <returns><see langword="true"/> if the memory location was incremented successfully, <see langword="false"/> otherwise</returns>
    bool TryIncrement();

    /// <summary>
    /// Tries to increment the current memory location
    /// </summary>
    /// <param name="count">The number of times to try to execute the operation</param>
    /// <param name="totalOperations">The total number of executed operators</param>
    /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
    /// <remarks>The parameter <paramref name="totalOperations"/> is incremented by the number of successful operations executed</remarks>
    bool TryIncrement(int count, ref int totalOperations);

    /// <summary>
    /// Tries to decrement the current memory location
    /// </summary>
    /// <returns><see langword="true"/> if the memory location was decremented successfully, <see langword="false"/> otherwise</returns>
    bool TryDecrement();

    /// <summary>
    /// Tries to decrement the current memory location
    /// </summary>
    /// <param name="count">The number of times to try to execute the operation</param>
    /// <param name="totalOperations">The total number of executed operators</param>
    /// <returns><see langword="true"/> if the pointer was moved successfully, <see langword="false"/> otherwise</returns>
    /// <remarks>The parameter <paramref name="totalOperations"/> is incremented by the number of successful operations executed</remarks>
    bool TryDecrement(int count, ref int totalOperations);

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
