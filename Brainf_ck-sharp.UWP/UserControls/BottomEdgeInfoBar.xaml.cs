using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ck_sharp_UWP.UserControls
{
    /// <summary>
    /// A thin status bar that indicates the IDE status
    /// </summary>
    public sealed partial class BottomEdgeInfoBar : UserControl
    {
        public BottomEdgeInfoBar()
        {
            this.InitializeComponent();
            Messenger.Default.Register<IDEStatusUpdateMessage>(this, m =>
            {
                // Visual state update
                String state = m.Status == IDEStatus.Console
                    ? "ConsoleState"
                    : m.Status == IDEStatus.IDE
                        ? "IDEState"
                        : "IDEErrorState";
                VisualStateManager.GoToState(this, state, false);

                // Manual UI updates
                InfoBlock.Text = m.Info;
                RowRun.Text = m.Row.ToString();
                ColumnTitleRun.Text = m.Status == IDEStatus.Console ? "Char" : "Col";
                ColumnRun.Text = m.Character.ToString();
                FileBlock.Text = m.Filename;
            });
        }
    }
}
