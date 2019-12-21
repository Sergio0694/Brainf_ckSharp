using System.Collections.Generic;
using Brainf_ckSharp.Legacy.MemoryState;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.IDEResults
{
    /// <summary>
    /// A model that stores info on the memory state for a script run in the IDE
    /// </summary>
    public class IDEResultSectionStateData : IDEResultSectionDataBase
    {
        /// <summary>
        /// Gets the indexed memory state info for the script
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<IndexedModelWithValue<Brainf_ckMemoryCell>> IndexedState { get; }
        
        /// <summary>
        /// Creates a new instance for the given memory state
        /// </summary>
        /// <param name="state">The indexed memory state to expose</param>
        public IDEResultSectionStateData([NotNull] IReadOnlyCollection<IndexedModelWithValue<Brainf_ckMemoryCell>> state)
            : base(IDEResultSection.MemoryState)
        {
            IndexedState = state;
        }
    }
}
