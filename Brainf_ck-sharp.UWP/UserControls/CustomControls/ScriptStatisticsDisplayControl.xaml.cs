using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.CustomControls
{
    public sealed partial class ScriptStatisticsDisplayControl : UserControl
    {
        public ScriptStatisticsDisplayControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the current result to use to extract the info to display
        /// </summary>
        public InterpreterResult Result
        {
            get => (InterpreterResult)GetValue(ResultProperty);
            set => SetValue(ResultProperty, value);
        }

        public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
            nameof(Result), typeof(InterpreterResult), typeof(ScriptStatisticsDisplayControl), 
            new PropertyMetadata(default(InterpreterResult), OnResultPropertyChanged));

        private static void OnResultPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScriptStatisticsDisplayControl @this = d.To<ScriptStatisticsDisplayControl>();
            if (e.NewValue is InterpreterResult info)
            {
                String
                    minutes = info.ElapsedTime.Minutes > 9 ? info.ElapsedTime.Minutes.ToString() : $"0{info.ElapsedTime.Minutes}",
                    seconds = info.ElapsedTime.Minutes > 9 ? info.ElapsedTime.Seconds.ToString() : $"0{info.ElapsedTime.Seconds}";
                @this.TimeBlock.Text = $"{minutes}:{seconds}.{info.ElapsedTime.Milliseconds:000}";
                @this.OperatorsBlock.Text = info.TotalOperations > 1
                    ? $"{info.TotalOperations} {LocalizationManager.GetResource("LowercaseOperators")}"
                    : LocalizationManager.GetResource("LowercaseSingleOperator");
            }
        }
    }
}
