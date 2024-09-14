using Brainf_ckSharp.Shared.Models.Console.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Brainf_ckSharp.Shared.Models.Console;

/// <summary>
/// A model for a console command being typed by the user
/// </summary>
public sealed partial class ConsoleCommand : ObservableObject, IConsoleEntry
{
    /// <summary>
    /// Gets or sets the current command being written by the user
    /// </summary>
    [ObservableProperty]
    private string command = string.Empty;

    /// <summary>
    /// Gets or sets whether or not the user is still writing code for the current command
    /// </summary>
    [ObservableProperty]
    private bool isActive = true;
}
