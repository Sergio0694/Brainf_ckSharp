using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Brainf_ck_sharp;
using Brainf_ck_sharp.Enums;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages.Requests;
using Brainf_ck_sharp_UWP.Messages.UI;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.Helpers
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
            string
                stdin = await Messenger.Default.RequestAsync<string, StdinBufferRequestMessage>(),
                code = await Messenger.Default.RequestAsync<string, SourceCodeRequestMessage>();
            OverflowMode mode = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.ByteOverflowModeEnabled))
                    ? OverflowMode.ByteOverflow
                    : OverflowMode.ShortNoOverflow;

            // Execute and notify the UI
            InterpreterResult result = await Task.Run(() => Brainf_ckInterpreter.Run(code, stdin, mode, threshold: ExecutionTimeThreshold));
            Messenger.Default.Send(new BackgroundExecutionStatusChangedMessage(result));
        }
    }
}
