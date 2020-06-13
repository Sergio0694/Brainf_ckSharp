using System;
using System.Windows.Input;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    internal sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// A simple <see cref="ICommand"/> implementation wrapping a <see cref="System.Action"/> instance
        /// </summary>
        /// <remarks>
        /// This class is necessary as a workaround for a .NET Native crash that was happening when trying
        /// to use reflection and an attached property to set handlers for the buttons in the control flyout.
        /// </remarks>
        private sealed class Command : ICommand
        {
            /// <summary>
            /// The <see cref="System.Action"/> to invoke when this command is executed
            /// </summary>
            private readonly Action Action;

            /// <summary>
            /// Creates a new <see cref="Command"/> instance with the specified parameters
            /// </summary>
            /// <param name="action">The <see cref="System.Action"/> to invoke</param>
            public Command(Action action)
            {
                Action = action;
            }

            /// <inheritdoc/>
            public bool CanExecute(object parameter)
            {
                return true;
            }

            /// <inheritdoc/>
            public void Execute(object parameter)
            {
                Action();
            }

            /// <inheritdoc/>
            event EventHandler? ICommand.CanExecuteChanged
            {
                add { }
                remove { }
            }
        }
    }
}
