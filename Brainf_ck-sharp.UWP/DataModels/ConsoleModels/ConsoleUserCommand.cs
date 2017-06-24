using System;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.DataModels.ConsoleModels
{
    /// <summary>
    /// Indicates a user command for the interpreter console
    /// </summary>
    public sealed class ConsoleUserCommand : ConsoleCommandModelBase
    {
        private String _Command = String.Empty;

        /// <summary>
        /// Gets the current command being written by the user
        /// </summary>
        [NotNull]
        public String Command
        {
            get => _Command;
            private set => Set(ref _Command, value);
        }

        /// <summary>
        /// Updates the current being written by the user
        /// </summary>
        /// <param name="command">The updated command line</param>
        public void UpdateCommand([NotNull] String command) => Command = command;
    }
}
