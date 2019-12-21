using System.Collections.Generic;
using Brainf_ckSharp.Legacy.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer
{
    /// <summary>
    /// A class with the info on the current console functions table
    /// </summary>
    public sealed class MemoryViewerFunctionsSectionData : MemoryViewerSectionBase
    {
        /// <summary>
        /// Gets the defined functions in the current instance
        /// </summary>
        [NotNull]
        public IReadOnlyList<IndexedModelWithValue<FunctionDefinition>> Functions { get; }

        /// <summary>
        /// Creates a new instance for the functions table section
        /// </summary>
        /// <param name="functions">The current functions info</param>
        public MemoryViewerFunctionsSectionData([NotNull] IReadOnlyList<IndexedModelWithValue<FunctionDefinition>> functions)
            : base(ConsoleMemoryViewerSection.FunctionsList)
        {
            Functions = functions;
        }
    }
}
