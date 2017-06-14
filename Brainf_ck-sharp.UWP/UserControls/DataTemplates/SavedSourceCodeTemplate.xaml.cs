using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Brainf_ck_sharp_UWP.AttachedProperties;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Helpers;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates
{
    public sealed partial class SavedSourceCodeTemplate : UserControl
    {
        public SavedSourceCodeTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((_, value) => VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false));
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
            Brainf_ckCodeInlineFormatter.SetSource(host, Regex.Replace(code.Code, @"[^\+\-\[\]\.,><]", ""));
            @this.CodeBlock.Inlines.Clear();
            @this.CodeBlock.Inlines.Add(host);
        }
    }
}
