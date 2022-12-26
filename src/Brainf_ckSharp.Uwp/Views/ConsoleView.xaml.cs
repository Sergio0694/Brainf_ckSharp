using System.Linq;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Messages.Settings;
using Brainf_ckSharp.Shared.Models.Console;
using Brainf_ckSharp.Shared.ViewModels.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Brainf_ckSharp.Uwp.Views;

/// <summary>
/// A view for an interactive REPL console for Brainf*ck/PBrain
/// </summary>
public sealed partial class ConsoleView : UserControl
{
    public ConsoleView()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<ConsoleViewModel>();

        App.Current.Services.GetRequiredService<IMessenger>().Register<ConsoleView, IdeThemeSettingChangedMessage>(this, (r, _) => r.RefreshDisplayedCommands());
    }

    /// <summary>
    /// Gets the <see cref="ConsoleViewModel"/> instance currently in use
    /// </summary>
    public ConsoleViewModel ViewModel => (ConsoleViewModel)DataContext;

    /// <summary>
    /// Gets or sets the spacing of the bottom footer
    /// </summary>
    public double FooterSpacing
    {
        get => FooterBorder.Height - 12;
        set => FooterBorder.Height = value + 12;
    }

    /// <summary>
    /// Forces all the displayed commands to be rendered again with updated settings
    /// </summary>
    private void RefreshDisplayedCommands()
    {
        // We want to render all the displayed commands again using the new theme.
        // The easy way to do that is to simply retrieve all the existing commands
        // and simulate an update of their embedded script. This will cause the
        // attached property to redraw the text with the new syntax highlight
        // theme, even if the source code is actually the same as it was before.
        foreach (ConsoleCommand item in ViewModel.Source.OfType<ConsoleCommand>())
        {
            string text = item.Command;

            item.Command = string.Empty;

            item.Command = text;
        }
    }
}
