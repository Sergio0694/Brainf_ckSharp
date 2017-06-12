using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace Brainf_ck_sharp_UWP.Views
{
    public sealed partial class IDEView : UserControl
    {
        public IDEView()
        {
            Loaded += IDEView_Loaded;
            this.InitializeComponent();
            DataContext = new IDEViewModel(EditBox.Document);
        }

        // Initializes the scroll events for the code
        private void IDEView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scroller = EditBox.FindChild<ScrollViewer>();
            scroller.ViewChanged += Scroller_ViewChanged;
        }

        // Updates the position of the line numbers when the edit box is scrolled
        private void Scroller_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // Keep the line numbers and the current cursor in sync with the code
            float target = (float) (_Top - 12 - EditBox.VerticalScrollViewerOffset);
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, target);
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + EditBox.ActualSelectionVerticalOffset));
        }

        public IDEViewModel ViewModel => DataContext.To<IDEViewModel>();

        // The current top margin
        private double _Top;

        /// <summary>
        /// Adjusts the top margin of the content in the list
        /// </summary>
        /// <param name="height">The desired height</param>
        public void AdjustTopMargin(double height)
        {
            _Top = height;
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height - 12)); // Adjust the initial offset of the line numbers and indicators
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height + 8));
            TopMarginGrid.Height = height;
        }

        private void EditBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Back) ViewModel.ApplySyntaxHighlightOnLastCharacter();
        }

        // Updates the line numbers displayed next to the code box
        private void EditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            // Draw the line numbers in the TextBlock next to the code
            int count = EditBox.GetLinesCount();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < count; i++)
            {
                builder.Append($"\n{i}");
            }
            LineBlock.Text = builder.ToString();
        }

        private void EditBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            // Update the visibility and the position of the cursor
            CursorBorder.SetVisualOpacity(EditBox.Document.Selection.Length > 0 ? 0 : 1);
            CursorBorder.SetVisualOffsetAsync(TranslationAxis.Y, (float)(_Top + 8 + EditBox.ActualSelectionVerticalOffset));
        }
    }
}
