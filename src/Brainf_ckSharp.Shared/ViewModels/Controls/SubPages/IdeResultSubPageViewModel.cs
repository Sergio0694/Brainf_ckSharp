using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Brainf_ckSharp.Configurations;
using Brainf_ckSharp.Enums;
using Brainf_ckSharp.Models;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared.Constants;
using Brainf_ckSharp.Shared.Enums;
using Brainf_ckSharp.Shared.Messages.InputPanel;
using Brainf_ckSharp.Shared.Models.Ide.Views;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Nito.AsyncEx;

#pragma warning disable CA1001, IDE0290

namespace Brainf_ckSharp.Shared.ViewModels.Controls.SubPages;

/// <summary>
/// A viewmodel for the IDE results page displayed when running a script.
/// </summary>
public sealed partial class IdeResultSubPageViewModel : ObservableRecipient
{
    /// <summary>
    /// The <see cref="ISettingsService"/> instance currently in use
    /// </summary>
    private readonly ISettingsService settingsService;

    /// <summary>
    /// A mutex to avoid race conditions when handling executions and tokens
    /// </summary>
    private readonly AsyncLock loadingMutex = new();

    /// <summary>
    /// The <see cref="CancellationTokenSource"/> to handle executions within <see cref="LoadDataAsync"/>
    /// </summary>
    private CancellationTokenSource? executionTokenSource;

    /// <summary>
    /// The <see cref="CancellationTokenSource"/> to handle debug breakpoints within <see cref="LoadDataAsync"/>
    /// </summary>
    private CancellationTokenSource? debugTokenSource;

    /// <summary>
    /// The <see cref="InterpreterSession"/> to use, when in DEBUG mode
    /// </summary>
    private InterpreterSession? debugSession;

    /// <summary>
    /// Creates a new <see cref="IdeResultSubPageViewModel"/> instance
    /// </summary>
    /// <param name="messenger">The <see cref="IMessenger"/> instance to use</param>
    /// <param name="settingsService">The <see cref="ISettingsService"/> instance to use</param>
    public IdeResultSubPageViewModel(IMessenger messenger, ISettingsService settingsService)
        : base(messenger)
    {
        this.settingsService = settingsService;
    }

    /// <summary>
    /// Gets the current collection of sections to display
    /// </summary>
    public ObservableGroupedCollection<IdeResultSection, IdeResultWithSectionInfo> Source { get; } = [];

    /// <summary>
    /// Gets or sets the script to execute
    /// </summary>
    public string? Script { get; set; }

    /// <summary>
    /// Gets or sets the breakpoints to use
    /// </summary>
    /// <remarks>If <see langword="null"/>, the RELEASE mode is used to run the code</remarks>
    public IMemoryOwner<int>? Breakpoints { get; set; }

    /// <summary>
    /// Gets whether or not the debugger is currently stopped at a breakpoint
    /// </summary>
    public bool IsAtBreakpoint => this.debugSession?.Current.ExitCode.HasFlag(ExitCode.BreakpointReached) == true;

    /// <inheritdoc/>
    protected override async void OnDeactivated()
    {
        this.executionTokenSource?.Cancel();
        this.debugTokenSource?.Cancel();

        using (await this.loadingMutex.LockAsync())
        {
            this.debugSession?.Dispose();
        }
    }

    /// <summary>
    /// Loads the currently available code samples and recently used files
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        Guard.IsNotNull(Source);

