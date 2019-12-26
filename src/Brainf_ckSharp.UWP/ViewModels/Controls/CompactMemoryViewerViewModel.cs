using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.UWP.Messages.Console.MemoryState;
using Brainf_ckSharp.UWP.Models.Console.Controls;
using Brainf_ckSharp.UWP.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;

#nullable enable

namespace Brainf_ckSharp.UWP.ViewModels.Controls
{
    /// <summary>
    /// A view model for a compact memory viewer for the interactive REPL console
    /// </summary>
    public sealed class CompactMemoryViewerViewModel : ItemsCollectionViewModelBase<Brainf_ckMemoryCellChunk>
    {
        /// <summary>
        /// Creates a new <see cref="CompactMemoryViewerViewModel"/> instance
        /// </summary>
        public CompactMemoryViewerViewModel()
        {
            MachineState = Messenger.Default.Request<MemoryStateRequestMessage, IReadOnlyTuringMachineState>();

            Messenger.Default.Register<MemoryStateChangedNotificationMessage>(this, m => MachineState = m.Value);
        }

        private IReadOnlyTuringMachineState? _MachineState;

        /// <summary>
        /// Gets or sets the <see cref="IReadOnlyTuringMachineState"/> instance for the current view model
        /// </summary>
        public IReadOnlyTuringMachineState? MachineState
        {
            get => _MachineState;
            set
            {
                if (ReferenceEquals(_MachineState, value)) return;

                _MachineState = value;
                RaisePropertyChanged();

                UpdateFromState(value);
            }
        }

        /// <summary>
        /// Updates the current model from the input machine state
        /// </summary>
        /// <param name="state">The input <see cref="IReadOnlyTuringMachineState"/> instance to read data from</param>
        public void UpdateFromState(IReadOnlyTuringMachineState? state)
        {
            if (state == null) Source.Clear();
            else
            {
                int chunks = state.Count / 4;

                if (Source.Count == chunks)
                {
                    // Update the existing models
                    foreach (Brainf_ckMemoryCellChunk chunk in Source)
                        chunk.UpdateFromState(state);
                }
                else
                {
                    Source.Clear();

                    // Populate the source collection from scratch
                    for (int i = 0; i < state.Count; i += 4)
                    {
                        Source.Add(new Brainf_ckMemoryCellChunk(state, i));
                    }
                }
                
            }
        }
    }
}
