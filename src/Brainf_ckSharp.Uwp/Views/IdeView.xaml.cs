using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Services;
using Brainf_ckSharp.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

#nullable enable

namespace Brainf_ckSharp.Uwp.Views
{
    /// <summary>
    /// A view for a Brainf*ck/PBrain IDE
    /// </summary>
    public sealed partial class IdeView : UserControl
    {
        public IdeView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Inserts the text of a selected code snippet
        /// </summary>
        /// <param name="sender">The sender <see cref="Button"/> for the snippet</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> for the current event</param>
        private void CodeSnippet_Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is string snippet)
            {
                string name = (
                    from fieldInfo in typeof(CodeSnippets).GetFields(BindingFlags.Public | BindingFlags.Static)
                    let fieldValue = (string)fieldInfo.GetRawConstantValue()
                    where fieldValue == snippet
                    select fieldInfo.Name).First();

                Ioc.Default.GetRequiredService<IAnalyticsService>().Log(Shared.Constants.Events.InsertCodeSnippet, (nameof(CodeSnippets), name));

                CodeEditor.InsertText(snippet);
            }
        }
    }
}
