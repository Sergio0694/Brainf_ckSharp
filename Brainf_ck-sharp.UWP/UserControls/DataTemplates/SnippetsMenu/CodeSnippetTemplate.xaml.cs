using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Brainf_ck_sharp_UWP.AttachedProperties;
using Brainf_ck_sharp_UWP.DataModels.Misc;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers.PointerEvents;

namespace Brainf_ck_sharp_UWP.UserControls.DataTemplates.SnippetsMenu
{
    public sealed partial class CodeSnippetTemplate : UserControl
    {
        public CodeSnippetTemplate()
        {
            this.InitializeComponent();
            this.ManageControlPointerStates((pointer, value) =>
            {
                // Visual states
                VisualStateManager.GoToState(this, value ? "Highlight" : "Default", false);

                // Lights
                if (pointer != PointerDeviceType.Mouse) return;
                LightBackground.StartXAMLTransformFadeAnimation(null, value ? 0.6 : 0, 200, null, EasingFunctionNames.Linear);
            });
        }

        /// <summary>
        /// Gets or sets the code snippet to display on the control
        /// </summary>
        public CodeSnippet Code
        {
            get => (CodeSnippet)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            nameof(Code), typeof(CodeSnippet), typeof(CodeSnippetTemplate),
            new PropertyMetadata(default(CodeSnippet), OnCodePropertyChanged));

        private static void OnCodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the parameters
            CodeSnippet code = e.NewValue.To<CodeSnippet>();
            if (code == null) return;

            // Title and code
            CodeSnippetTemplate @this = d.To<CodeSnippetTemplate>();
            @this.TitleBlock.Text = code.Title;
            Span host = new Span();
            string text = code.Code.Length <= 60 ? code.Code : code.Code.Substring(0, 60); // The preview is only 1 line long
            Brainf_ckCodeInlineFormatter.SetSource(host, text);
            @this.CodeBlock.Inlines.Clear();
            @this.CodeBlock.Inlines.Add(host);
        }
    }
}
