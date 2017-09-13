using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels.Abstract.JumpList;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels
{
    public class IDERunResultFlyoutViewModel : DeferredJumpListViewModelBase<IDEResultSection, IDEResultSectionDataBase>
    {
        /// <summary>
        /// Gets the current execution session
        /// </summary>
        private InterpreterExecutionSession Session { get; set; }

        /// <summary>
        /// Initializes the current view model with a function that returns the first session to display to the user
        /// </summary>
        /// <param name="factory">A function that returns the first execution session to show to the user</param>
        public async Task InitializeAsync([NotNull] Func<InterpreterExecutionSession> factory)
        {
            Session = await Task.Run(factory);
            RaiseBreakpointOptionsActiveStatusChanged(Session.CanContinue);
            await LoadGroupsAsync();
            InitializationCompleted?.Invoke(this, EventArgs.Empty);
        }

        /* ===================
         * NOTE
         * ===================
         * The APIs to manage a SemanticZoom control are quite messy and this code is needed
         * in order to setup the right data for the bindings.
         * Please don't judge me for this part, I know it's ugly :( */
        protected override Task<IList<JumpListGroup<IDEResultSection, IDEResultSectionDataBase>>> OnLoadGroupsAsync()
        {
            return Task.Run(() =>
            {
                // Get an empty list to populate and prepare a helper function
                IList<JumpListGroup<IDEResultSection, IDEResultSectionDataBase>> source = new List<JumpListGroup<IDEResultSection, IDEResultSectionDataBase>>();
                JumpListGroup<IDEResultSection, IDEResultSectionDataBase> GroupFromSection(IDEResultSection section)
                {
                    // The same session instance needs to be passed to every template to generate their data to display
                    return new JumpListGroup<IDEResultSection, IDEResultSectionDataBase>(section, new[] { new IDEResultSectionSessionData(section, Session) });
                }

                // Exception type (if present) and Stdout buffer (if it contains at least a character)
                if (!Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.Success))
                {
                    ScriptExceptionInfo info = ScriptExceptionInfo.FromResult(Session.CurrentResult);
                    source.Add(new JumpListGroup<IDEResultSection, IDEResultSectionDataBase>(
                        IDEResultSection.ExceptionType, new[] { new IDEResultExceptionInfoData(info) }));
                }
                if (Session.CurrentResult.Output.Length > 0) source.Add(GroupFromSection(IDEResultSection.Stdout));

                // Error location when needed
                if (Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown))
                {
                    source.Add(GroupFromSection(IDEResultSection.ErrorLocation));
                }
                else if (Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached))
                {
                    source.Add(GroupFromSection(IDEResultSection.BreakpointReached));
                }

                // Add the proper stack trace
                if (Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.ExceptionThrown) ||
                    Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.ThresholdExceeded) ||
                    Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.BreakpointReached))
                {
                    source.Add(GroupFromSection(IDEResultSection.StackTrace));
                }

                // Always show the source code
                source.Add(GroupFromSection(IDEResultSection.SourceCode));

                // Add the memory state and the statistics only if the code was executed
                if (!Session.CurrentResult.ExitCode.HasFlag(InterpreterExitCode.MismatchedParentheses))
                {
                    // Functions, if present
                    if (Session.CurrentResult.Functions.Count > 0)
                    {
                        IndexedModelWithValue<FunctionDefinition>[] functions = IndexedModelWithValue<FunctionDefinition>.New(Session.CurrentResult.Functions).ToArray();
                        source.Add(new JumpListGroup<IDEResultSection, IDEResultSectionDataBase>(
                            IDEResultSection.MemoryState, new[] { new IDEResultSectionFunctionsData(functions) }));
                    }

                    // Calculate the memory state info and add it to the queue
                    IndexedModelWithValue<Brainf_ckMemoryCell>[] state = IndexedModelWithValue<Brainf_ckMemoryCell>.New(Session.CurrentResult.MachineState).ToArray();
                    source.Add(new JumpListGroup<IDEResultSection, IDEResultSectionDataBase>(
                        IDEResultSection.MemoryState, new[] { new IDEResultSectionStateData(state) }));

                    // Add the statistics
                    source.Add(GroupFromSection(IDEResultSection.Stats));
                }
                return source;
            });
        }

        /// <inheritdoc/>
        public override void Cleanup()
        {
            Session.Dispose();
            Session = null;
            base.Cleanup();
            InitializationCompleted = null;
            LoadingStateChanged = null;
            BreakpointOptionsActiveStatusChanged = null;
        }

        /// <summary>
        /// Raised when the initialization is completed and all the items to display have been loaded
        /// </summary>
        public event EventHandler InitializationCompleted;

        /// <summary>
        /// Raised whenever the loading status changes for the control
        /// </summary>
        public event EventHandler<bool> LoadingStateChanged;

        /// <summary>
        /// Raised whenever the status for the breakpoint options changes
        /// </summary>
        public event EventHandler<bool> BreakpointOptionsActiveStatusChanged;

        // Field to keep track of the breakpoint options status
        private bool _BreakpointButtonsEnabled;

        /// <summary>
        /// Raises the <see cref="BreakpointOptionsActiveStatusChanged"/> event if needed
        /// </summary>
        /// <param name="state">The new status for the breakpoint options</param>
        private void RaiseBreakpointOptionsActiveStatusChanged(bool state)
        {
            if (_BreakpointButtonsEnabled != state)
            {
                _BreakpointButtonsEnabled = state;
                BreakpointOptionsActiveStatusChanged?.Invoke(this, state);
            }
        }

        // Continues a script from its current state
        private async void ManageDebugSessionAsync(bool runToCompletion)
        {
            LoadingStateChanged?.Invoke(this, true);
            await Task.Delay(500);
            await Task.Run(() =>
            {
                if (runToCompletion) Session.RunToCompletion();
                else Session.Continue();
            });
            await LoadGroupsAsync();
            await Task.Delay(500);
            LoadingStateChanged?.Invoke(this, false);
            RaiseBreakpointOptionsActiveStatusChanged(Session.CanContinue);
        }

        /// <summary>
        /// Continues the execution from the current breakpoint
        /// </summary>
        public void Continue() => ManageDebugSessionAsync(false);

        /// <summary>
        /// Runs the script until the end, skipping other breakpoints along the way, if present
        /// </summary>
        public void RunToCompletion() => ManageDebugSessionAsync(true);
    }
}
