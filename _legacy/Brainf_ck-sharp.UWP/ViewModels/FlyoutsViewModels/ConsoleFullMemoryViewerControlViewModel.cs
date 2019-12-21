using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.ConsoleMemoryViewer;
using Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList;
using Brainf_ckSharp.Legacy.MemoryState;
using Brainf_ckSharp.Legacy.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class ConsoleFullMemoryViewerControlViewModel : DeferredJumpListViewModelBase<ConsoleMemoryViewerSection, MemoryViewerSectionBase>
    {
        // The current memory state
        [NotNull]
        private readonly IReadonlyTouringMachineState State;

        // The function definitions
        [NotNull]
        private readonly IReadOnlyList<FunctionDefinition> Functions;

        public ConsoleFullMemoryViewerControlViewModel([NotNull] IReadonlyTouringMachineState state, [NotNull] IReadOnlyList<FunctionDefinition> functions)
        {
            State = state;
            Functions = functions;
        }

        /// <summary>
        /// Initializes the current view model with a function that returns the first session to display to the user
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadGroupsAsync();
            InitializationCompleted?.Invoke(this, EventArgs.Empty);
        }

        // SemanticZoom data loading
        protected override Task<IList<JumpListGroup<ConsoleMemoryViewerSection, MemoryViewerSectionBase>>> OnLoadGroupsAsync()
        {
            return Task.Run(() =>
            {
                // Get an empty list to populate and prepare a helper function
                IList<JumpListGroup<ConsoleMemoryViewerSection, MemoryViewerSectionBase>> source = new List<JumpListGroup<ConsoleMemoryViewerSection, MemoryViewerSectionBase>>();
                
                // Functions
                IndexedModelWithValue<FunctionDefinition>[] functions = IndexedModelWithValue<FunctionDefinition>.New(Functions).ToArray();
                source.Add(new JumpListGroup<ConsoleMemoryViewerSection, MemoryViewerSectionBase>(
                    ConsoleMemoryViewerSection.FunctionsList, new[] { new MemoryViewerFunctionsSectionData(functions) }));

                // Calculate the memory state info and add it to the queue
                IndexedModelWithValue<Brainf_ckMemoryCell>[] state = IndexedModelWithValue<Brainf_ckMemoryCell>.New(State).ToArray();
                source.Add(new JumpListGroup<ConsoleMemoryViewerSection, MemoryViewerSectionBase>(
                    ConsoleMemoryViewerSection.MemoryCells, new[] { new MemoryViewerMemoryCellsSectionData(state) }));
                return source;
            });
        }

        /// <inheritdoc/>
        public override void Cleanup()
        {
            base.Cleanup();
            InitializationCompleted = null;
        }

        /// <summary>
        /// Raised when the initialization is completed and all the items to display have been loaded
        /// </summary>
        public event EventHandler InitializationCompleted;
    }
}
