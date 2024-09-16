using System;
using System.Threading;
using Brainf_ckSharp.Configurations;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Memory.Interfaces;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Models.Base;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.ViewModels.Views;
using Brainf_ckSharp.Shared.ViewModels.Views.Abstract;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

#pragma warning disable CA1001

namespace Brainf_ckSharp.Shared.ViewModels.Controls;

/// <summary>
/// A viewmodel for the status bar in the application.
/// </summary>
public sealed partial class StatusBarViewModel : ObservableRecipient
{
    /// <summary>
    /// The <see cref="ISettingsService"/> instance currently in use
    /// </summary>
    private readonly ISettingsService settingsService;

    /// <summary>
    /// The <see cref="SynchronizationContext"/> in use when <see cref="StatusBarViewModel"/> is instantiated
    /// </summary>
    private readonly SynchronizationContext context;

    /// <summary>
    /// The <see cref="timer"/> instance used to perodically invoke <see cref="RunBackgroundCode"/>
    /// </summary>
    private readonly Timer timer;

    /// <summary>
    /// The last source code that was used
    /// </summary>
    private ReadOnlyMemory<char> source;

    /// <summary>
    /// The last stdin that was used
    /// </summary>
    private ReadOnlyMemory<char> stdin;

    /// <summary>
    /// The last memory size setting that was used
    /// </summary>
    private int memorySize;

    /// <summary>
    /// The last data type that was used
    /// </summary>
    private DataType dataType;

    /// <summary>
    /// The last execution options that were used
    /// </summary>
    private ExecutionOptions executionOptions;

    /// <summary>
    /// The last machine state that was used, if available
    /// </summary>
    private IReadOnlyMachineState? machineState;

    /// <summary>
    /// The backing field for <see cref="BackgroundExecutionResult"/>.
    /// </summary>
    private Option<InterpreterResult>? backgroundExecutionResult;

    /// <summary>
    /// Creates a new <see cref="StatusBarViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public StatusBarViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger)
    {
        this.settingsService = settingsService;
        this.context = SynchronizationContext.Current;
        this.timer = new Timer(vm => ((StatusBarViewModel)vm).RunBackgroundCode(), this, default, TimeSpan.FromSeconds(2));
    }

    /// <inheritdoc/>
    protected override void OnActivated()
    {
        Messenger.Register<StatusBarViewModel, PropertyChangedMessage<bool>>(this, (r, m) => r.Receive(m));
    }

    /// <summary>
    /// Gets the current <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance
    /// </summary>
    public Option<InterpreterResult>? BackgroundExecutionResult
    {
        get => this.backgroundExecutionResult;
        private set
        {
            this.backgroundExecutionResult?.Value?.MachineState.Dispose();

            this.backgroundExecutionResult = value;

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the <see cref="WorkspaceViewModelBase"/> instance in use
    /// </summary>
    [ObservableProperty]
    private WorkspaceViewModelBase? workspaceViewModel;

    /// <summary>
    /// Assigns <see cref="WorkspaceViewModel"/> and <see cref="IdeViewModel"/> when the current view model changes
    /// </summary>
    /// <param name="viewModel">The input <see cref="WorkspaceViewModelBase"/> to track</param>
    private void SetupActiveViewModel(WorkspaceViewModelBase viewModel)
    {
        WorkspaceViewModel = viewModel;

        // Restart the time to make sure to update the background
        // execution result immediately when the workspace changes.
        _ = this.timer.Change(default, TimeSpan.FromSeconds(2));
    }

    /// <summary>
    /// Runs the current code in the background, if needed
    /// </summary>
    private void RunBackgroundCode()
    {
        if (WorkspaceViewModel is not WorkspaceViewModelBase viewModel)
        {
            return;
        }

        // Load all the new arguments and data for the new execution.
        // Before actually executing the code, also check with the previously
        // stored arguments to be able to skip the execution if there were no changes.
        ReadOnlyMemory<char> source = viewModel.Text;
        ReadOnlyMemory<char> stdin = Messenger.Send(new StdinRequestMessage(true)).Response.AsMemory();
        int memorySize = this.settingsService.GetValue<int>(SettingsKeys.MemorySize);
        DataType dataType = this.settingsService.GetValue<DataType>(SettingsKeys.DataType);
        ExecutionOptions executionOptions = this.settingsService.GetValue<ExecutionOptions>(SettingsKeys.ExecutionOptions);
        IReadOnlyMachineState? machineState = (viewModel as ConsoleViewModel)?.MachineState;

        if (source.Span.SequenceEqual(this.source.Span) &&
            stdin.Span.SequenceEqual(this.stdin.Span) &&
            memorySize == this.memorySize &&
            dataType == this.dataType &&
            executionOptions == this.executionOptions &&
            ((machineState is null && this.machineState is null) ||
              machineState?.Equals(this.machineState!) == true))
        {
            return;
        }

        this.source = source;
        this.stdin = stdin;
        this.memorySize = memorySize;
        this.dataType = dataType;
        this.executionOptions = executionOptions;
        this.machineState = machineState;

        CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(2));

        Option<InterpreterResult> result = Brainf_ckInterpreter.TryRun(new ReleaseConfiguration
        {
            Source = WorkspaceViewModel.Text,
            Stdin = stdin,
            MemorySize = memorySize,
            DataType = dataType,
            ExecutionOptions = executionOptions,
            ExecutionToken = tokenSource.Token
        });

        // Update the property from the original synchronization context
        this.context.Post(_ => BackgroundExecutionResult = result, null);
    }

    /// <summary>
    /// Sets the currently active viewmodel.
    /// </summary>
    /// <param name="message">The input notification message.</param>
    private void Receive(PropertyChangedMessage<bool> message)
    {
        if (message.PropertyName == nameof(IsActive) &&
            message.NewValue &&
            message.Sender is WorkspaceViewModelBase viewModel)
        {
            SetupActiveViewModel(viewModel);
        }
    }
}
