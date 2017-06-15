using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Brainf_ck_sharp_UWP.FlyoutService.Interfaces;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.ViewModels;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts
{
    public sealed partial class SaveCodePromptFlyout : UserControl, IValidableDialog
    {
        public SaveCodePromptFlyout()
        {
            this.InitializeComponent();
            DataContext = new SaveCodePromptFlyoutViewModel();
            ViewModel.ValidStatusChanged += ViewModel_ValidStatusChanged;
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
