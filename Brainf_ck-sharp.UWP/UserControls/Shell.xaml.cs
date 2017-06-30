using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo;
using Brainf_ck_sharp_UWP.UserControls.InheritedControls.CustomCommandBar;
using Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard;
using Brainf_ck_sharp_UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Enums;
using MemoryViewerFlyout = Brainf_ck_sharp_UWP.UserControls.Flyouts.MemoryState.MemoryViewerFlyout;

namespace Brainf_ck_sharp_UWP.UserControls
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            // Update the effects and initialize the UI
            this.Loaded += Shell_Loaded;
            this.SizeChanged += (s, e) =>
            {
                // Loading popup, if present
                if (_LoadingPopup?.Child is LoadingPopupControl child)
                {
                    child.Width = e.NewSize.Width;
                    child.Height = e.NewSize.Height;
                }
            };
            this.InitializeComponent();
            DataContext = new ShellViewModel(() =>
            {
                String stdin = StdinHeader.StdinBuffer;
                StdinHeader.ResetStdin();
                return stdin;
            });
            Console.ViewModel.IsEnabled = true;

            // Hide the title placeholder if needed
            if (UniversalAPIsHelper.IsMobileDevice)
            {
                PCPlaceholderGrid.Visibility = Visibility.Collapsed;
                KeyboardCanvas.Visibility = Visibility.Collapsed;
                KeyboardBorder.Visibility = Visibility.Collapsed;
            }

            // Flyout management
            Messenger.Default.Register<FlyoutOpenedMessage>(this, m => ManageFlyoutUI(true));
            Messenger.Default.Register<FlyoutClosedNotificationMessage>(this, m => ManageFlyoutUI(false));
            Messenger.Default.Register<AppLoadingStatusChangedMessage>(this, m => ManageLoadingUI(m.Loading));
        }

        public ShellViewModel ViewModel => DataContext.To<ShellViewModel>();

        #region UI

        // The current loading popup
        private Popup _LoadingPopup;

        // Manages the loading UI
        private async void ManageLoadingUI(bool loading)
        {
            // Prepare and open a popup to cover the UI while the app is loading
            if (loading)
            {
                if (_LoadingPopup != null) return;
                LoadingPopupControl control = new LoadingPopupControl();
                Popup popup = new Popup { Child = control };
                control.Height = ActualHeight;
                control.Width = ActualWidth;
                control.SetVisualOpacity(0);
                popup.IsOpen = true;
                control.StartCompositionFadeAnimation(null, 1, 200, null, EasingFunctionNames.Linear);
                _LoadingPopup = popup;
            }
            else
            {
                // Hide the popup if present
                if (_LoadingPopup?.Child is LoadingPopupControl child)
                {
                    await child.StartCompositionFadeAnimationAsync(null, 0, 200, null, EasingFunctionNames.Linear);
                    _LoadingPopup.IsOpen = false;
                    _LoadingPopup = null;
                }
            }
        }

        // Adjusts the UI when a flyout is displayed in the app
        private void ManageFlyoutUI(bool shown)
        {
            FadeCanvas.IsHitTestVisible = shown;
            FadeCanvas.StartCompositionFadeAnimation(null, shown ? 1 : 0, 250, null, EasingFunctionNames.Linear);
        }

        // Initialize the effects
        private async void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            // UI setup
            FadeCanvas.SetVisualOpacity(0);
            Messenger.Default.Send(new ConsoleStatusUpdateMessage(IDEStatus.Console, LocalizationManager.GetResource("Ready"), 0, 0));
            Console.AdjustTopMargin(HeaderGrid.ActualHeight + 8);
            IDE.AdjustTopMargin(HeaderGrid.ActualHeight);
            if (UniversalAPIsHelper.IsMobileDevice)
            {
                await HeaderBorder.AttachCompositionInAppCustomAcrylicEffectAsync(HeaderBorder, 8, 800,
                    Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.6f, null,
                    HeaderCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
                OperatorsKeyboard.Background = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 10, 10, 10));
            }
            else
            {
                await HeaderBorder.AttachCompositionCustomAcrylicEffectAsync(Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.8f,
                    HeaderCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
                await KeyboardBorder.AttachCompositionCustomAcrylicEffectAsync(Color.FromArgb(byte.MaxValue, 16, 16, 16), 0.95f,
                    KeyboardCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
            }

            // Disable the swipe gestures in the keyboard pivot
            ScrollViewer scroller = CommandsPivot.FindChild<ScrollViewer>();
            scroller.PointerEntered += Scroller_PointerIn;
            scroller.PointerMoved += Scroller_PointerIn;
            scroller.PointerExited += Scroller_PointerOut;
            scroller.PointerReleased += Scroller_PointerOut;
            scroller.PointerCaptureLost += Scroller_PointerOut;

            // Welcome message
            if (AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.WelcomeMessageShown), out bool shown) && !shown)
            {
                // Show the message
                Task.Delay(2000).ContinueWith(t =>
                {
                    FlyoutManager.Instance.Show(LocalizationManager.GetResource("DevMessage"), LocalizationManager.GetResource("WelcomeText"));
                }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();

                // Update the setting
                AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.WelcomeMessageShown), true, SettingSaveMode.OverwriteIfExisting);
            }
            else if (AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.ReviewPromptShown), out bool review) && !review &&
                     AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.AppStartups), out uint startups) && startups > 4)
            {
                // Show the review prompt
                Task.Delay(2000).ContinueWith(t =>
                {
                    ReviewPromptFlyout reviewFlyout = new ReviewPromptFlyout();
                    FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("HowsItGoing"), reviewFlyout, 
                        new Thickness(0, 12, 0, 0), FlyoutDisplayMode.ActualHeight).Forget();
                }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();

                // Update the setting
                AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ReviewPromptShown), true, SettingSaveMode.OverwriteIfExisting);
            }
        }

        // Disables the swipe gesture for the keyboard pivot (swiping that pivot causes the app to crash)
        private void Scroller_PointerIn(object sender, PointerRoutedEventArgs e)
        {
            sender.To<ScrollViewer>().HorizontalScrollMode = ScrollMode.Disabled;
        }

        // Restores the original scrolling settings when the pointer is outside the keyboard pivot
        private void Scroller_PointerOut(object sender, PointerRoutedEventArgs e)
        {
            sender.To<ScrollViewer>().HorizontalScrollMode = ScrollMode.Enabled;
        }

        #endregion

        // Updates the UI and the view models when the user changes the current page
        private void PivotControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sender.To<Pivot>().SelectedIndex;
            SharedCommandBar.SwitchContent(index == 0);
            Console.ViewModel.IsEnabled = index == 0;
            IDE.ViewModel.IsEnabled = index == 1;
        }

        /// <summary>
        /// Shows the current console memory state in a flyout
        /// </summary>
        public void RequestShowMemoryState()
        {
            IReadonlyTouringMachineState source = Console.ViewModel.State;
            MemoryViewerFlyout viewer = new MemoryViewerFlyout();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("MemoryStateTitle"), viewer).Forget();
            Task.Delay(100).ContinueWith(t => viewer.ViewModel.InitializeAsync(source), TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Shows the flyout with the guide on the first 255 Unicode characters and their values
        /// </summary>
        public void RequestShowUnicodeCharacters()
        {
            UnicodeCharactersGuideFlyout flyout = new UnicodeCharactersGuideFlyout();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UnicodeTitle"), flyout).Forget();
            Task.Delay(200).ContinueWith(t => flyout.ViewModel.LoadAsync().Forget(), TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Shows the current code library to the user
        /// </summary>
        public async void RequestShowCodeLibrary()
        {
            LocalSourceCodesBrowserFlyout flyout = new LocalSourceCodesBrowserFlyout();
            FlyoutClosedResult<CategorizedSourceCode> result = await FlyoutManager.Instance.ShowAsync<LocalSourceCodesBrowserFlyout, CategorizedSourceCode>(
                LocalizationManager.GetResource("CodeLibrary"), flyout, new Thickness(), openCallback: () => flyout.ViewModel.LoadGroupsAsync().Forget());
            if (result) Messenger.Default.Send(new SourceCodeLoadingRequestedMessage(result.Value));
        }

        // Shows the small navigation keyboard popup
        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualArrowsKeyboardControl keyboard = new VirtualArrowsKeyboardControl();
            CustomCommandBarButton button = (CustomCommandBarButton)sender;
            Point point = button.GetVisualCoordinates();
            Rect area = new Rect(point, new Size(button.ActualWidth, button.ActualHeight));
            FlyoutManager.ShowCustomContextFlyout(keyboard, area, true);
        }

        // Shows the developer info
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            DevInfoFlyout flyout = new DevInfoFlyout();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("About"), flyout, new Thickness(0), FlyoutDisplayMode.ActualHeight).Forget();
        }
    }
}
