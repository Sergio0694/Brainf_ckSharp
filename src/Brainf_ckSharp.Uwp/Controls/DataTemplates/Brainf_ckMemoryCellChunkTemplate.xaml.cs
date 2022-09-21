using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Models.Console.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.DataTemplates;

/// <summary>
/// A template to display a <see cref="Brainf_ckMemoryCellChunk"/> model
/// </summary>
public sealed partial class Brainf_ckMemoryCellChunkTemplate : UserControl
{
    public Brainf_ckMemoryCellChunkTemplate()
    {
        this.InitializeComponent();
        this.DataContextChanged += (s, e) => this.Bindings.Update();
    }

    /// <summary>
    /// Gets the <see cref="Brainf_ckMemoryCellChunk"/> instance for the current view
    /// </summary>
    public Brainf_ckMemoryCellChunk? ViewModel => this.DataContext as Brainf_ckMemoryCellChunk;
}
