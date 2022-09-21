using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.ViewModels.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Brainf_ckSharp.Uwp.Controls.Host;

public sealed partial class StatusBar : UserControl
{
    public StatusBar()
    {
        this.InitializeComponent();
        this.DataContext = App.Current.Services.GetRequiredService<StatusBarViewModel>();

        ViewModel.IsActive = true;
    }

    /// <summary>
    /// Gets the <see cref="StatusBarViewModel"/> instance currently in use
    /// </summary>
    public StatusBarViewModel ViewModel => (StatusBarViewModel)DataContext;
}
