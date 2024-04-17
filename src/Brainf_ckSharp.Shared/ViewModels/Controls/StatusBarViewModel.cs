using System;
using System.Threading;
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
public sealed class StatusBarViewModel : ObservableRecipient
{
    /// <summary>
    /// The <see cref="ISettingsService"/> instance currently in use
    /// </summary>
    private readonly ISettingsService SettingsService;

    /// <summary>
    /// The <see cref="SynchronizationContext"/> in use when <see cref="StatusBarViewModel"/> is instantiated
    /// </summary>
    private readonly SynchronizationContext Context;

    /// <summary>
    /// The <see cref="Timer"/> instance used to perodically invoke <see cref="RunBackgroundCode"/>
    /// </summary>
    private readonly Timer Timer;

    /// <summary>
    /// The last source code that was used
    /// </summary>
    private ReadOnlyMemory<char> _Source;

    /// <summary>
    /// The last stdin that was used
    /// </summary>
    private string _Stdin = string.Empty;

    /// <summary>
    /// The last memory size setting that was used
    /// </summary>
    private int _MemorySize;

    /// <summary>
    /// The last overflow mode that was used
    /// </summary>
    private OverflowMode _OverflowMode;

    /// <summary>
    /// The last machine state that was used, if available
    /// </summary>
    private IReadOnlyMachineState? _MachineState;

    /// <summary>
    /// Creates a new <see cref="StatusBarViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public StatusBarViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger)
    {
        this.SettingsService = settingsService;
        this.Context = SynchronizationContext.Current;
        this.Timer = new Timer(vm => ((StatusBarViewModel)vm).RunBackgroundCode(), this, default, TimeSpan.FromSeconds(2));
    }
    
    /// <inheritdoc/>
    protected override void OnActivated()
    {
        Messenger.Register<StatusBarViewModel, PropertyChangedMessage<bool>>(this, (r, m) => r.Receive(m));
    }

    private Option<InterpreterResult>? _BackgroundExecutionResult;

    /// <summary>
    /// Gets the current <see cref="Option{T}"/> of <see cref="InterpreterResult"/> instance
    /// </summary>
    public Option<InterpreterResult>? BackgroundExecutionResult
    {
        get => this._BackgroundExecutionResult;
        private set
        {
            this._BackgroundExecutionResult?.Value?.MachineState.Dispose();

            this._BackgroundExecutionResult = value;

            OnPropertyChanged();
        }
    }

    private WorkspaceViewModelBase? _WorkspaceViewModel;

    /// <summary>
    /// Gets the <see cref="WorkspaceViewModelBase"/> instance in use
    /// </summary>
    public WorkspaceViewModelBase? WorkspaceViewModel
    {
        get => this._WorkspaceViewModel;
        private set => SetProperty(ref this._WorkspaceViewModel, value);
    }

    /// <summary>
    /// Assigns <see cref="WorkspaceViewModel"/> and <see cref="IdeViewModel"/> when the current view model changes
    /// </summary>
    /// <param name="viewModel">The input <see cref="WorkspaceViewModelBase"/> to track</param>
    private void SetupActiveViewModel(WorkspaceViewModelBase viewModel)
    {
        WorkspaceViewModel = viewModel;

        // Restart the time to make sure to update the background
        // execution result immediately when the workspace changes.
        this.Timer.Change(default, TimeSpan.FromSeconds(2));
    }

    /// <summary>
    /// Runs the current code in the background, if needed
    /// </summary>
    private void RunBackgroundCode()
    {
        if (WorkspaceViewModel is not WorkspaceViewModelBase viewModel) return;

        // Load all the new arguments and data for the new execution.
        // Before actually executing the code, also check with the previously
        // stored arguments to be able to skip the execution if there were no changes.
        ReadOnlyMemory<char> source = viewModel.Text;
        string stdin = Messenger.Send(new StdinRequestMessage(true));
        int memorySize = this.SettingsService.GetValue<int>(SettingsKeys.MemorySize);
        OverflowMode overflowMode = this.SettingsService.GetValue<OverflowMode>(SettingsKeys.OverflowMode);
        IReadOnlyMachineState? machineState = (viewModel as ConsoleViewModel)?.MachineState;

        if (source.Span.SequenceEqual(this._Source.Span) &&
            stdin.Equals(this._Stdin) &&
            memorySize == this._MemorySize &&
            overflowMode == this._OverflowMode &&
            ((machineState is null && this._MachineState is null) ||
              machineState?.Equals(this._MachineState!) == true))
        {
            return;
        }

        this._Source = source;
        this._Stdin = stdin;
        this._MemorySize = memorySize;
        this._OverflowMode = overflowMode;
        this._MachineState = machineState;

        CancellationTokenSource tokenSource = new(TimeSpan.FromSeconds(2));

        Option<InterpreterResult> result = Brainf_ckInterpreter
            .CreateReleaseConfiguration()
            .WithSource(WorkspaceViewModel.Text)
            .WithStdin(stdin)
            .WithMemorySize(memorySize)
            .WithOverflowMode(overflowMode)
            .WithExecutionToken(tokenSource.Token)
            .TryRun();

        // Update the property from the original synchronization context
        this.Context.Post(_ => BackgroundExecutionResult = result, null);
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
