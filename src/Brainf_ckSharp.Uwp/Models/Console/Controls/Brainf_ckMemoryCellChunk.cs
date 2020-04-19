using System;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Uwp.Models.Console.Controls
{
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

            _Zero = state[BaseOffset];
            _One = state[BaseOffset + 1];
            _Two = state[BaseOffset + 2];
            _Three = state[BaseOffset + 3];
        }

        private Brainf_ckMemoryCell _Zero;

        /// <summary>
        /// Gets the first memory cell
        /// </summary>
        public Brainf_ckMemoryCell Zero
        {
            get => _Zero;
            set => Set(ref _Zero, value);
        }

        private Brainf_ckMemoryCell _One;

        /// <summary>
        /// Gets the second memory cell
        /// </summary>
        public Brainf_ckMemoryCell One
        {
            get => _One;
            set => Set(ref _One, value);
        }

        private Brainf_ckMemoryCell _Two;

        /// <summary>
        /// Gets the third memory cell
        /// </summary>
        public Brainf_ckMemoryCell Two
        {
            get => _Two;
            set => Set(ref _Two, value);
        }

        private Brainf_ckMemoryCell _Three;

        /// <summary>
        /// Gets the fourth memory cell
        /// </summary>
        public Brainf_ckMemoryCell Three
        {
            get => _Three;
            set => Set(ref _Three, value);
        }

        /// <summary>
        /// Gets the offset of the first memory cell in the chunk with respect to the source memory state
        /// </summary>
        public int BaseOffset { get; }

        /// <summary>
        /// Gets whether or not the current position is within the current chunk
        /// </summary>
        public bool IsChunkSelected => _Zero.IsSelected ||
                                       _One.IsSelected ||
                                       _Two.IsSelected ||
                                       _Three.IsSelected;

        private int _SelectedIndex;

        /// <summary>
        /// Gets the index of the selected cell in the current chunk, if present
        /// </summary>
        /// <remarks>This property clamps the relative offset in the [0,3] range</remarks>
        public int SelectedIndex => Math.Clamp(_SelectedIndex, BaseOffset, BaseOffset + 4) - BaseOffset;

        /// <summary>
        /// Updates the current model from the input machine state
        /// </summary>
        /// <param name="state">The input <see cref="IReadOnlyMachineState"/> instance to read data from</param>
        public void UpdateFromState(IReadOnlyMachineState state)
        {
            if (state.Count < BaseOffset + 3) throw new ArgumentException($"The input state is too short: size of {state.Count} for an offset of {BaseOffset}", nameof(state));

            Zero = state[BaseOffset];
            One = state[BaseOffset + 1];
            Two = state[BaseOffset + 2];
            Three = state[BaseOffset + 3];

            _SelectedIndex = state.Position;

            OnPropertyChanged(nameof(IsChunkSelected));
            OnPropertyChanged(nameof(SelectedIndex));
        }
    }
}
