using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
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
            Messenger.Default.Register<IDEStatusUpdateMessageBase>(this, true, m =>
            {
                // Visual state update
                String state;
                switch (m.Status)
                {
                    case IDEStatus.Console:
                        state = "ConsoleState";
                        break;
                    case IDEStatus.FaultedConsole:
                        state = "ConsoleErrorState";
                        break;
                    case IDEStatus.IDE:
                        state = "IDEState";
                        break;
                    case IDEStatus.FaultedIDE:
                        state = "IDEErrorState";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                VisualStateManager.GoToState(this, state, false);

                // Manual UI updates
                InfoBlock.Text = m.Info;
                switch (m)
                {
                    case ConsoleStatusUpdateMessage console:
                        ErrorRun.Text = console.ErrorPosition.ToString();
                        CharRun.Text = console.Character.ToString();
                        FileGrid.Visibility = Visibility.Collapsed;
                        break;
                    case IDEStatusUpdateMessage ide:
                        RowRun.Text = ide.Row.ToString();
                        ColumnRun.Text = ide.Column.ToString();
                        FileGrid.Visibility = ide.FilenameVisibile.ToVisibility();
                        FileBlock.Text = ide.Filename ?? String.Empty;
                        if (ide.Status == IDEStatus.FaultedIDE) IDEErrorRun.Text = $"[{ide.ErrorRow}, {ide.ErrorColumn}]";
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}
