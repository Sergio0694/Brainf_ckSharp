using Brainf_ckSharp.Interfaces;
using Brainf_ckSharp.Tools;
using Brainf_ckSharp.UWP.Models.Console;
using Brainf_ckSharp.UWP.Models.Console.Interfaces;
using Brainf_ckSharp.UWP.ViewModels.Abstract;

namespace Brainf_ckSharp.UWP.ViewModels
{
    /// <summary>
    /// A view model for an interactive REPL console for Brainf*ck/PBrain
    /// </summary>
    public sealed class ConsoleViewModel : ItemsCollectionViewModelBase<IConsoleEntry>
    {
        /// <summary>
        /// Creates a new <see cref="ConsoleViewModel"/> instances with a new command ready to use
        /// </summary>
        public ConsoleViewModel()
        {
            Source.Add(new ConsoleCommand());
        }

        /// <summary>
        /// Gets the <see cref="IReadOnlyTuringMachineState"/> instance currently in use
        /// </summary>
        public IReadOnlyTuringMachineState MachineState { get; private set; } = TuringMachineStateProvider.Default;
    }
}
