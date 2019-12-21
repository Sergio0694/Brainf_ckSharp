using System.Collections.Generic;
using Brainf_ckSharp.Legacy.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.IDEResults
{
    /// <summary>
    /// A model that stores info on the function definitions for a script run in the IDE
    /// </summary>
    public class IDEResultSectionFunctionsData : IDEResultSectionDataBase
    {
        /// <summary>
        /// Gets the indexed function definitions for the script
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<IndexedModelWithValue<FunctionDefinition>> IndexedDefinitions { get; }

        /// <summary>
        /// Creates a new instance for a list of function definitions
        /// </summary>
        /// <param name="definitions">The indexed definitions to expose</param>
        public IDEResultSectionFunctionsData([NotNull] IReadOnlyCollection<IndexedModelWithValue<FunctionDefinition>> definitions)
            : base(IDEResultSection.FunctionDefinitions)
        {
            IndexedDefinitions = definitions;
        }
    }
}
