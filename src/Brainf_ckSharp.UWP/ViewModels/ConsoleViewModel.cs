using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Tools;
using Brainf_ckSharp.UWP.Models.Console;
using Brainf_ckSharp.UWP.Models.Console.Interfaces;
using Brainf_ckSharp.UWP.ViewModels.Abstract;

namespace Brainf_ckSharp.UWP.ViewModels
{
    /// <summary>
    /// A view model for an interactive REPL console for Brainf*ck/PBrain
    /// </summary>
    public sealed class ConsoleViewModel : ItemsCollectionViewModelBase<IConsoleEntry>
    {
        /// <summary>
        /// An <see cref="AsyncMutex"/> instance to synchronize accesses to the console results
        /// </summary>
        private readonly AsyncMutex ExecutionMutex = new AsyncMutex();

        /// <summary>
        /// Creates a new <see cref="ConsoleViewModel"/> instances with a new command ready to use
        /// </summary>
        public ConsoleViewModel()
        {
            Source.Add(new ConsoleCommand());
        }

        /// <summary>
        /// Gets the <see cref="IReadOnlyTuringMachineState"/> instance currently in use
        /// </summary>
        public IReadOnlyTuringMachineState MachineState { get; private set; } = TuringMachineStateProvider.Default;

        /// <summary>
        /// Adds a new operator to the last command in the console
        /// </summary>
        /// <param name="c">The current operator to add</param>
        public async Task AddOperatorAsync(char c)
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Brainf_ckParser.IsOperator(c) &&
                    Source.LastOrDefault() is ConsoleCommand command)
                {
                    command.Command += c;
                }
                else throw new InvalidOperationException("Missing console command to modify");
            }
        }

        /// <summary>
        /// Deletes the last operator in the last command in the console
        /// </summary>
        public async Task DeleteLastOperatorAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    string current = command.Command;
                    command.Command = current.Substring(0, Math.Max(current.Length - 1, 0));
                }
                else throw new InvalidOperationException("Missing console command to modify");
            }
        }

        /// <summary>
        /// Resets the currently active console command
        /// </summary>
        public async Task ResetCommandAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    command.Command = string.Empty;
                }
                else throw new InvalidOperationException("Missing console command to modify");
            }
        }

        /// <summary>
        /// Restarts the current console memory state
        /// </summary>
        public async Task RestartAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                Source.Add(new ConsoleRestart());
                MachineState = TuringMachineStateProvider.Default;
                Source.Add(new ConsoleCommand());
            }
        }

        /// <summary>
        /// Clears the current console output
        /// </summary>
        public async Task ClearScreenAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                TryClearSource();
                Source.Add(new ConsoleCommand());
            }
        }

        /// <summary>
        /// Executes the current console command
        /// </summary>
        public async Task ExecuteCommandAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (!(Source.LastOrDefault() is ConsoleCommand command)) throw new InvalidOperationException("Missing console command to run");

                command.IsActive = false;

                /* Handle the various possible commands:
                 *   Empty command: skip the execution and add a new line
                 *   Syntax errors and exceptions: each has its own template
                 *   Runs with no output: just add a new line
                 *   Runs with output: add the output line, then a new command line */
                if (!string.IsNullOrEmpty(command.Command))
                {
                    Option<InterpreterResult> result = await Task.Run(() => Brainf_ckInterpreter.TryRun(command.Command));

                    if (!result.ValidationResult.IsSuccess) Source.Add(new ConsoleSyntaxError(result.ValidationResult));
                    else
                    {
                        // In all cases, update the current memory state
                        MachineState = result.Value.MachineState;
                        if (!string.IsNullOrEmpty(result.Value.Stdout)) Source.Add(new ConsoleResult(result.Value.Stdout));
                        if (!result.Value.ExitCode.HasFlag(ExitCode.Success)) Source.Add(new ConsoleException(result.Value.ExitCode));
                    }
                }

                Source.Add(new ConsoleCommand());
            }
        }
    }
}
