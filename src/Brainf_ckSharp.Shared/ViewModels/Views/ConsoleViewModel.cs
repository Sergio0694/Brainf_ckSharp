using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Memory.Tools;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.Console.Commands;
using Brainf_ckSharp.Shared.Messages.Console.MemoryState;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.Models.Console;
using Brainf_ckSharp.Shared.Models.Console.Interfaces;
using Brainf_ckSharp.Shared.ViewModels.Views.Abstract;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Messaging;
using Nito.AsyncEx;

#nullable enable

namespace Brainf_ckSharp.Shared.ViewModels.Views
{
    /// <summary>
    /// A view model for an interactive REPL console for Brainf*ck/PBrain
    /// </summary>
    public sealed class ConsoleViewModel : WorkspaceViewModelBase,
        IRecipient<MemoryStateRequestMessage>,
        IRecipient<RunCommandRequestMessage>,
        IRecipient<DeleteOperatorRequestMessage>,
        IRecipient<ClearCommandRequestMessage>,
        IRecipient<RestartConsoleRequestMessage>,
        IRecipient<ClearConsoleScreenRequestMessage>,
        IRecipient<RepeatCommandRequestMessage>,
        IRecipient<OverflowModeSettingChangedMessage>,
        IRecipient<MemorySizeSettingChangedMessage>,
        IRecipient<OperatorKeyPressedNotificationMessage>
    {
        /// <summary>
        /// The <see cref="ISettingsService"/> instance currently in use
        /// </summary>
        private readonly ISettingsService SettingsService;

        /// <summary>
        /// The <see cref="IKeyboardListenerService"/> instance currently in use
        /// </summary>
        private readonly IKeyboardListenerService KeyboardListenerService;

        /// <summary>
        /// An <see cref="AsyncLock"/> instance to synchronize accesses to the console results
        /// </summary>
        private readonly AsyncLock ExecutionMutex = new AsyncLock();

        /// <summary>
        /// Creates a new <see cref="ConsoleViewModel"/> instance with a new command ready to use
        /// </summary>
        /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
        /// <param name="keyboardListenerService">The <see cref="IKeyboardListenerService"/> instance to use</param>
        public ConsoleViewModel(ISettingsService settingsService, IKeyboardListenerService keyboardListenerService)
        {
            SettingsService = settingsService;
            KeyboardListenerService = keyboardListenerService;

            // Initialize the machine state with the current user settings
            _MachineState = MachineStateProvider.Create(
                SettingsService.GetValue<int>(SettingsKeys.MemorySize),
                SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode));
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            KeyboardListenerService.CharacterReceived += TryAddOperator;
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            KeyboardListenerService.CharacterReceived -= TryAddOperator;
        }

        /// <inheritdoc/>
        protected override void OnTextChanged(ReadOnlyMemory<char> text)
        {
            ValidationResult = Brainf_ckParser.ValidateSyntax(text.Span);
            Column = text.Length + 1;
        }

        /// <summary>
        /// Gets the collection of currently visible console lines
        /// </summary>
        public ObservableCollection<IConsoleEntry> Source { get; } = new ObservableCollection<IConsoleEntry> { new ConsoleCommand() };

        private IReadOnlyMachineState _MachineState;

        /// <summary>
        /// Gets the <see cref="IReadOnlyMachineState"/> instance currently in use
        /// </summary>
        public IReadOnlyMachineState MachineState
        {
            get => _MachineState;
            private set => SetProperty(ref _MachineState, value, true);
        }

        /// <summary>
        /// Adds a new operator to the last command in the console
        /// </summary>
        /// <param name="c">The current operator to add</param>
        private void TryAddOperator(char c)
        {
            _ = TryAddOperatorAsync(c);
        }

        /// <summary>
        /// Adds a new operator to the last command in the console
        /// </summary>
        /// <param name="c">The current operator to add</param>
        private async Task TryAddOperatorAsync(char c)
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (!Brainf_ckParser.IsOperator(c)) return; 
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    command.Command += c;

