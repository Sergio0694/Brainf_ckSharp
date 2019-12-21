using System.Collections.Generic;
using Brainf_ckSharp.Legacy.MemoryState;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer
{
    /// <summary>
    /// A class with the info on the current console memory state
    /// </summary>
    public sealed class MemoryViewerMemoryCellsSectionData : MemoryViewerSectionBase
    {
        /// <summary>
        /// Gets the memory cells in the current instance
        /// </summary>
        [NotNull]
        public IReadOnlyList<IndexedModelWithValue<Brainf_ckMemoryCell>> State { get; }

        /// <summary>
        /// Creates a new instance for the memory state section
        /// </summary>
        /// <param name="state">The current memory info</param>
        public MemoryViewerMemoryCellsSectionData([NotNull] IReadOnlyList<IndexedModelWithValue<Brainf_ckMemoryCell>> state)
            : base(ConsoleMemoryViewerSection.MemoryCells)
        {
            State = state;
        }
    }
}
