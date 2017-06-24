using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp_UWP.PopupService.Interfaces;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo
{
    public sealed partial class DevSupportPickerFlyout : UserControl, IEventConfirmedContent<int>
    {
        public DevSupportPickerFlyout()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc cref="IEventConfirmedContent{T}"/>
        public event EventHandler<int> ContentConfirmed;

        /// <inheritdoc cref="IEventConfirmedContent{T}"/>
        public int Result { get; private set; }

        // Raises the event and stores the result value
        private void OnEntryPicked(int index)
        {
            Result = index;
            ContentConfirmed?.Invoke(this, index);
        }

        // $0.99 donation
        private void Coffee_Click(object sender, RoutedEventArgs e) => OnEntryPicked(0);

        // $2.99 donation
        private void Present_Click(object sender, RoutedEventArgs e) => OnEntryPicked(1);

        // $4.99 donation
        private void Support_Click(object sender, RoutedEventArgs e) => OnEntryPicked(2);

        // $9.99 donation
        private void Love_Click(object sender, RoutedEventArgs e) => OnEntryPicked(3);
    }
}
