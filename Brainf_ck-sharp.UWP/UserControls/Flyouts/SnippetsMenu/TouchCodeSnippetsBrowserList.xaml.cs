using System;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.ViewModels.FlyoutsViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.SnippetsMenu
{
    public sealed partial class TouchCodeSnippetsBrowserList : UserControl, IEventConfirmedContent<CodeSnippet>
    {
        public TouchCodeSnippetsBrowserList()
        {
            this.InitializeComponent();
            this.DataContext = new CodeSnippetsBrowserViewModel();
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                DataContext = null;
                ContentConfirmed = null;
            };
        }

        public CodeSnippetsBrowserViewModel ViewModel => this.DataContext.To<CodeSnippetsBrowserViewModel>();

        // Raises the ContentConfirmed event when the user clicks on a code snippet
        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is CodeSnippet code)
            {
                Result = code;
                ContentConfirmed?.Invoke(this, code);
            }
        }

        /// <inheritdoc cref="IEventConfirmedContent{T}"/>
        public event EventHandler<CodeSnippet> ContentConfirmed;

        /// <inheritdoc cref="IEventConfirmedContent{T}"/>
        public CodeSnippet Result { get; private set; }
    }
}
