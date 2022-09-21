using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brainf_ckSharp.Memory.Interfaces;
using Microsoft.Toolkit.HighPerformance.Helpers;
using static System.Diagnostics.Debug;

namespace Brainf_ckSharp.Models;

/// <summary>
/// A model that represents the information on a given memory cell in a <see cref="IReadOnlyMachineState"/> object
/// </summary>
[DebuggerDisplay("({Index}: {Value}, {Character}, {IsSelected})")]
public readonly struct Brainf_ckMemoryCell : IEquatable<Brainf_ckMemoryCell>
{
    /// <summary>
    /// The state for the cell, which combines the current index with a flag that
    /// indicates whether the current cell is selected. Since the maximum possible
    /// value for the index is <see cref="short.MaxValue"/>, we can track the
    /// selection in the 15th bit, and use the previous ones to store the index.
    /// </summary>
    private readonly ushort State;

    /// <summary>
    /// Creates a new instance with the given value
    /// </summary>
    /// <param name="index">The index for the memory cell</param>
    /// <param name="value">The value for the memory cell</param>
    /// <param name="isSelected">Gets whether the cell is selected</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Brainf_ckMemoryCell(int index, ushort value, bool isSelected)
    {
        Assert((uint)index <= short.MaxValue);

        State = (ushort)BitHelper.SetFlag((uint)index, 15, isSelected);
        Value = value;
    }

    /// <summary>
    /// Gets the numerical index for the current memory cell
    /// </summary>
    public int Index
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)BitHelper.SetFlag(State, 15, false);
    }

    /// <summary>
    /// Gets the value of the current cell
    /// </summary>
    public ushort Value { get; }

    /// <summary>
    /// Gets the corresponding character for the current cell
    /// </summary>
    public char Character
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (char)Value;
    }

    /// <summary>
    /// Gets whether or not the cell is currently selected
    /// </summary>
    public bool IsSelected
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => BitHelper.HasFlag(State, 15);
    }

    /// <summary>
    /// Checks whether or not two <see cref="Brainf_ckMemoryCell"/> instances are equal
    /// </summary>
    /// <param name="a">The first <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
    /// <param name="b">The second <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
    /// <returns><see langword="true"/> if the two input <see cref="Brainf_ckMemoryCell"/> are equal, <see langword="false"/> otherwise</returns>
    public static bool operator ==(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => a.Equals(b);

    /// <summary>
    /// Checks whether or not two <see cref="Brainf_ckMemoryCell"/> instances are not equal
    /// </summary>
    /// <param name="a">The first <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
    /// <param name="b">The second <see cref="Brainf_ckMemoryCell"/> instance to compare</param>
    /// <returns><see langword="true"/> if the two input <see cref="Brainf_ckMemoryCell"/> are not equal, <see langword="false"/> otherwise</returns>
    public static bool operator !=(Brainf_ckMemoryCell a, Brainf_ckMemoryCell b) => !a.Equals(b);

    /// <inheritdoc/>
    public bool Equals(Brainf_ckMemoryCell other)
    {
        return
            State == other.State &&
            Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return
            obj is Brainf_ckMemoryCell cell &&
            Equals(cell);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(State, Value);
    }
}