        using (await this.loadingMutex.LockAsync())
        {
            this.executionTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            // Execution arguments and options
            string stdin = Messenger.Send(new StdinRequestMessage(false));
            int memorySize = this.settingsService.GetValue<int>(SettingsKeys.MemorySize);
            DataType dataType = this.settingsService.GetValue<DataType>(SettingsKeys.DataType);
            ExecutionOptions executionOptions = this.settingsService.GetValue<ExecutionOptions>(SettingsKeys.ExecutionOptions);

            // Run in RELEASE mode
            if (Breakpoints is null)
            {
                InterpreterResult result = await Task.Run(() =>
                {
                    return Brainf_ckInterpreter.TryRun(new ReleaseConfiguration
                    {
                        Source = Script.AsMemory(),
                        Stdin = stdin.AsMemory(),
                        MemorySize = memorySize,
                        DataType = dataType,
                        ExecutionOptions = executionOptions,
                        ExecutionToken = this.executionTokenSource.Token
                    }).Value!;
                });

                LoadResults(result);
            }
            else
            {
                this.debugTokenSource = new CancellationTokenSource();

                // Run in DEBUG mode
                this.debugSession = await Task.Run(() =>
                {
                    InterpreterSession session = Brainf_ckInterpreter.TryRun(new DebugConfiguration
                    {
                        Source = Script.AsMemory(),
                        Stdin = stdin.AsMemory(),
                        Breakpoints = Breakpoints.Memory,
                        MemorySize = memorySize,
                        DataType = dataType,
                        ExecutionOptions = executionOptions,
                        ExecutionToken = this.executionTokenSource.Token,
                        DebugToken = this.debugTokenSource.Token
                    }).Value!;

                    _ = session.MoveNext();

                    return session;
                });

                LoadResults(this.debugSession.Current);

                OnPropertyChanged(nameof(IsAtBreakpoint));
            }
        }
    }

    /// <summary>
    /// Continues the DEBUG session and moves ahead by one step
    /// </summary>
    [RelayCommand]
    private async Task ContinueAsync()
    {
        using (await this.loadingMutex.LockAsync())
        {
            _ = await Task.Run(() => this.debugSession!.MoveNext());

            LoadResults(this.debugSession!.Current);

            OnPropertyChanged(nameof(IsAtBreakpoint));
        }
    }

    /// <summary>
    /// Runs the current DEBUG session to completion, skipping remaining breakpoints
    /// </summary>
    [RelayCommand]
    private async Task SkipAsync()
    {
        using (await this.loadingMutex.LockAsync())
        {
            this.debugTokenSource!.Cancel();

            _ = await Task.Run(() => this.debugSession!.MoveNext());

            LoadResults(this.debugSession!.Current);

            OnPropertyChanged(nameof(IsAtBreakpoint));
        }
    }

    /// <summary>
    /// Displays the result for an execution run
    /// </summary>
    /// <param name="result">The <see cref="InterpreterResult"/> instance to display the results for</param>
    private void LoadResults(InterpreterResult result)
    {
        Source.Clear();

        // A function used to quickly add a specific section to the current collection
        void AddToSource(IdeResultSection section)
        {
            IdeResultWithSectionInfo model = new() { Section = section, Result = result };

            _ = Source.AddGroup(section, [model]);
        }

        // The order of items in the result view is as follows:
        // - (optional) Exception type
        // - (optional) Stdout buffer
        // - (optional) Error location
        // - (optional) Breakpoint location
        // - (optional) Stack trace
        // - Source code
        // - (optional) Function definitions
        // - Memory state
        // - Statistics
        //
        // Each group stores the type of section it represents, so that
        // a template selector can be used in the view. The value of each
        // group is the the whole session result, as it contains all the
        // available info for the current script execution.
        // Each template is responsible for extracting info from it
        // and display according to its own function and section type.
        if (!result.ExitCode.HasFlag(ExitCode.Success))
        {
            AddToSource(IdeResultSection.ExceptionType);
        }

        if (result.Stdout.Length > 0)
        {
            AddToSource(IdeResultSection.Stdout);
        }

        if (result.ExitCode.HasFlag(ExitCode.ExceptionThrown))
        {
            AddToSource(IdeResultSection.FaultingOperator);
        }
        else if (result.ExitCode.HasFlag(ExitCode.BreakpointReached))
        {
            AddToSource(IdeResultSection.BreakpointReached);
        }

        if (result.ExitCode.HasFlag(ExitCode.ExceptionThrown) ||
            result.ExitCode.HasFlag(ExitCode.ThresholdExceeded) ||
            result.ExitCode.HasFlag(ExitCode.BreakpointReached))
        {
            AddToSource(IdeResultSection.StackTrace);
        }

        AddToSource(IdeResultSection.SourceCode);

        if (result.Functions.Count > 0)
        {
            AddToSource(IdeResultSection.FunctionDefinitions);
        }

        AddToSource(IdeResultSection.MemoryState);
        AddToSource(IdeResultSection.Statistics);
    }
}
