using System.Text.RegularExpressions;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Brainf_ck_sharp_UWP.AttachedProperties;
using Brainf_ck_sharp_UWP.DataModels;
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
        public IndexedModelWithValue<CodeSnippet> Code
        {
            get => (IndexedModelWithValue<CodeSnippet>)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            nameof(Code), typeof(IndexedModelWithValue<CodeSnippet>), typeof(CodeSnippetTemplate),
            new PropertyMetadata(default(IndexedModelWithValue<CodeSnippet>), OnCodePropertyChanged));

        private static void OnCodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Unpack the parameters
            if (!(e.NewValue is IndexedModelWithValue<CodeSnippet> code)) return;

            // Title and code
            CodeSnippetTemplate @this = d.To<CodeSnippetTemplate>();
            @this.TitleBlock.Text = code.Value.Title;
            Span host = new Span();
            string
                operators = Regex.Replace(code.Value.Code, @"[^-+\[\]\.,><():]", ""),
                trimmed = operators.Length <= 60 ? operators : operators.Substring(0, 60); // The preview is only 1 line long
            Brainf_ckCodeInlineFormatter.SetSource(host, trimmed);
            @this.CodeBlock.Inlines.Clear();
            @this.CodeBlock.Inlines.Add(host);
            
            // Adjust the UI (warning: ugly hacks ahead)
            @this.BottomSeparator.Visibility = (code.Index != 3).ToVisibility();
            @this.LightBorder.Margin = new Thickness(0, 0, 0, (code.Index != 3) ? 1 : 0);
        }
    }
}
