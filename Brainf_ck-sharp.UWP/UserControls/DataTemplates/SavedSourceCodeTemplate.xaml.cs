using System;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp_UWP.AttachedProperties;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates
{
    public sealed partial class SavedSourceCodeTemplate : UserControl
    {
        public SavedSourceCodeTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((_, value) =>
            {
                if (value) VisualStateManager.GoToState(this, "Highlight", false);
                else if (!_FlyoutOpen) VisualStateManager.GoToState(this, "Default", false);
            });
        }

        /// <summary>
        /// Gets or sets the categorized source code to display on the control
        /// </summary>
        public CategorizedSourceCodeWithSyntaxInfo CodeInfo
        {
            get => (CategorizedSourceCodeWithSyntaxInfo)GetValue(CodeInfoProperty);
            set => SetValue(CodeInfoProperty, value);
        }

        public static readonly DependencyProperty CodeInfoProperty = DependencyProperty.Register(
            nameof(CodeInfo), typeof(CategorizedSourceCodeWithSyntaxInfo), typeof(SavedSourceCodeTemplate),
            new PropertyMetadata(default(CategorizedSourceCodeWithSyntaxInfo), OnCodeInfoPropertyChanged));

        private static void OnCodeInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the parameters
            CategorizedSourceCodeWithSyntaxInfo code = e.NewValue.To<CategorizedSourceCodeWithSyntaxInfo>();
            if (code == null) return;

            // Title and code
            SavedSourceCodeTemplate @this = d.To<SavedSourceCodeTemplate>();
            @this.TitleBlock.Text = code.Code.Title;
            Span host = new Span();
            Brainf_ckCodeInlineFormatter.SetSource(host, code.Code.Code);
            @this.CodeBlock.Inlines.Clear();
            @this.CodeBlock.Inlines.Add(host);
        }

        #region Events

        /// <summary>
        /// Raised whenever the user toggles the favorite status for the current code
        /// </summary>
        public event EventHandler<SourceCode> FavoriteToggleRequested;

        /// <summary>
        /// Raised whenever the user requests to rename the saved code
        /// </summary>
        public event EventHandler<SourceCode> RenameRequested;

        /// <summary>
        /// Raised whenever the user selects a share method to share the saved code
        /// </summary>
        public event EventHandler<(SourceCodeShareType, SourceCode)> ShareRequested;

        /// <summary>
        /// Raised whenever the user requests to delete the current item
        /// </summary>
        public event EventHandler<SourceCode> DeleteRequested;

        #endregion

        #region Menu flyout management

        /// <summary>
        /// Indicates whether or not a MenuFlyout is currently open
        /// </summary>
        private bool _FlyoutOpen;

        // Shows the MenuFlyout at the right position
        private void ShowMenuFlyout(Point offset)
        {
            // Get the custom MenuFlyout
            MenuFlyout menuFlyout = MenuFlyoutHelper.PrepareSavedSourceCodeMenuFlyout(
                () => FavoriteToggleRequested?.Invoke(this, CodeInfo?.Code), CodeInfo?.Code.Favorited == true,
                () => RenameRequested?.Invoke(this, CodeInfo?.Code),
                type => ShareRequested?.Invoke(this, (type, CodeInfo?.Code)),
                () => DeleteRequested?.Invoke(this, CodeInfo?.Code));
            menuFlyout.Closed += (s, e) =>
            {
                _FlyoutOpen = false;
                VisualStateManager.GoToState(this, "Default", false);
            };

            // Show the menu
            _FlyoutOpen = true;
            VisualStateManager.GoToState(this, "Highlight", false);
            menuFlyout.ShowAt(this, offset);
        }

        // Animates the control and shows the MenuFlyout when the input device is a touch screen
        private void SavedSourceCodeTemplate_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (CodeInfo?.Type == SavedSourceCodeType.Sample ||
                e.PointerDeviceType != PointerDeviceType.Touch ||
                e.HoldingState != HoldingState.Started) return;
            ShowMenuFlyout(e.GetPosition(this));
        }

        // Shows the MenuFlyout when using a mouse or a pen
        private void SavedSourceCodeTemplate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (CodeInfo?.Type == SavedSourceCodeType.Sample ||
                e.PointerDeviceType == PointerDeviceType.Touch) return;
            ShowMenuFlyout(e.GetPosition(this));
        }

        #endregion
    }
}
