using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Brainf_ck_sharp;
using Brainf_ck_sharp.Enums;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels.ConsoleModels;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.UI;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.IDE;
using Brainf_ck_sharp_UWP.Messages.KeyboardShortcuts;
using Brainf_ck_sharp_UWP.Messages.Settings;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.ViewModels.Abstract;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.ViewModels
{
    public class ConsoleViewModel : ItemsCollectionViewModelBase<ConsoleCommandModelBase>
    {
        public ConsoleViewModel()
        {
            Source.Add(new ConsoleUserCommand());
            Messenger.Default.Register<OverflowModeChangedMessage>(this, m =>
            {
                // Check if the mode was enabled and a visual refresh is needed
                if (m.Value == OverflowMode.ByteOverflow && State.Any(cell => cell.Value > byte.MaxValue))
                {
                    State = State.ApplyByteOverflow();
                    Messenger.Default.Send(new ConsoleMemoryStateChangedMessage(State));
                }
            });
        }

        /// <summary>
        /// Raised whenever a new console line is added to the source collection or edited
        /// </summary>
        public event EventHandler ConsoleLineAddedOrModified;

        private bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the instance is enabled and it is processing incoming messages
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (Set(ref _IsEnabled, value))
                {
                    if (value)
                    {
                        Messenger.Default.Register<OperatorAddedMessage>(this, op => TryAddCommandCharacter(op.Value));
                        Messenger.Default.Register<PlayScriptMessage>(this, m =>
                        {
                            if (m.Type == ScriptPlayType.Default) ExecuteCommand(m.StdinBuffer, m.Mode).Forget();
                            else if (m.Type == ScriptPlayType.RepeatedCommand) RepeatLastScript(m.StdinBuffer, m.Mode).Forget();
                        });
                        Messenger.Default.Register<ClearConsoleLineMessage>(this, m => TryResetCommand());
                        Messenger.Default.Register<UndoConsoleCharacterMessage>(this, m => TryUndoLastCommandCharacter());
                        Messenger.Default.Register<RestartConsoleMessage>(this, m => Restart());
                        Messenger.Default.Register<ClearScreenMessage>(this, m => TryClearScreen());
                        Messenger.Default.Register<CharacterReceivedMessage>(this, m => TryAddCommandCharacter(m.Value));
                        Messenger.Default.Register<DeleteKeyPressedMessage>(this, _ => TryUndoLastCommandCharacter());
                        SendCommandAvailableMessages();
                    }
                    else Messenger.Default.Unregister(this);
                }
            }
        }

        /// <summary>
        /// Gets the current machine state to use to process the scripts
        /// </summary>
        [NotNull]
        public IReadonlyTouringMachineState State { get; private set; } = TouringMachineStateProvider.Initialize(AppSettingsParser.InterpreterMemorySize);

        /// <summary>
        /// Gets the currently defined functions
        /// </summary>
        [NotNull]
        public IReadOnlyList<FunctionDefinition> Functions { get; private set; } = new FunctionDefinition[0];

        private bool _CanRestart;

        /// <summary>
        /// Gets whether or not the console can be restarted from its current state
        /// </summary>
        public bool CanRestart
        {
            get => _CanRestart;
            private set
            {
                if (Set(ref _CanRestart, value))
                    Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.Restart, value));
            }
        }

        /// <summary>
        /// Restarts the console and resets the current state
        /// </summary>
        public void Restart()
        {
            if (!CanRestart) return;
            CanRestart = false;
            Source.Add(new ConsoleRestartCommand());
            State = TouringMachineStateProvider.Initialize(AppSettingsParser.InterpreterMemorySize);
            Functions = new FunctionDefinition[0];
            Source.Add(new ConsoleUserCommand());
            Messenger.Default.Send(new ConsoleMemoryStateChangedMessage(State));
        }

        /// <summary>
        /// Tries to repeat the previous command
        /// </summary>
        /// <param name="stdin">The current input buffer</param>
        /// <param name="mode">The overflow mode to use</param>
        private async Task RepeatLastScript([NotNull] string stdin, OverflowMode mode)
        {
            // Retrieve the current and the last command
            ConsoleUserCommand current = null, last = null;
            foreach (ConsoleUserCommand model in Source.Reverse().Where(model => model is ConsoleUserCommand).Cast<ConsoleUserCommand>())
            {
                if (current == null) current = model;
                else if (last == null && model.Command.Length > 0) last = model;
                else if (last != null) break;
            }
            if (current == null || last == null) return;

            // Repeat the script
            current.UpdateCommand(last.Command);
            await ExecuteCommand(stdin, mode);
        }

        /// <summary>
        /// Gets whether or not there is an available user command to execute
        /// </summary>
        public bool CommandAvailable => Source.LastOrDefault() is ConsoleUserCommand command &&
                                        command.Command.Length > 0;

        // Broadcasts the messages to reflect the current console status
        private void SendCommandAvailableMessages(bool? forcedStatus = null)
        {
            bool play = forcedStatus ?? CommandAvailable && Brainf_ckInterpreter.CheckSourceSyntax(Source.Last().To<ConsoleUserCommand>().Command).Valid;
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.Play, play));
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.DeleteLastCharacter, forcedStatus ?? CommandAvailable));
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.Clear, forcedStatus ?? CommandAvailable));
            bool script = Source.Skip(forcedStatus == false ? 0 : 1).Where(model => model is ConsoleUserCommand).Cast<ConsoleUserCommand>().Any(model => model.Command.Length > 0);
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.RepeatLastScript, script));
            if (Source.Last() is ConsoleUserCommand command &&
                command.Command.Length > 0 && forcedStatus != false)
            {
                SyntaxValidationResult result = Brainf_ckInterpreter.CheckSourceSyntax(command.Command);
                Messenger.Default.Send(result.Valid 
                    ? new ConsoleStatusUpdateMessage(IDEStatus.Console, LocalizationManager.GetResource("Ready"), command.Command.Length, 0) 
                    : new ConsoleStatusUpdateMessage(IDEStatus.FaultedConsole, LocalizationManager.GetResource("Warning"), command.Command.Length, result.ErrorPosition));
            }
            else Messenger.Default.Send(new ConsoleStatusUpdateMessage(IDEStatus.Console, LocalizationManager.GetResource("Ready"), 0, 0));
            Messenger.Default.Send(new AvailableActionStatusChangedMessage(SharedAction.ClearScreen, ClearScreenAvailable));
        }

        /// <summary>
        /// Tries to delete the last character in the active command line
        /// </summary>
        public void TryUndoLastCommandCharacter()
        {
            if (!CommandAvailable) return;
            ConsoleUserCommand command = (ConsoleUserCommand)Source.Last();
            command.UpdateCommand(command.Command.Substring(0, command.Command.Length - 1));
            SendCommandAvailableMessages();
            ConsoleLineAddedOrModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets whether or not it is possible to clear the screen
        /// </summary>
        public bool ClearScreenAvailable => Source.Count > 1 ||
                                            Source.First() is ConsoleUserCommand command && command.Command.Length > 0;

        /// <summary>
        /// Clears the screen, if there are console lines to remove
        /// </summary>
        public void TryClearScreen()
        {
            if (!ClearScreenAvailable) return;
            Clear();
            Source.Add(new ConsoleUserCommand());
            SendCommandAvailableMessages();
        }

        /// <summary>
        /// Tries to reset the text in the active command
        /// </summary>
        public void TryResetCommand()
        {
            if (!CommandAvailable) return;
            ConsoleUserCommand command = (ConsoleUserCommand)Source.Last();
            command.UpdateCommand(string.Empty);
            SendCommandAvailableMessages();
            ConsoleLineAddedOrModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Executes the current user command, if possible
        /// </summary>
        public async Task ExecuteCommand([NotNull] string stdin, OverflowMode mode)
        {
            if (!CommandAvailable) return;
            CanRestart = true;
            SendCommandAvailableMessages(false);
            string command = Source.Last().To<ConsoleUserCommand>().Command;
            InterpreterResult result = await Task.Run(() => Brainf_ckInterpreter.Run(command, stdin, State, Functions, mode, 1000));
            if (result.ExitCode.HasFlag(InterpreterExitCode.Success) &&
                result.ExitCode.HasFlag(InterpreterExitCode.TextOutput))
            {
                // Text output
                Source.Add(new ConsoleCommandResult(result.Output));
            }
            else if (!result.ExitCode.HasFlag(InterpreterExitCode.Success))
            {
                ScriptExceptionInfo info = ScriptExceptionInfo.FromResult(result);
                Source.Add(new ConsoleExceptionResult(info));
            }
            State = result.MachineState;
            Functions = result.Functions;

            // New user command
            Source.Add(new ConsoleUserCommand());
            ConsoleLineAddedOrModified?.Invoke(this, EventArgs.Empty);
            Messenger.Default.Send(new ConsoleMemoryStateChangedMessage(result.MachineState));
        }

        /// <summary>
        /// Tries to add a new operator to the active user command line
        /// </summary>
        /// <param name="c">The new operator to add</param>
        public void TryAddCommandCharacter(char c)
        {
            if (!Brainf_ckInterpreter.Operators.Contains(c)) return;
            if (Source.LastOrDefault() is ConsoleUserCommand command)
            {
                command.UpdateCommand($"{command.Command}{c}");
                SendCommandAvailableMessages();
            }
            ConsoleLineAddedOrModified?.Invoke(this, EventArgs.Empty);
        }
    }
}
