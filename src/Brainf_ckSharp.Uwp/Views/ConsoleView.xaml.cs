using System.Linq;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Shared.Enums.Settings;
using Brainf_ckSharp.Shared.Models.Console;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

#nullable enable

namespace Brainf_ckSharp.Uwp.Views
{
    /// <summary>
    /// A view for an interactive REPL console for Brainf*ck/PBrain
    /// </summary>
    public sealed partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            this.InitializeComponent();

            Messenger.Default.Register<ValueChangedMessage<IdeTheme>>(this, m =>
            {
                // We want to render all the displayed commands again using the new theme.
                // The easy way to do that is to simply retrieve all the existing commands
                // and simulate an update of their embedded script. This will cause the
                // attached property to redraw the text with the new syntax highlight
                // theme, even if the source code is actually the same as it was before.
                foreach (ConsoleCommand item in ViewModel.Source.OfType<ConsoleCommand>())
                {
                    string text = item.Command;

                    item.Command = string.Empty;

                    item.Command = text;
                }
            });
        }
    }
}