                    Text = command.Command.AsMemory();
                }
                else ThrowHelper.ThrowInvalidOperationException("Missing console command to modify");
            }
        }

        /// <summary>
        /// Deletes the last operator in the last command in the console
        /// </summary>
        private async Task DeleteLastOperatorAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    string current = command.Command;

                    command.Command = current.Substring(0, Math.Max(current.Length - 1, 0));

                    Text = command.Command.AsMemory();
                }
                else ThrowHelper.ThrowInvalidOperationException("Missing console command to modify");
            }
        }

        /// <summary>
        /// Resets the currently active console command
        /// </summary>
        private async Task ResetCommandAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    command.Command = string.Empty;

                    Text = Memory<char>.Empty;
                }
                else ThrowHelper.ThrowInvalidOperationException("Missing console command to modify");
            }
        }

        /// <summary>
        /// Restarts the current console memory state
        /// </summary>
        private async Task RestartAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.LastOrDefault() is ConsoleCommand command)
                {
                    command.IsActive = false;
                }
                else ThrowHelper.ThrowInvalidOperationException("Missing console command to modify");

                Source.Add(new ConsoleRestart());

                MachineState = MachineStateProvider.Create(
                    SettingsService.GetValue<int>(SettingsKeys.MemorySize),
                    SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode));

                Source.Add(new ConsoleCommand());

                Text = Memory<char>.Empty;
            }
        }

        /// <summary>
        /// Clears the current console output
        /// </summary>
        private async Task ClearScreenAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                Source.Clear();

                Source.Add(new ConsoleCommand());

                Text = Memory<char>.Empty;
            }
        }

        /// <summary>
        /// Executes the current console command
        /// </summary>
        private async Task ExecuteCommandAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (!(Source.LastOrDefault() is ConsoleCommand command))
                {
                    ThrowHelper.ThrowInvalidOperationException("Missing console command to run");

                    return;
                }

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
                string stdin = Messenger.Send(new StdinRequestMessage(false));

                Option<InterpreterResult> result = await Task.Run(() =>
                {
                    return Brainf_ckInterpreter
                        .CreateReleaseConfiguration()
                        .WithSource(command)
                        .WithStdin(stdin)
                        .WithInitialState(MachineState)
                        .WithExecutionToken(new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token)
                        .TryRun();
                });

                if (!result.ValidationResult.IsSuccess) Source.Add(new ConsoleSyntaxError(result.ValidationResult));
                else
                {
                    // In all cases, update the current memory state
                    MachineState = result.Value!.MachineState;

                    // Display textual results and exit codes
                    if (!string.IsNullOrEmpty(result.Value!.Stdout)) Source.Add(new ConsoleResult(result.Value!.Stdout));
                    if (!result.Value!.ExitCode.HasFlag(ExitCode.Success)) Source.Add(new ConsoleException(result.Value!.ExitCode, result.Value!.HaltingInfo!));
                }
            }

            Source.Add(new ConsoleCommand());

            Text = Memory<char>.Empty;
        }

        /// <summary>
        /// Repeats the last executed script
        /// </summary>
        private async Task RepeatLastScriptAsync()
        {
            using (await ExecutionMutex.LockAsync())
            {
                if (Source.Reverse().OfType<ConsoleCommand>().Skip(1).FirstOrDefault() is ConsoleCommand previous)
                {
                    if (!(Source.LastOrDefault() is ConsoleCommand current))
                    {
                        ThrowHelper.ThrowInvalidOperationException("Missing console command to run");

                        return;
                    }

                    current.IsActive = false;
                    current.Command = previous.Command;

                    await ExecuteCommandAsync(previous.Command);
                }
            }
        }

        /// <inheritdoc/>
        void IRecipient<MemoryStateRequestMessage>.Receive(MemoryStateRequestMessage message)
        {
            message.Reply(MachineState);
        }

        /// <inheritdoc/>
        void IRecipient<RunCommandRequestMessage>.Receive(RunCommandRequestMessage message)
        {
            _ = ExecuteCommandAsync();
        }

        /// <inheritdoc/>
        void IRecipient<DeleteOperatorRequestMessage>.Receive(DeleteOperatorRequestMessage message)
        {
            _ = DeleteLastOperatorAsync();
        }

        /// <inheritdoc/>
        void IRecipient<ClearCommandRequestMessage>.Receive(ClearCommandRequestMessage message)
        {
            _ = ResetCommandAsync();
        }

        /// <inheritdoc/>
        void IRecipient<RestartConsoleRequestMessage>.Receive(RestartConsoleRequestMessage message)
        {
            _ = RestartAsync();
        }

        /// <inheritdoc/>
        void IRecipient<ClearConsoleScreenRequestMessage>.Receive(ClearConsoleScreenRequestMessage message)
        {
            _ = ClearScreenAsync();
        }

        /// <inheritdoc/>
        void IRecipient<RepeatCommandRequestMessage>.Receive(RepeatCommandRequestMessage message)
        {
            _ = RepeatLastScriptAsync();
        }

        /// <inheritdoc/>
        void IRecipient<OverflowModeSettingChangedMessage>.Receive(OverflowModeSettingChangedMessage message)
        {
            _ = RestartAsync();
        }

        /// <inheritdoc/>
        void IRecipient<MemorySizeSettingChangedMessage>.Receive(MemorySizeSettingChangedMessage message)
        {
            _ = RestartAsync();
        }

        /// <inheritdoc/>
        void IRecipient<OperatorKeyPressedNotificationMessage>.Receive(OperatorKeyPressedNotificationMessage message)
        {
            _ = TryAddOperatorAsync(message.Value);
        }
    }
}
