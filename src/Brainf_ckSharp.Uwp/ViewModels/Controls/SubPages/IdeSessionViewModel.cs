using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Uwp.Enums;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
using GalaSoft.MvvmLight.Command;

#nullable enable

namespace Brainf_ckSharp.Uwp.ViewModels.Controls.SubPages
{
    public sealed class IdeSessionViewModel : GroupedItemsCollectionViewModelBase<InterpreterSessionSection, InterpreterResult>
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
            if (!result.ExitCode.HasFlag(ExitCode.Success)) Source.Add(InterpreterSessionSection.ExceptionType, result);
            if (result.Stdout.Length > 0) Source.Add(InterpreterSessionSection.Stdout, result);
            
            if (result.ExitCode.HasFlag(ExitCode.ExceptionThrown)) Source.Add(InterpreterSessionSection.ErrorLocation, result);
            else if (result.ExitCode.HasFlag(ExitCode.BreakpointReached)) Source.Add(InterpreterSessionSection.BreakpointReached, result);

            if (result.ExitCode.HasFlag(ExitCode.ExceptionThrown) ||
                result.ExitCode.HasFlag(ExitCode.ThresholdExceeded) ||
                result.ExitCode.HasFlag(ExitCode.BreakpointReached))
            {
                Source.Add(InterpreterSessionSection.StackTrace, result);
            }

            Source.Add(InterpreterSessionSection.SourceCode, result);

            if (result.Functions.Count > 0) Source.Add(InterpreterSessionSection.FunctionDefinitions, result);

            Source.Add(InterpreterSessionSection.MemoryState, result);

            Source.Add(InterpreterSessionSection.Statistics, result);
        }
    }
}
