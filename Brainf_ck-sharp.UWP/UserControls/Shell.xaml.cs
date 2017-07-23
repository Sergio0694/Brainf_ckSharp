using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Messages;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo;
using Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard;
using Brainf_ck_sharp_UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Enums;
using UICompositionAnimations.Helpers;
using UICompositionAnimations.Helpers.PointerEvents;
using MemoryViewerFlyout = Brainf_ck_sharp_UWP.UserControls.Flyouts.MemoryState.MemoryViewerFlyout;
using SettingsPanelFlyout = Brainf_ck_sharp_UWP.UserControls.Flyouts.Settings.SettingsPanelFlyout;

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

            // Apply the in-app blur on mobile devices
            if (ApiInformationHelper.IsMobileDevice)
            {
                HeaderGrid.Background = XAMLResourcesHelper.GetResourceValue<CustomAcrylicBrush>("HeaderInAppAcrylicBrush");
            }
            else
            {
                // Apply the desired blur effect
                AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.InAppBlurMode), out int blurMode);
                HeaderGrid.Background = XAMLResourcesHelper.GetResourceValue<CustomAcrylicBrush>(blurMode == 0 ? "HeaderHostBackdropBlurBrush" : "HeaderInAppAcrylicBrush");
            }

            // Flyout management
            Messenger.Default.Register<FlyoutOpenedMessage>(this, m => ManageFlyoutUI(true));
            Messenger.Default.Register<FlyoutClosedNotificationMessage>(this, m => ManageFlyoutUI(false));
            Messenger.Default.Register<AppLoadingStatusChangedMessage>(this, m => ManageLoadingUI(m.Loading));
            Messenger.Default.Register<BlurModeChangedMessage>(this, m =>
            {
                if (!ApiInformationHelper.IsMobileDevice)
                {
                    HeaderGrid.Background = XAMLResourcesHelper.GetResourceValue<CustomAcrylicBrush>(m.BlurMode == 0 ? "HeaderHostBackdropBlurBrush" : "HeaderInAppAcrylicBrush");
                }
            });
        }

        public ShellViewModel ViewModel => DataContext.To<ShellViewModel>();

        #region UI

        /// <summary>
        /// Sets whether or not the status bar placeholder for Windows 10 Mobile devices should be displayed
        /// </summary>
        /// <param name="show">The new value for the placeholder visibility</param>
        public void ShowStatusBarPlaceholder(bool show) => StatusBarPlaceholder.Visibility = show.ToVisibility();

        // The current loading popup
        private Popup _LoadingPopup;

        // The semaphore to avoid race conditions with the loading popup
        private readonly SemaphoreSlim LoadingSemaphore = new SemaphoreSlim(1);

        // Manages the loading UI
        private async void ManageLoadingUI(bool loading)
        {
            // Prepare and open a popup to cover the UI while the app is loading
            await LoadingSemaphore.WaitAsync();
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
            LoadingSemaphore.Release();
        }

        // Adjusts the UI when a flyout is displayed in the app
        private void ManageFlyoutUI(bool shown)
        {
            FadeCanvas.IsHitTestVisible = shown;
            FadeCanvas.StartCompositionFadeAnimation(null, shown ? 1 : 0, 250, null, EasingFunctionNames.Linear);
        }

        // The delay in ms for the startup prompts tp display to the user
        private const int StartupPromptsPopupDelay = 1400;

        // Initialize the effects
        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            // UI setup
            FadeCanvas.SetVisualOpacity(0);
            Messenger.Default.Send(new ConsoleStatusUpdateMessage(IDEStatus.Console, LocalizationManager.GetResource("Ready"), 0, 0));
            Console.AdjustTopMargin(HeaderGrid.ActualHeight + 8);
            IDE.AdjustTopMargin(HeaderGrid.ActualHeight);

            // Disable the swipe gestures in the keyboard pivot
            ScrollViewer scroller = CommandsPivot.FindChild<ScrollViewer>();
            if (scroller != null)
            {
                scroller.PointerEntered += Scroller_PointerIn;
                scroller.PointerMoved += Scroller_PointerIn;
                scroller.PointerExited += Scroller_PointerOut;
                scroller.PointerReleased += Scroller_PointerOut;
                scroller.PointerCaptureLost += Scroller_PointerOut;
            }

            // Light border UI
            ExpanderControl.FindChild<Button>("ExpanderStateButton").ManageLightsPointerStates(value =>
            {
                ExpanderLightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 1, 200, null, EasingFunctionNames.Linear);
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.4 : 0, 200, null, EasingFunctionNames.Linear);
            });

            // Popups
            ShowStartupPopups();
        }

        // Shows the startup popups when needed
        private void ShowStartupPopups()
        {
            // Welcome message
            if (AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.WelcomeMessageShown), out bool shown) && !shown)
            {
                // Show the message
                Task.Delay(StartupPromptsPopupDelay).ContinueWith(t =>
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
                Task.Delay(StartupPromptsPopupDelay).ContinueWith(t =>
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
            FlyoutManager.Instance.ShowCustomContextFlyout(keyboard, sender.To<FrameworkElement>(), true);
        }

        // Shows the developer info
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            DevInfoFlyout flyout = new DevInfoFlyout();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("About"), flyout, new Thickness(0), FlyoutDisplayMode.ActualHeight).Forget();
        }

        // Changes the current header blur mode
        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the settings panel
            int
                theme = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme)),
                tabs = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength));
            bool whitespaces = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));
            String font = AppSettingsManager.Instance.GetValue<String>(nameof(AppSettingsKeys.SelectedFontName));
            SettingsPanelFlyout settings = new SettingsPanelFlyout();
            Task.Delay(100).ContinueWith(t => settings.ViewModel.LoadGroups(), TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("Settings"), settings, new Thickness());
            bool
                themeChanged = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme)) != theme,
                tabsChanged = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength)) != tabs,
                fontChanged = AppSettingsManager.Instance.GetValue<String>(nameof(AppSettingsKeys.SelectedFontName))?.Equals(font) != true,
                whitespacesChanged = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces)) != whitespaces;
            if (themeChanged || tabsChanged || fontChanged || whitespacesChanged)
            {
                // UI refresh needed
                if (themeChanged)
                {
                    Messenger.Default.Send(new AppLoadingStatusChangedMessage(true));
                    await Task.Delay(500);
                }
                Messenger.Default.Send(new IDESettingsChangedMessage(themeChanged, tabsChanged, fontChanged, whitespacesChanged));
            }
        }
    }
}
