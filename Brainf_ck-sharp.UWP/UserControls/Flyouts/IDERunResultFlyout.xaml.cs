using System;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.ViewModels;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class IDERunResultFlyout : UserControl, IBusyWorkingContent
    {
        public IDERunResultFlyout([NotNull] InterpreterExecutionSession session)
        {
            this.InitializeComponent();
            DataContext = new IDERunResultFlyoutViewModel(session);
            ViewModel.LoadingStateChanged += (_, e) => WorkingStateChanged?.Invoke(this, e);
        }

        public IDERunResultFlyoutViewModel ViewModel => DataContext.To<IDERunResultFlyoutViewModel>();

        /// <inheritdoc cref="IBusyWorkingContent"/>
        public event EventHandler<bool> WorkingStateChanged;
    }
}
