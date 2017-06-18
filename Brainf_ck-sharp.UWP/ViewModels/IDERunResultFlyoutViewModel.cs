﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.IDEResults;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class IDERunResultFlyoutViewModel : JumpListViewModelBase<IDEResultSection, IDEResultSectionDataBase>
    {
        private InterpreterExecutionSession Session { get; set; }

        /// <summary>
        /// Gets whether or not the breakpoint management buttons are currently visible
        /// </summary>
        public bool BreakpointMode => Session.CanContinue;

        public IDERunResultFlyoutViewModel([NotNull] InterpreterExecutionSession session) => Session = session;

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
                if (!Session.CurrentResult.HasFlag(InterpreterExitCode.Success))
                {
                    ScriptExceptionInfo info = ScriptExceptionInfo.FromResult(Session.CurrentResult);
                    source.Add(new JumpListGroup<IDEResultSection, IDEResultSectionDataBase>(
                        IDEResultSection.ExceptionType, new[] { new IDEResultExceptionInfoData(info) }));
                }
                if (Session.CurrentResult.Output.Length > 0) source.Add(GroupFromSection(IDEResultSection.Stdout));

                // Error location when needed and stack trace, if present
                if (Session.CurrentResult.HasFlag(InterpreterExitCode.ExceptionThrown))
                {
                    source.Add(GroupFromSection(IDEResultSection.ErrorLocation));
                }
                else if (Session.CurrentResult.HasFlag(InterpreterExitCode.BreakpointReached))
                {
                    source.Add(GroupFromSection(IDEResultSection.BreakpointReached));
                }
                if (Session.CurrentResult.HasFlag(InterpreterExitCode.ExceptionThrown) ||
                    Session.CurrentResult.HasFlag(InterpreterExitCode.BreakpointReached))
                {
                    source.Add(GroupFromSection(IDEResultSection.StackTrace));
                }

                // Always show the source code
                source.Add(GroupFromSection(IDEResultSection.SourceCode));

                // Calculate the memory state info and add it to the queue
                IndexedModelWithValue<Brainf_ckMemoryCell>[] state = IndexedModelWithValue<Brainf_ckMemoryCell>.New(Session.CurrentResult.MachineState).ToArray();
                source.Add(new JumpListGroup<IDEResultSection, IDEResultSectionDataBase>(
                    IDEResultSection.MemoryState, new[] { new IDEResultSectionStateData(state) }));
                return source;
            });
        }

        /// <summary>
        /// Raised whenever the loading status changes for the control
        /// </summary>
        public event EventHandler<(bool Loading, bool IsIntermediate)> LoadingStateChanged; 

        // Continues a script from its current state
        private async void ManageDebugSessionAsync(bool runToCompletion)
        {
            LoadingStateChanged?.Invoke(this, (true, true));
            await Task.Delay(500);
            Session = await Task.Run(() => runToCompletion ? Session.RunToCompletion() : Session.Continue());
            await LoadGroupsAsync();
            await Task.Delay(500);
            LoadingStateChanged?.Invoke(this, (false, BreakpointMode));
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
