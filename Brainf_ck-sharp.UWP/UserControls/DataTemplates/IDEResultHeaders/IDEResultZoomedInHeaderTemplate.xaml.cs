using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.IDEResultHeaders
{
    public sealed partial class IDEResultZoomedInHeaderTemplate : UserControl
    {
        public IDEResultZoomedInHeaderTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((_, value) => VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false));
        }

        /// <summary>
        /// Gets or sets the title to display in the control
        /// </summary>
        public String Title
        {
            get => (String)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(String), typeof(IDEResultZoomedInHeaderTemplate), new PropertyMetadata(default(String), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<IDEResultZoomedInHeaderTemplate>().Block.Text = e.NewValue.To<String>() ?? String.Empty;
        }
    }
}
