using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Memory.Tools;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Uwp.Messages.Console.Commands;
using Brainf_ckSharp.Uwp.Messages.Console.MemoryState;
using Brainf_ckSharp.Uwp.Messages.InputPanel;
using Brainf_ckSharp.Uwp.Models.Console;
using Brainf_ckSharp.Uwp.Models.Console.Interfaces;
using Brainf_ckSharp.Uwp.ViewModels.Abstract.Collections;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ckSharp.Uwp.ViewModels.Views
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

            /* This message is never unsubscribed, for two reasons:
             * - It's only received from this view model, so there's no risk of conflicts
             * - It is first received before the OnActivate method is called, so
             *   registering it from there would cause a startup crash. */
            Messenger.Default.Register<MemoryStateRequestMessage>(this, m => m.ReportResult(MachineState));
        }

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Messenger.Default.Register<CharacterReceivedNotificationMessage>(this, m => _ = TryAddOperatorAsync(m));
            Messenger.Default.Register<OperatorKeyPressedNotificationMessage>(this, m => _ = TryAddOperatorAsync(m));
            Messenger.Default.Register<RunCommandRequestMessage>(this, m => _ = ExecuteCommandAsync());
            Messenger.Default.Register<DeleteOperatorRequestMessage>(this, m => _ = DeleteLastOperatorAsync());
            Messenger.Default.Register<ClearCommandRequestMessage>(this, m => _ = ResetCommandAsync());
            Messenger.Default.Register<RestartConsoleRequestMessage>(this, m => _ = RestartAsync());
            Messenger.Default.Register<ClearConsoleScreenRequestMessage>(this, m => _ = ClearScreenAsync());
            Messenger.Default.Register<RepeatCommandRequestMessage>(this, m => _ = RepeatLastScriptAsync());
        }

        private IReadOnlyMachineState _MachineState = MachineStateProvider.Default;

        /// <summary>
        /// Gets the <see cref="IReadOnlyMachineState"/> instance currently in use
        /// </summary>
        public IReadOnlyMachineState MachineState
        {
            get => _MachineState;
            set
            {
                _MachineState = value;

                Messenger.Default.Send(new MemoryStateChangedNotificationMessage(value));
            }
        }

        /// <summary>
        /// Adds a new operator to the last command in the console
        /// </summary>
        /// <param name="c">The current operator to add</param>
        public async Task TryAddOperatorAsync(char c)
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (!Brainf_ckParser.IsOperator(c)) return; 
                if (Source.LastOrDefault() is ConsoleCommand command)
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
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    command.IsActive = false;
                }
                else throw new InvalidOperationException("Missing console command to modify");

                Source.Add(new ConsoleRestart());
                MachineState = MachineStateProvider.Default;
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

                await ExecuteCommandAsync(command.Command);
            }
        }

        /// <summary>
        /// Executes a given console command
        /// </summary>
        /// <param name="command">The source code of the command to execute</param>
        private async Task ExecuteCommandAsync(string command)
        {
            /* Handle the various possible commands:
             *   Empty command: skip the execution and add a new line
             *   Syntax errors and exceptions: each has its own template
             *   Runs with no output: just add a new line
             *   Runs with output: add the output line, then a new command line */
            if (!string.IsNullOrEmpty(command))
            {
                string stdin = Messenger.Default.Request<StdinRequestMessage, string>();
                Option<InterpreterResult> result = await Task.Run(() =>
                {
                    return Brainf_ckInterpreter
                        .CreateReleaseConfiguration()
                        .WithSource(command)
                        .WithStdin(stdin)
                        .WithInitialState(MachineState)
                        .TryRun();
                });

                if (!result.ValidationResult.IsSuccess) Source.Add(new ConsoleSyntaxError(result.ValidationResult));
                else
                {
                    // In all cases, update the current memory state
                    MachineState = result.Value.MachineState;

                    // Display textual results and exit codes
                    if (!string.IsNullOrEmpty(result.Value.Stdout)) Source.Add(new ConsoleResult(result.Value.Stdout));
                    if (!result.Value.ExitCode.HasFlag(ExitCode.Success)) Source.Add(new ConsoleException(result.Value.ExitCode));
                }
            }

            Source.Add(new ConsoleCommand());
        }

        /// <summary>
        /// Repeats the last executed script
        /// </summary>
        public async Task RepeatLastScriptAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.Reverse().OfType<ConsoleCommand>().Skip(1).FirstOrDefault() is ConsoleCommand previous)
                {
                    if (!(Source.LastOrDefault() is ConsoleCommand current)) throw new InvalidOperationException("Missing console command to run");

                    current.IsActive = false;
                    current.Command = previous.Command;
                    await ExecuteCommandAsync(previous.Command);
                }
            }
        }
    }
}
