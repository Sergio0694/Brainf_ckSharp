using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Uwp.Enums;
using Brainf_ckSharp.Uwp.Models.Ide.Views;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
using GalaSoft.MvvmLight.Command;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class IdeSessionViewModel : GroupedItemsCollectionViewModelBase<IdeResultSection, IdeResultWithSectionInfo>
    {
        /// <summary>
        /// Creates a new <see cref="IdeSessionViewModel"/> instance
        /// </summary>
        public IdeSessionViewModel()
        {
            LoadDataCommand = new RelayCommand(() => _ = LoadDataAsync());
        }

        /// <summary>
        /// Gets or sets the script to execute
        /// </summary>
        public string? Script { get; set; }

        /// <summary>
        /// Gets or sets the stdin buffer to use
        /// </summary>
        public string? Stdin { get; set; }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance responsible for loading the available source codes
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// Loads the currently available code samples and recently used files
        /// </summary>
        private async Task LoadDataAsync()
        {
            Guard.MustBeNotNull(Source, nameof(Source));
            Guard.MustBeNotNull(Stdin, nameof(Stdin));

            // Run the code on a background thread
            InterpreterResult result = await Task.Run(() =>
            {
                using InterpreterSession session = Brainf_ckInterpreter
                    .CreateDebugConfiguration()
                    .WithSource(Script!)
                    .WithStdin(Stdin!)
                    .TryRun()
                    .Value!;

                session.MoveNext();

                return session.Current;
            });

            Source.Clear();

            // A function used to quickly add a specific section to the current collection
            void AddToSource(IdeResultSection section)
            {
                var model = new IdeResultWithSectionInfo(section, result);

                Source.Add(section, model);
            }

            /* The order of items in the result view is as follows:
             * - (optional) Exception type
             * - (optional) Stdout buffer
             * - (optional) Error location
             * - (optional) Breakpoint location
             * - (optional) Stack trace
             * - Source code
             * - (optional) Function definitions
             * - Memory state
             * - Statistics
             *
             * Each group stores the type of section it represents, so that
             * a template selector can be used in the view. The value of each
             * group is the the whole session result, as it contains all the
             * available info for the current script execution.
             * Each template is responsible for extracting info from it
             * and display according to its own function and section type. */
            if (!result.ExitCode.HasFlag(ExitCode.Success)) AddToSource(IdeResultSection.ExceptionType);
            if (result.Stdout.Length > 0) AddToSource(IdeResultSection.Stdout);
            
            if (result.ExitCode.HasFlag(ExitCode.ExceptionThrown)) AddToSource(IdeResultSection.ErrorLocation);
            else if (result.ExitCode.HasFlag(ExitCode.BreakpointReached)) AddToSource(IdeResultSection.BreakpointReached);

            if (result.ExitCode.HasFlag(ExitCode.ExceptionThrown) ||
                result.ExitCode.HasFlag(ExitCode.ThresholdExceeded) ||
                result.ExitCode.HasFlag(ExitCode.BreakpointReached))
            {
                AddToSource(IdeResultSection.StackTrace);
            }

            AddToSource(IdeResultSection.SourceCode);

            if (result.Functions.Count > 0) AddToSource(IdeResultSection.FunctionDefinitions);

            AddToSource(IdeResultSection.MemoryState);

            AddToSource(IdeResultSection.Statistics);
        }
    }
}
