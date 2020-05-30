using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Memory.Tools;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Messages.Console.Commands;
using Brainf_ckSharp.Shared.Messages.Console.MemoryState;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Models.Console;
using Brainf_ckSharp.Shared.Models.Console.Interfaces;
using Brainf_ckSharp.Shared.ViewModels.Views.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Nito.AsyncEx;

namespace Brainf_ckSharp.Shared.ViewModels.Views
{
    /// <summary>
    /// A view model for an interactive REPL console for Brainf*ck/PBrain
    /// </summary>
    public sealed class ConsoleViewModel : WorkspaceViewModelBase
    {
        /// <summary>
        /// An <see cref="AsyncLock"/> instance to synchronize accesses to the console results
        /// </summary>
        private readonly AsyncLock ExecutionMutex = new AsyncLock();

        /// <summary>
        /// Creates a new <see cref="ConsoleViewModel"/> instance with a new command ready to use
        /// </summary>
        public ConsoleViewModel()
        {
            // The row is always set to 1, as commands are one line each
            Row = 1;

            // This does not apply to this workspace view model
            IsUnsavedEditPending = false;

            Source = new ObservableCollection<IConsoleEntry> { new ConsoleCommand() };

            // This message is never unsubscribed, for two reasons:
            // - It's only received from this view model, so there's no risk of conflicts
            // - It is first received before the OnActivate method is called, so
            //   registering it from there would cause a startup crash.
            Messenger.Register<MemoryStateRequestMessage>(this, m => m.ReportResult(MachineState));
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            Ioc.Default.GetRequiredService<IKeyboardListenerService>().CharacterReceived += TryAddOperator;

            Messenger.Register<OperatorKeyPressedNotificationMessage>(this, m => _ = TryAddOperatorAsync(m.Value));
            Messenger.Register<RunCommandRequestMessage>(this, m => _ = ExecuteCommandAsync());
            Messenger.Register<DeleteOperatorRequestMessage>(this, m => _ = DeleteLastOperatorAsync());
            Messenger.Register<ClearCommandRequestMessage>(this, m => _ = ResetCommandAsync());
            Messenger.Register<RestartConsoleRequestMessage>(this, m => _ = RestartAsync());
            Messenger.Register<ClearConsoleScreenRequestMessage>(this, m => _ = ClearScreenAsync());
            Messenger.Register<RepeatCommandRequestMessage>(this, m => _ = RepeatLastScriptAsync());
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            Ioc.Default.GetRequiredService<IKeyboardListenerService>().CharacterReceived -= TryAddOperator;

            base.OnDeactivated();
        }

        /// <summary>
        /// Gets the collection of currently visible console lines
        /// </summary>
        public ObservableCollection<IConsoleEntry> Source { get; }

        private IReadOnlyMachineState _MachineState = MachineStateProvider.Default;

        /// <summary>
        /// Gets the <see cref="IReadOnlyMachineState"/> instance currently in use
        /// </summary>
        public IReadOnlyMachineState MachineState
        {
            get => _MachineState;
            private set => Set(ref _MachineState, value, true);
        }

        private string _Text = string.Empty;

        /// <summary>
        /// Gets the current command that can be executed
        /// </summary>
        public override string Text
        {
            get => _Text;
            set
            {
                if (Set(ref _Text, value))
                {
                    ValidationResult = Brainf_ckParser.ValidateSyntax(value);
                    Column = value.Length + 1;
                }
            }
        }

        /// <summary>
        /// Adds a new operator to the last command in the console
        /// </summary>
        /// <param name="c">The current operator to add</param>
        public void TryAddOperator(char c)
        {
            _ = TryAddOperatorAsync(c);
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

                    Text = command.Command;
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

                    Text = command.Command;
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

                    Text = string.Empty;
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

                Text = string.Empty;
            }
        }

        /// <summary>
        /// Clears the current console output
        /// </summary>
        public async Task ClearScreenAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                Source.Clear();

                Source.Add(new ConsoleCommand());

                Text = string.Empty;
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
            // Handle the various possible commands:
            // - Empty command: skip the execution and add a new line
            // - Syntax errors and exceptions: each has its own template
            // - Runs with no output: just add a new line
            // - Runs with output: add the output line, then a new command line
            if (!string.IsNullOrEmpty(command))
            {
                string stdin = Messenger.Request<StdinRequestMessage, string>();
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
                    MachineState = result.Value!.MachineState;

                    // Display textual results and exit codes
                    if (!string.IsNullOrEmpty(result.Value.Stdout)) Source.Add(new ConsoleResult(result.Value.Stdout));
                    if (!result.Value.ExitCode.HasFlag(ExitCode.Success)) Source.Add(new ConsoleException(result.Value.ExitCode));
                }
            }

            Source.Add(new ConsoleCommand());

            Text = string.Empty;
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
