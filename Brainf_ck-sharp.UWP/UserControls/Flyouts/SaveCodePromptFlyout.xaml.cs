using System;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.AttachedProperties;
using Brainf_ck_sharp_UWP.FlyoutService.Interfaces;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.ViewModels;
using JetBrains.Annotations;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class SaveCodePromptFlyout : UserControl, IValidableDialog
    {
        public SaveCodePromptFlyout([NotNull] String code)
        {
            this.InitializeComponent();
            DataContext = new SaveCodePromptFlyoutViewModel();
            ViewModel.ValidStatusChanged += ViewModel_ValidStatusChanged;
            Brainf_ckCodeInlineFormatter.SetSource(CodeSpan, code);
        }

        private void ViewModel_ValidStatusChanged(object sender, bool e) => ValidStateChanged?.Invoke(this, e);

        public event EventHandler<bool> ValidStateChanged;

        public SaveCodePromptFlyoutViewModel ViewModel => DataContext.To<SaveCodePromptFlyoutViewModel>();

        public String Title => TitleBox.Text;

        private void NameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.ValidateNameAsync(sender.To<TextBox>().Text).Forget();
        }
    }
}
