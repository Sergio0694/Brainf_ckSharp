using System;
using System.Text.RegularExpressions;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp.Legacy.UWP.AttachedProperties;
using Brainf_ck_sharp.Legacy.UWP.DataModels.EventArgs;
using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite;
using Brainf_ck_sharp.Legacy.UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp.Legacy.UWP.Helpers.Extensions;
using Brainf_ck_sharp.Legacy.UWP.Helpers.UI;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp.Legacy.UWP.UserControls.DataTemplates
{
    public sealed partial class SavedSourceCodeTemplate : UserControl
    {
        public SavedSourceCodeTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((pointer, value) =>
            {
                // Visual states
                if (value) VisualStateManager.GoToState(this, "Highlight", false);
                else if (!_FlyoutOpen) VisualStateManager.GoToState(this, "Default", false);

                // Lights
                if (pointer != PointerDeviceType.Mouse) return;
                LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
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
            string text = Regex.Replace(code.Code.Code, @"[^-+\[\]\.,><():]", "");
            if (text.Length > 150) text = text.Substring(0, 150); // Only parse the first 150 characters to increase performance
            Brainf_ckCodeInlineFormatter.SetSource(host, text);
            @this.CodeBlock.Inlines.Clear();
            @this.CodeBlock.Inlines.Add(host);
        }

        /// <summary>
        /// Gets or sets whether or not this saved code is currently selected
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(SavedSourceCodeTemplate), new PropertyMetadata(false, OnIsSelectedPropertyChanged));

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SavedSourceCodeTemplate @this = d.To<SavedSourceCodeTemplate>();
            bool selected = e.NewValue.To<bool>();
            @this.IsHitTestVisible = !selected;
            @this.Opacity = selected ? 0.6 : 1;
            @this.LightBorder.StartXAMLTransformFadeAnimation(null, selected ? 0 : 1, 200, null, EasingFunctionNames.Linear);
            @this.FadeCanvas.StartXAMLTransformFadeAnimation(null, selected ? 1 : 0, 200, null, EasingFunctionNames.Linear);
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
        public event EventHandler<SourceCodeShareEventArgs> ShareRequested;

        /// <summary>
        /// Raised whenever the user requests to delete the current item
        /// </summary>
        public event EventHandler<SourceCode> DeleteRequested;

        /// <summary>
        /// Raised whenever the user requests to export the selected source code as a C program
        /// </summary>
        public event EventHandler<SourceCode> TranslateToCRequested; 

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
                type => ShareRequested?.Invoke(this, new SourceCodeShareEventArgs(type, CodeInfo?.Code)),
                () => TranslateToCRequested?.Invoke(this, CodeInfo?.Code), CodeInfo?.IsSyntaxValid == true,
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
