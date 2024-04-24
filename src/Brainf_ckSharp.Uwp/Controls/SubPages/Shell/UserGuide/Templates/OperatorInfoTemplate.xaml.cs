using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Brainf_ckSharp.Uwp.Controls.SubPages.Shell.UserGuide.Templates;

/// <summary>
/// A template that displays info on a given Brainf*ck/PBrain operator
/// </summary>
public sealed partial class OperatorInfoTemplate : UserControl
{
    /// <summary>
    /// Creates a new <see cref="OperatorInfoTemplate"/>
    /// </summary>
    public OperatorInfoTemplate()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the name of the current item to describe
    /// </summary>
    public string ItemName
    {
        get => this.ItemNameBlock.Text;
        set => this.ItemNameBlock.Text = value;
    }

    /// <summary>
    /// Gets or sets the foreground brush for the current item name
    /// </summary>
    public Brush ItemNameForegroundBrush
    {
        get => this.ItemNameBlock.Foreground;
        set => this.ItemNameBlock.Foreground = value;
    }

    /// <summary>
    /// Gets or sets the description for the current item
    /// </summary>
    public string Description
    {
        get => this.DescriptionBlock.Text;
        set => this.DescriptionBlock.Text = value;
    }
}
