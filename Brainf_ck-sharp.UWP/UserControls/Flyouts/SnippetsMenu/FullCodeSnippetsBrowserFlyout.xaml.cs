using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Messages.IDE;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.SnippetsMenu
{
    public sealed partial class FullCodeSnippetsBrowserFlyout : UserControl, IEventConfirmedContent
    {
        public FullCodeSnippetsBrowserFlyout([NotNull] ITextDocument document)
        {
            this.InitializeComponent();
            this.DataContext = new CustomRichEditBoxContextMenuViewModel(document);
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                DataContext = null;
                ContentConfirmed = null;
            };
        }

        public CustomRichEditBoxContextMenuViewModel ViewModel => this.DataContext.To<CustomRichEditBoxContextMenuViewModel>();

        // Raises the ContentConfirmed event when the user clicks on a code snippet
        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IndexedModelWithValue<CodeSnippet> code)
            {
                Messenger.Default.Send(new CodeSnippetSelectedMessage(code.Value));
                ContentConfirmed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc cref="IEventConfirmedContent"/>
        public event EventHandler ContentConfirmed;

        private void CutButton_Tapped(object sender, TappedRoutedEventArgs e) => RequestClipboardOperation(ClipboardOperation.Cut);

        private void CopyButton_Tapped(object sender, TappedRoutedEventArgs e) => RequestClipboardOperation(ClipboardOperation.Copy);

        private void PasteButton_Tapped(object sender, TappedRoutedEventArgs e) => RequestClipboardOperation(ClipboardOperation.Paste);

        // Requests a specific clipboard operation
        private void RequestClipboardOperation(ClipboardOperation operation)
        {
            Messenger.Default.Send(new ClipboardOperationRequestMessage(operation));
            ContentConfirmed?.Invoke(this, EventArgs.Empty);
        }
    }
}
