using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Interfaces;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Views.UnicodeCharactersMap;

public sealed partial class UnicodeCharactersMapSubPage : UserControl, IConstrainedSubPage
{
    public UnicodeCharactersMapSubPage()
    {
        this.InitializeComponent();
    }

    /// <inheritdoc/>
    public double MaxExpandedWidth { get; } = 520;

    /// <inheritdoc/>
    public double MaxExpandedHeight { get; } = double.PositiveInfinity;
}
