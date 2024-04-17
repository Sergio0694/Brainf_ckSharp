using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.Models.Console.Controls;

/// <summary>
/// A model that represents a group of 4 contiguous memory cells
/// </summary>
public sealed class Brainf_ckMemoryCellChunk : ObservableObject
{
    /// <summary>
    /// Creates a new <see cref="Brainf_ckMemoryCellChunk"/> instance with the specified parameters
    /// </summary>
    /// <param name="state">The input <see cref="IReadOnlyMachineState"/> instance to read data from</param>
    /// <param name="offset">The offset of the first memory cell in the chunk with respect to the source memory state</param>
    public Brainf_ckMemoryCellChunk(IReadOnlyMachineState state, int offset)
    {
        BaseOffset = offset;

        this._Zero = state[BaseOffset];
        this._One = state[BaseOffset + 1];
        this._Two = state[BaseOffset + 2];
        this._Three = state[BaseOffset + 3];

        this._SelectedIndex = state.Position;
    }

    private Brainf_ckMemoryCell _Zero;

    /// <summary>
    /// Gets the first memory cell
    /// </summary>
    public Brainf_ckMemoryCell Zero
    {
        get => this._Zero;
        set => SetProperty(ref this._Zero, value);
    }

    private Brainf_ckMemoryCell _One;

    /// <summary>
    /// Gets the second memory cell
    /// </summary>
    public Brainf_ckMemoryCell One
    {
        get => this._One;
        set => SetProperty(ref this._One, value);
    }

    private Brainf_ckMemoryCell _Two;

    /// <summary>
    /// Gets the third memory cell
    /// </summary>
    public Brainf_ckMemoryCell Two
    {
        get => this._Two;
        set => SetProperty(ref this._Two, value);
    }

    private Brainf_ckMemoryCell _Three;

    /// <summary>
    /// Gets the fourth memory cell
    /// </summary>
    public Brainf_ckMemoryCell Three
    {
        get => this._Three;
        set => SetProperty(ref this._Three, value);
    }

    /// <summary>
    /// Gets the offset of the first memory cell in the chunk with respect to the source memory state
    /// </summary>
    public int BaseOffset { get; }

    /// <summary>
    /// Gets whether or not the current position is within the current chunk
    /// </summary>
    public bool IsChunkSelected => this._Zero.IsSelected ||
                                   this._One.IsSelected ||
                                   this._Two.IsSelected ||
                                   this._Three.IsSelected;

    private int _SelectedIndex;

    /// <summary>
    /// Gets the index of the selected cell in the current chunk, if present
    /// </summary>
    /// <remarks>This property clamps the relative offset in the [0,3] range</remarks>
    public int SelectedIndex
    {
        get
        {
            int index = this._SelectedIndex - BaseOffset;

            if (index > 3) return 3;
            if (index < 0) return 0;

            return index;
        }
    }

    /// <summary>
    /// Updates the current model from the input machine state
    /// </summary>
    /// <param name="state">The input <see cref="IReadOnlyMachineState"/> instance to read data from</param>
    public void UpdateFromState(IReadOnlyMachineState state)
    {
        if (state.Count < BaseOffset + 3)
        {
            ThrowHelper.ThrowArgumentException(nameof(state), "The input state is too short for the current offset");
        }

        Zero = state[BaseOffset];
        One = state[BaseOffset + 1];
        Two = state[BaseOffset + 2];
        Three = state[BaseOffset + 3];

        this._SelectedIndex = state.Position;

        OnPropertyChanged(nameof(IsChunkSelected));
        OnPropertyChanged(nameof(SelectedIndex));
    }
}
