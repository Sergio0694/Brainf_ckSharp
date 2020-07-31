using Brainf_ckSharp.Shared.Models.Console.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.Models.Console
{
    /// <summary>
    /// A model for a console command being typed by the user
    /// </summary>
    public sealed class ConsoleCommand : ObservableObject, IConsoleEntry
    {
        private string _Command = string.Empty;

        /// <summary>
        /// Gets or sets the current command being written by the user
        /// </summary>
        public string Command
        {
            get => _Command;
            set => SetProperty(ref _Command, value);
        }

        private bool _IsActive = true;

        /// <summary>
        /// Gets or sets whether or not the user is still writing code for the current command
        /// </summary>
        public bool IsActive
        {
            get => _IsActive;
            set => SetProperty(ref _IsActive, value);
        }
    }
}
