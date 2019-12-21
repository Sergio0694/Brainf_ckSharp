using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.Legacy.UWP.DataModels.Misc;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using Brainf_ck_sharp.Legacy.UWP.Messages.IDE;
using Brainf_ck_sharp.Legacy.UWP.Messages.UI;
using Brainf_ckSharp.Legacy.ReturnTypes;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls
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
                string state;
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
                        FileBlock.Text = ide.Filename?.Trim(28) ?? string.Empty;
                        if (ide.Status == IDEStatus.FaultedIDE) IDEErrorRun.Text = $"[{ide.ErrorRow}, {ide.ErrorColumn}]";
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            });
            Messenger.Default.Register<BreakpointErrorStatusChangedMessage>(this, m =>
            {
                VisualStateManager.GoToState(this, m.Value ? "BreakpointsDefaultStatus" : "BreakpointsErrorStatus", false);
            });
            Messenger.Default.Register<IDEPendingChangesStatusChangedMessage>(this, m =>
            {
                VisualStateManager.GoToState(this, m.Value ? "IDEPendingChangesState" : "IDENoPendingChangesState", false);
            });
            Messenger.Default.Register<BackgroundExecutionStatusChangedMessage>(this, m =>
            {
                VisualStateManager.GoToState(this, m.Value.ExitCode.HasFlag(InterpreterExitCode.Success) || m.Value.ExitCode.HasFlag(InterpreterExitCode.NoCodeInterpreted) 
                    ? "AutorunEnabledOkState" : "AutorunEnabledFailState", false);
                string output;
                if (m.Value.ExitCode.HasFlag(InterpreterExitCode.Success)) output = m.Value.Output;
                else if (m.Value.ExitCode.HasFlag(InterpreterExitCode.NoCodeInterpreted)) output = LocalizationManager.GetResource("NoCodeInterpreted");
                else output = ScriptExceptionInfo.FromResult(m.Value).Message;
                output = output.Trim(40); // 40 is roughly the number of characters that stay in a single line
                string tooltip = string.IsNullOrEmpty(output) ? null : output;
                if (tooltip?.Equals(_AutorunTooltip) != true)
                {
                    _AutorunTooltip = tooltip;
                    ToolTipService.SetToolTip(AutorunHitCanvas, tooltip);
                }
            });
            Messenger.Default.Register<BackgroundExecutionDisabledMessage>(this, m => VisualStateManager.GoToState(this, "AutorunDisabledState", false));
        }

        // The last tooltip for the autorun icon
        [CanBeNull]
        private string _AutorunTooltip;
    }
}
