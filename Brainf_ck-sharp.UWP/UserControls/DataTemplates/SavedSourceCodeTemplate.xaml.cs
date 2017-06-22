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
        /// Gets or sets the source code to display on the control
        /// </summary>
        public SourceCode SourceCode
        {
            get => (SourceCode)GetValue(SourceCodeProperty);
            set => SetValue(SourceCodeProperty, value);
        }

        public static readonly DependencyProperty SourceCodeProperty = DependencyProperty.Register(
            nameof(SourceCode), typeof(SourceCode), typeof(SavedSourceCodeTemplate), 
            new PropertyMetadata(default(SourceCode), OnSourceCodePropertyChanged));

        private static void OnSourceCodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SourceCode code = e.NewValue.To<SourceCode>();
            if (code == null) return;
            SavedSourceCodeTemplate @this = d.To<SavedSourceCodeTemplate>();
            @this.TitleBlock.Text = code.Title;
            Span host = new Span();
            Brainf_ckCodeInlineFormatter.SetSource(host, code.Code);
            @this.CodeBlock.Inlines.Clear();
            @this.CodeBlock.Inlines.Add(host);
        }

        /// <summary>
        /// Gets or sets the code type for the current instance
        /// </summary>
        public SavedSourceCodeType CodeType
        {
            get => (SavedSourceCodeType)GetValue(CodeTypeProperty);
            set => SetValue(CodeTypeProperty, value);
        }

        public static readonly DependencyProperty CodeTypeProperty = DependencyProperty.Register(
            nameof(CodeType), typeof(SavedSourceCodeType), typeof(SavedSourceCodeTemplate), new PropertyMetadata(default(SavedSourceCodeType)));

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
                () => FavoriteToggleRequested?.Invoke(this, SourceCode), SourceCode.Favorited,
                () => RenameRequested?.Invoke(this, SourceCode),
                type => ShareRequested?.Invoke(this, (type, SourceCode)),
                () => DeleteRequested?.Invoke(this, SourceCode));
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
            if (CodeType == SavedSourceCodeType.Sample ||
                e.PointerDeviceType != PointerDeviceType.Touch ||
                e.HoldingState != HoldingState.Started) return;
            ShowMenuFlyout(e.GetPosition(this));
        }

        // Shows the MenuFlyout when using a mouse or a pen
        private void SavedSourceCodeTemplate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (CodeType == SavedSourceCodeType.Sample ||
                e.PointerDeviceType == PointerDeviceType.Touch) return;
            ShowMenuFlyout(e.GetPosition(this));
        }

        #endregion
    }
}
