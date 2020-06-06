using System;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Uwp.Controls.SubPages.Shell;
using Brainf_ckSharp.Uwp.Controls.SubPages.Shell.Settings;
using Brainf_ckSharp.Uwp.Controls.SubPages.Shell.UserGuide;
using Brainf_ckSharp.Uwp.Controls.SubPages.Views.UnicodeCharactersMap;
using Brainf_ckSharp.Uwp.Messages.Navigation;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Host
{
    /// <summary>
    /// The shell that aacts as root visual element for the application
    /// </summary>
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets an <see cref="ICommand"/> instance responsible for requesting to move within the code editor
        /// </summary>
        public ICommand MoveCommand { get; } = new RelayCommand<VirtualKey>(key => Messenger.Default.Send(new ValueChangedMessage<VirtualKey>(key)));

        /// <summary>
        /// Shows the user guide
        /// </summary>
        private void ViewModel_OnUserGuideRequested(object sender, EventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<UserGuideSubPage>());
        }

        /// <summary>
        /// Shows the unicode map
        /// </summary>
        private void ViewModel_OnUnicodeMapRequested(object sender, EventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<UnicodeCharactersMapSubPage>());
        }

        /// <summary>
        /// Shows the app settings
        /// </summary>
        private void ViewModel_OnSettingsRequested(object sender, EventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<SettingsSubPage>());
        }

        /// <summary>
        /// Shows the info about the current app
        /// </summary>
        private void ViewModel_OnAboutInfoRequested(object sender, EventArgs e)
        {
            Messenger.Default.Send(SubPageNavigationRequestMessage.To<AboutSubPage>());
        }
    }
}
