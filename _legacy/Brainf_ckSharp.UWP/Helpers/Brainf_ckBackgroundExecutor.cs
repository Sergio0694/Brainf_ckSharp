using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Settings;
using Brainf_ck_sharp.Legacy.UWP.Messages.Requests;
using Brainf_ck_sharp.Legacy.UWP.Messages.UI;
using Brainf_ckSharp.Legacy;
using Brainf_ckSharp.Legacy.Enums;
using Brainf_ckSharp.Legacy.MemoryState;
using Brainf_ckSharp.Legacy.ReturnTypes;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.Helpers
{
    /// <summary>
    /// A service that executes the current code the user is working on periodically in the background, notifying the UI
    /// </summary>
    public sealed class Brainf_ckBackgroundExecutor
    {
        #region Fields and constructor

        // The interval between background runs
        private const int ExecutionInterval = 1200;

        // The maximum run time for each code to test
        private const int ExecutionTimeThreshold = 1000;

        // The timer to schedule the execution of the code
        [NotNull]
        private readonly DispatcherTimer Timer = new DispatcherTimer();

        private Brainf_ckBackgroundExecutor()
        {
            Timer.Interval = TimeSpan.FromMilliseconds(ExecutionInterval);
            Timer.Tick += Timer_Tick;
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Gets the singleton <see cref="Brainf_ckBackgroundExecutor"/> instance
        /// </summary>
        [NotNull]
        public static Brainf_ckBackgroundExecutor Instance { get; } = new Brainf_ckBackgroundExecutor();

        private bool _IsEnabled;

        /// <summary>
        /// Gets or sets whether or not the background code execution is enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                if (_IsEnabled == value) return;
                _IsEnabled = value;
                if (value) Timer.Start();
                else Timer.Stop();
            }
        }

        #endregion

        // Executes the code
        private async void Timer_Tick(object sender, object e)
        {
            // Setup
            (string code, string stdin, IReadonlyTouringMachineState state) = await Messenger.Default.RequestAsync<(string, string, IReadonlyTouringMachineState), BackgroundExecutionInputRequestMessage>();
            OverflowMode mode = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ByteOverflowModeEnabled))
                    ? OverflowMode.ByteOverflow
                    : OverflowMode.ShortNoOverflow;

            // Execute and notify the UI
            InterpreterResult result = await Task.Run(() => Brainf_ckInterpreter.Run(code, stdin, state, mode, ExecutionTimeThreshold));
            Messenger.Default.Send(new BackgroundExecutionStatusChangedMessage(result));
        }
    }
}
