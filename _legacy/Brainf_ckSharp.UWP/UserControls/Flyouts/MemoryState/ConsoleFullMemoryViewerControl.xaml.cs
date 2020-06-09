using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.PopupService.Interfaces;
using Brainf_ck_sharp.Legacy.UWP.ViewModels.FlyoutsViewModels;
using Brainf_ckSharp.Legacy.MemoryState;
using Brainf_ckSharp.Legacy.ReturnTypes;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.Flyouts.MemoryState
{
    public sealed partial class ConsoleFullMemoryViewerControl : UserControl, IAsyncLoadedContent
    {
        public ConsoleFullMemoryViewerControl([NotNull] IReadonlyTouringMachineState state, [NotNull] IReadOnlyList<FunctionDefinition> functions)
        {
            this.InitializeComponent();
            DataContext = new ConsoleFullMemoryViewerControlViewModel(state, functions);
            ViewModel.InitializationCompleted += (s, e) =>
            {
                LoadingPending = false;
                LoadingCompleted?.Invoke(this, EventArgs.Empty);
            };
            this.Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                ViewModel.Cleanup();
                DataContext = null;
                LoadingCompleted = null;
            };
        }

        public ConsoleFullMemoryViewerControlViewModel ViewModel => DataContext.To<ConsoleFullMemoryViewerControlViewModel>();

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public event EventHandler LoadingCompleted;

        /// <inheritdoc cref="IAsyncLoadedContent"/>
        public bool LoadingPending { get; private set; } = true;
    }
}
