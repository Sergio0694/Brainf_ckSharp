﻿using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel
{
    /// <summary>
    /// A compact memory viewer for the interactive REPL console
    /// </summary>
    public sealed partial class CompactMemoryViewer : UserControl
    {
        public CompactMemoryViewer()
        {
            this.InitializeComponent();

            ViewModel.IsActive = true;
        }
    }
}
