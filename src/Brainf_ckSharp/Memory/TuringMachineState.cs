using System.Numerics;
using Brainf_ckSharp.Constants;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using CommunityToolkit.Diagnostics;

namespace Brainf_ckSharp.Memory;

/// <summary>
/// A <see langword="class"/> that provides the ability to create empty machine states for later use
/// </summary>
public static class TuringMachineState
{
    /// <summary>
    /// Gets the default machine state instance
    /// </summary>
    public static IReadOnlyMachineState Default { get; } = Create(Specs.DefaultMemorySize, Specs.DefaultDataType);

    /// <summary>
    /// Creates a new <see cref="IReadOnlyMachineState"/> instance with the specified parameters
    /// </summary>
    /// <param name="size">The size of the state machine to create</param>
    /// <returns>A new <see cref="IReadOnlyMachineState"/> instance with the specified parameters</returns>
    public static IReadOnlyMachineState Create(int size)
    {
        return Create(size, Specs.DefaultDataType);
    }

    /// <summary>
    /// Creates a new <see cref="IReadOnlyMachineState"/> instance with the specified parameters
    /// </summary>
    /// <param name="size">The size of the state machine to create</param>
    /// <param name="dataType">The data type to use in the state machine to create</param>
    /// <returns>A new <see cref="IReadOnlyMachineState"/> instance with the specified parameters</returns>
    public static IReadOnlyMachineState Create(int size, DataType dataType)
    {
        Guard.IsBetweenOrEqualTo(size, Specs.MinimumMemorySize, Specs.MaximumMemorySize);
        Guard.IsTrue(BitOperations.IsPow2(size), nameof(size), "The size must be a power of 2.");

        return dataType switch
        {
            DataType.Byte => new TuringMachineState<byte>(size),
            DataType.UnsignedShort => new TuringMachineState<ushort>(size),
            _ => ThrowHelper.ThrowArgumentOutOfRangeException<IReadOnlyMachineState>(nameof(dataType), "Invalid data type.")
        };
    }
}
