using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.UWP.Constants;
using Brainf_ckSharp.UWP.Messages.InputPanel;
using Brainf_ckSharp.UWP.Services.Settings;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace Brainf_ckSharp.UWP.Controls.Host.InputPanel.Header
{
    public sealed partial class StdinHeader : UserControl
    {
        // Constants for the visual states
        private const string KeyboardSelectedVisualStateName = "KeyboardSelected";
        private const string MemoryViewerSelectedVisualStateName = "MemoryViewerSelected";

        public StdinHeader()
        {
            this.InitializeComponent();

            Messenger.Default.Register<StdinRequestMessage>(this, ExtractStdinBuffer);
        }

        /// <summary>
        /// Gets or sets the currently selected index for the stdin header buttons
        /// </summary>
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="SelectedIndex"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            nameof(SelectedIndex),
            typeof(int),
            typeof(StdinHeader),
            new PropertyMetadata(default(int), OnSelectedIndexPropertyChanged));

        /// <summary>
        /// Updates the UI when <see cref="SelectedIndex"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StdinHeader @this = (StdinHeader)d;
            int index = (int)e.NewValue;
            VisualStateManager.GoToState(@this, index == 0 ? KeyboardSelectedVisualStateName : MemoryViewerSelectedVisualStateName, false);
        }

        // Sets the selected index to 0 when the keyboard button is clicked
        private void VirtualKeyboardHeaderSelected(object sender, EventArgs e) => SelectedIndex = 0;

        // Sets the selected index to 1 when the memory viewer button is clicked
        private void MemoryViewerHeaderSelected(object sender, EventArgs e) => SelectedIndex = 1;

        /// <summary>
        /// The <see cref="ISettingsService"/> currently in use
        /// </summary>
        private readonly ISettingsService SettingsService = SimpleIoc.Default.GetInstance<ISettingsService>();

        /// <summary>
        /// Handles a request for the current stdin buffer
        /// </summary>
        /// <param name="request">The input request message for the stdin buffer</param>
        private void ExtractStdinBuffer(StdinRequestMessage request)
        {
            request.ReportResult(StdinBox.Text);

            // Clear the buffer if requested
            if (SettingsService.GetValue<bool>(SettingsKeys.ClearStdinBufferOnRequest))
            {
                StdinBox.Text = string.Empty;
            }
        }
    }
}
