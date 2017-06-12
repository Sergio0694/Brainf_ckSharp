using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
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
            Messenger.Default.Register<OperatorAddedMessage>(this, m => AddOperatorFromVirtualKeyboard(m.Operator));
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
            float target = (float) (_Top - 8 - EditBox.VerticalScrollViewerOffset);
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, target);
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
            LinesGrid.SetVisualOffsetAsync(TranslationAxis.Y, (float)(height - 8));
            TopMarginGrid.Height = height;
        }

        private void AddOperatorFromVirtualKeyboard(char c)
        {
            ViewModel.InsertSingleCharacter(c);
        }

        private void EditBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Back) ViewModel.ApplySyntaxHighlightOnLastCharacter();
        }

        // Updates the line numbers displayed next to the code box
        private void EditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            int count = EditBox.GetLinesCount();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < count; i++)
            {
                builder.Append($"\n{i}");
            }
            LineBlock.Text = builder.ToString();
        }
    }
}
