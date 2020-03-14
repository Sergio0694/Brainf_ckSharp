using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Messages.InputPanel;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel.VirtualKeyboard
{
    /// <summary>
    /// A virtual keyboard with the Brainf*ck/PBrain operators
    /// </summary>
    public sealed partial class OperatorsKeyboard : UserControl
    {
        public OperatorsKeyboard()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles a given operator button being clicked
        /// </summary>
        /// <param name="sender">An <see cref="OperatorButton"/> instance with the input operator</param>
        /// <param name="e">Unused event args, the input operator is in the <see cref="OperatorButton.Operator"/> property</param>
        private void OperatorButton_OnClick(object sender, EventArgs e)
        {
            if (sender is OperatorButton button)
            {
                Debug.Assert(button.Operator != null);
                Debug.Assert(button.Operator.Length == 1);
                Debug.Assert(Brainf_ckParser.IsOperator(button.Operator[0]));

                char op = button.Operator[0];

                Messenger.Default.Send(new OperatorKeyPressedNotificationMessage(op));
            }
            else throw new InvalidOperationException("Invalid operator button pressed");
        }
    }
}
