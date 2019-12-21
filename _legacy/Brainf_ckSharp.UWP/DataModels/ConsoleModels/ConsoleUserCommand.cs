using JetBrains.Annotations;

namespace Brainf_ck_sharp.Legacy.UWP.DataModels.ConsoleModels
{
    /// <summary>
    /// Indicates a user command for the interpreter console
    /// </summary>
    public sealed class ConsoleUserCommand : ConsoleCommandModelBase
    {
        private string _Command = string.Empty;

        /// <summary>
        /// Gets the current command being written by the user
        /// </summary>
        [NotNull]
        public string Command
        {
            get => _Command;
            private set => Set(ref _Command, value);
        }

        /// <summary>
        /// Updates the current being written by the user
        /// </summary>
        /// <param name="command">The updated command line</param>
        public void UpdateCommand([NotNull] string command) => Command = command;
    }
}
