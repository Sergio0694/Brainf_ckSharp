using System.Collections.ObjectModel;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Shared.Messages.Console.MemoryState;
using Brainf_ckSharp.Shared.Models.Console.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

#pragma warning disable IDE0290

namespace Brainf_ckSharp.Shared.ViewModels.Controls;

/// <summary>
/// A view model for a compact memory viewer for the interactive REPL console
/// </summary>
public sealed partial class CompactMemoryViewerViewModel : ObservableRecipient
{
    /// <summary>
    /// Creates a new <see cref="CompactMemoryViewerViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    public CompactMemoryViewerViewModel(IMessenger messenger) : base(messenger)
    {
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        Messenger.Register<CompactMemoryViewerViewModel, PropertyChangedMessage<IReadOnlyMachineState>>(this, (r, m) => r.MachineState = m.NewValue);

        MachineState = Messenger.Send<MemoryStateRequestMessage>().Response;
    }

    /// <summary>
    /// Gets the current collection of <see cref="Brainf_ckMemoryCellChunk"/> instances
    /// </summary>
    public ObservableCollection<Brainf_ckMemoryCellChunk> Source { get; } = [];

    /// <summary>
    /// Gets or sets the <see cref="IReadOnlyMachineState"/> instance for the current view model
    /// </summary>
    [ObservableProperty]
    private IReadOnlyMachineState? machineState;

    /// <inheritdoc/>
    partial void OnMachineStateChanged(IReadOnlyMachineState? state)
    {
        if (state == null)
        {
            Source.Clear();
        }
        else
        {
            int chunks = state.Count / 4;

            if (Source.Count == chunks)
            {
                // Update the existing models
                foreach (Brainf_ckMemoryCellChunk chunk in Source)
                {
                    chunk.UpdateFromState(state);
                }
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
