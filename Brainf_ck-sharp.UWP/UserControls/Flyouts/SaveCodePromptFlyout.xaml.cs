using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.AttachedProperties;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;
using Brainf_ck_sharp_UWP.ViewModels;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class SaveCodePromptFlyout : UserControl, IValidableDialog
    {
        /// <summary>
        /// Creates a new instance to display to the user
        /// </summary>
        /// <param name="code">The code that's being saved or edited</param>
        /// <param name="name">The current name for the code, if it's already saved in the database</param>
        public SaveCodePromptFlyout([NotNull] String code, [CanBeNull] String name)
        {
            Loaded += (s, e) => TitleBox.Focus(FocusState.Programmatic);
            this.InitializeComponent();
            DataContext = new SaveCodePromptFlyoutViewModel(name);
            ViewModel.ValidStatusChanged += ViewModel_ValidStatusChanged;
            Brainf_ckCodeInlineFormatter.SetSource(CodeSpan, code);
            Unloaded += (s, e) =>
            {
                this.Bindings.StopTracking();
                ViewModel.Cleanup();
                DataContext = null;
                ValidStateChanged = null;
            };
        }

        private void ViewModel_ValidStatusChanged(object sender, bool e) => ValidStateChanged?.Invoke(this, e);

        /// <inheritdoc cref="IValidableDialog"/>
        public event EventHandler<bool> ValidStateChanged;

        public SaveCodePromptFlyoutViewModel ViewModel => DataContext.To<SaveCodePromptFlyoutViewModel>();

        /// <summary>
        /// Gets the current title chosen from the <see cref="TextBox"/> inside the control
        /// </summary>
        public String Title => TitleBox.Text;

        private void NameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateNameAsync(sender.To<TextBox>().Text).Forget();
        }
    }
}
