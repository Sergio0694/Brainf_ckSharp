using System;
using System.Linq;
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
using UICompositionAnimations.Behaviours.Effects.Base;
using UICompositionAnimations.Enums;

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
                // Default effects
                _HeaderEffect?.AdjustSize();
                _KeyboardEffect?.AdjustSize();

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

        // Acrylic brush for the header
        private AttachedStaticCompositionEffect<Border> _HeaderEffect;

        // Acrylic brush for the virtual keyboard
        private AttachedStaticCompositionEffect<Border> _KeyboardEffect;

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
                _HeaderEffect = await HeaderBorder.GetAttachedInAppSemiAcrylicEffectAsync(HeaderBorder, 8, 800,
                    Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.6f,
                    HeaderCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
                OperatorsKeyboard.Background = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 10, 10, 10));
            }
            else
            {
                _HeaderEffect = await HeaderBorder.GetAttachedSemiAcrylicEffectAsync(Color.FromArgb(byte.MaxValue, 30, 30, 30), 0.8f,
                    HeaderCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
                _KeyboardEffect = await KeyboardBorder.GetAttachedSemiAcrylicEffectAsync(Color.FromArgb(byte.MaxValue, 16, 16, 16), 0.95f,
                    KeyboardCanvas, new Uri("ms-appx:///Assets/Misc/noise.png"));
            }

            // Disable the swipe gestures in the keyboard pivot
            ScrollViewer scroller = CommandsPivot.FindChild<ScrollViewer>();
            scroller.PointerEntered += Scroller_PointerIn;
            scroller.PointerMoved += Scroller_PointerIn;
            scroller.PointerExited += Scroller_PointerOut;
            scroller.PointerReleased += Scroller_PointerOut;
            scroller.PointerCaptureLost += Scroller_PointerOut;
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
            Task.Run(() => IndexedModelWithValue<Brainf_ckMemoryCell>.New(source).ToArray()).ContinueWith(t =>
            {
                viewer.Source = t.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("MemoryStateTitle"), viewer).Forget();
        }

        /// <summary>
        /// Shows the flyout with the guide on the first 255 Unicode characters and their values
        /// </summary>
        public void RequestShowUnicodeCharacters()
        {
            UnicodeCharactersGuideFlyout flyout = new UnicodeCharactersGuideFlyout();
            flyout.ViewModel.LoadAsync().Forget();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UnicodeTitle"), flyout).Forget();
        }

        /// <summary>
        /// Shows the current code library to the user
        /// </summary>
        public async void RequestShowCodeLibrary()
        {
            LocalSourceCodesBrowserFlyout flyout = new LocalSourceCodesBrowserFlyout();
            await flyout.ViewModel.LoadGroupsAsync();
            FlyoutClosedResult<CategorizedSourceCode> result = await FlyoutManager.Instance.ShowAsync<LocalSourceCodesBrowserFlyout, CategorizedSourceCode>(
                LocalizationManager.GetResource("CodeLibrary"), flyout, new Thickness());
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
