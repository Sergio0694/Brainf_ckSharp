using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.Enums;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using Brainf_ck_sharp_UWP.Helpers.Settings;
using Brainf_ck_sharp_UWP.Helpers.UI;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.Messages.IDE;
using Brainf_ck_sharp_UWP.Messages.KeyboardShortcuts;
using Brainf_ck_sharp_UWP.Messages.Requests;
using Brainf_ck_sharp_UWP.Messages.Settings;
using Brainf_ck_sharp_UWP.Messages.UI;
using Brainf_ck_sharp_UWP.PopupService;
using Brainf_ck_sharp_UWP.PopupService.Misc;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.DevInfo;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.MemoryState;
using Brainf_ck_sharp_UWP.UserControls.Flyouts.UserGuide;
using Brainf_ck_sharp_UWP.UserControls.VirtualKeyboard;
using Brainf_ck_sharp_UWP.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
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
            PlaceholderGrid.Visibility = (!ApplicationViewHelper.IsFullScreenOrTabletMode).ToVisibility();
            DataContext = new ShellViewModel(() =>
            {
                string stdin = StdinHeader.StdinBuffer;
                if (AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.ClearStdinBufferOnExecution), out bool reset) && reset)
                {
                    StdinHeader.ResetStdin();
                }
                return stdin;
            });
            Console.ViewModel.IsEnabled = true;

            // Flyout management
            Messenger.Default.Register<FlyoutOpenedMessage>(this, m => ManageFlyoutUI(true));
            Messenger.Default.Register<FlyoutClosedNotificationMessage>(this, m => ManageFlyoutUI(false));
            Messenger.Default.Register<AppLoadingStatusChangedMessage>(this, m => ManageLoadingUI(m.Loading, !m.ImmediateDisplayRequested));

            // Other messages
            Messenger.Default.Register<IDEDisplayRequestMessage>(this, _ => PivotControl.SelectedIndex = 1);
            Messenger.Default.Register<CtrlShortcutPressedMessage>(this, async m =>
            {
                // Skip if there's an open flyout
                if (await FlyoutManager.Instance.IsFlyoutOpenAsync()) return;

                // Play
                if (m.Key == VirtualKey.R && m.Modifiers == VirtualKeyModifiers.Control &&
                    (PivotControl.SelectedIndex == 0 && ViewModel.PlayAvailable ||
                     PivotControl.SelectedIndex == 1 && ViewModel.IDECodeAvailable))
                {
                    ViewModel.RequestPlay(); // Request to play a console script or to execute the code in the IDE
                }
                else if (m.Key == VirtualKey.R &&
                         m.Modifiers == (VirtualKeyModifiers.Control | VirtualKeyModifiers.Menu) &&
                         ViewModel.DebugAvailable)
                {
                    ViewModel.RequestDebug();
                }

                // Save
                else if (m.Key == VirtualKey.S && m.Modifiers == VirtualKeyModifiers.Control && ViewModel.SaveAvailable)
                {
                    Messenger.Default.Send(new SaveSourceCodeRequestMessage(CodeSaveType.Save));
                }
                else if (m.Key == VirtualKey.S &&
                         m.Modifiers == (VirtualKeyModifiers.Control | VirtualKeyModifiers.Menu) &&
                         ViewModel.SaveAsAvailable)
                {
                    Messenger.Default.Send(new SaveSourceCodeRequestMessage(CodeSaveType.SaveAs));
                }

                // Misc
                else if (m.Key == VirtualKey.U && m.Modifiers == VirtualKeyModifiers.Control) RequestShowUnicodeCharacters();
                else if (m.Key == VirtualKey.M && m.Modifiers == VirtualKeyModifiers.Control && PivotControl.SelectedIndex == 0) RequestShowMemoryState();
                else if (m.Key == VirtualKey.L && m.Modifiers == VirtualKeyModifiers.Control && PivotControl.SelectedIndex == 1) RequestShowCodeLibrary();
                else if (m.Key == VirtualKey.I && m.Modifiers == VirtualKeyModifiers.Control) RequestShowSettingsPanel();
            });
            Messenger.Default.Register<BackgroundExecutionInputRequestMessage>(this, m =>
            {
                m.ReportResult((
                    PivotControl.SelectedIndex == 0 ? Console.SourceCode : IDE.SourceCode,
                    StdinHeader.StdinBuffer,
                    PivotControl.SelectedIndex == 0 ? Console.ViewModel.State : TouringMachineStateProvider.Initialize(64)));
            });
        }

        public ShellViewModel ViewModel => DataContext.To<ShellViewModel>();

        #region UI

        // The current loading popup
        private Popup _LoadingPopup;

        // The semaphore to avoid race conditions with the loading popup
        private readonly SemaphoreSlim LoadingSemaphore = new SemaphoreSlim(1);

        // Manages the loading UI
        private async void ManageLoadingUI(bool loading, bool animate)
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
                if (animate)
                {
                    control.SetVisualOpacity(0);
                    popup.IsOpen = true;
                    control.StartCompositionFadeAnimation(null, 1, 200, null, EasingFunctionNames.Linear);
                }
                else popup.IsOpen = true;
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
            // Realtime UI adjustment
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += (view, _) =>
            {
                Visibility visibility = (!ApplicationViewHelper.IsFullScreenOrTabletMode).ToVisibility();
                if (PlaceholderGrid.Visibility == visibility) return;
                PlaceholderGrid.Visibility = visibility;
                UpdateUIElementsSizeBindings();
                IDE.RefreshUI();
            };

            // UI setup
            FadeCanvas.SetVisualOpacity(0);
            Messenger.Default.Send(new ConsoleStatusUpdateMessage(IDEStatus.Console, LocalizationManager.GetResource("Ready"), 0, 0));
            UpdateUIElementsSizeBindings();

            // Light border UI
            ExpanderControl.FindChild<Button>("ExpanderStateButton").ManageLightsPointerStates(value =>
            {
                ExpanderLightBorder.StartXAMLTransformFadeAnimation(null, value ? 0 : 1, 200, null, EasingFunctionNames.Linear);
                BackgroundBorder.StartXAMLTransformFadeAnimation(null, value ? 0.4 : 0, 200, null, EasingFunctionNames.Linear);
            });

            // Popups
            if (!_StartupMessagesProcessed)
            {
                _StartupMessagesProcessed = true;
                ShowStartupPopups();
            }

            // Starting page
            if (AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.StartingPage)) == 1)
            {
                _SkipInitialAnimation = true;
                PivotControl.SelectedIndex = 1;
            }
        }

        // Updates the size of the UI elements that rely on the actual window size
        private void UpdateUIElementsSizeBindings()
        {
            HeaderGrid.Measure(new Size(ActualWidth, double.PositiveInfinity));
            double height = HeaderGrid.DesiredSize.Height;
            Console.AdjustTopMargin(height + 8);
            IDE.AdjustTopMargin(height);
        }

        // Indicates whether or not to skip the first page switch animation
        private bool _SkipInitialAnimation;

        // Local field to keep track of the calls to the method below
        private bool _StartupMessagesProcessed;

        // Shows the startup popups when needed
        private void ShowStartupPopups()
        {
            // Welcome message
            if (AppSettingsManager.Instance.TryGetValue(nameof(AppSettingsKeys.WelcomeMessageShown), out bool shown) && !shown)
            {
                // Show the message
                Task.Delay(StartupPromptsPopupDelay).ContinueWith(async t =>
                {
                    WelcomeMessageFlyout welcome = new WelcomeMessageFlyout();
                    FlyoutResult result = await FlyoutManager.Instance.ShowAsync($"{LocalizationManager.GetResource("WelcomeTitle")} 😄", welcome, 
                        LocalizationManager.GetResource("UserGuide"), new Thickness(12, 12, 16, 12), FlyoutDisplayMode.ActualHeight);
                    if (result == FlyoutResult.Confirmed)
                    {
                        // Show the user guide
                        UserGuideViewerControl guide = new UserGuideViewerControl();
                        FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("UserGuide"), guide, null, new Thickness()).Forget();
                    }
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
                    FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("HowsItGoing"), reviewFlyout, null,
                        new Thickness(0, 12, 0, 0), FlyoutDisplayMode.ActualHeight).Forget();
                }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();

                // Update the setting
                AppSettingsManager.Instance.SetValue(nameof(AppSettingsKeys.ReviewPromptShown), true, SettingSaveMode.OverwriteIfExisting);
            }
        }

        #endregion

        // Updates the UI and the view models when the user changes the current page
        private void PivotControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sender.To<Pivot>().SelectedIndex;
            if (index == 1 && _SkipInitialAnimation)
            {
                SharedCommandBar.SwitchContent(index == 0);
                _SkipInitialAnimation = false;
            }
            else SharedCommandBar.SwitchContentAsync(index == 0);
            Console.ViewModel.IsEnabled = index == 0;
            IDE.ViewModel.IsEnabled = index == 1;

            // UI adjustments on page change
            if (index == 1)
            {
                StdinHeader.SetMemoryViewButtonIsEnabledProperty(false);
                if (CommandsPivot.SelectedIndex == 1) CommandsPivot.SelectedIndex = 0;
            }
            else StdinHeader.SetMemoryViewButtonIsEnabledProperty(true);
        }

        /// <summary>
        /// Shows the current console memory state in a flyout
        /// </summary>
        public void RequestShowMemoryState()
        {
            IReadonlyTouringMachineState source = Console.ViewModel.State;
            if (Console.ViewModel.Functions.Count == 0)
            {
                // No available functions
                MemoryViewerFlyout viewer = new MemoryViewerFlyout();
                FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("MemoryStateTitle"), viewer).Forget();
                Task.Delay(100).ContinueWith(t => viewer.ViewModel.InitializeAsync(source), TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                // Full memory viewer
                ConsoleFullMemoryViewerControl fullViewer = new ConsoleFullMemoryViewerControl(source, Console.ViewModel.Functions);
                FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("CurrentState"), fullViewer, null, new Thickness()).Forget();
                Task.Delay(100).ContinueWith(t => fullViewer.ViewModel.InitializeAsync(), TaskScheduler.FromCurrentSynchronizationContext());
            }
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
            SourceCode loaded = IDE.ViewModel.CategorizedCode?.Type == SavedSourceCodeType.Sample ? null : IDE.ViewModel.LoadedCode;
            LocalSourceCodesBrowserFlyout flyout = new LocalSourceCodesBrowserFlyout(loaded);
            FlyoutClosedResult<CategorizedSourceCode> result = await FlyoutManager.Instance.ShowAsync<LocalSourceCodesBrowserFlyout, CategorizedSourceCode>(
                LocalizationManager.GetResource("CodeLibrary"), flyout, new Thickness(), openCallback: () => flyout.ViewModel.LoadGroupsAsync().Forget());
            if (result) Messenger.Default.Send(new SourceCodeLoadingRequestedMessage(result.Value, SavedCodeLoadingSource.CodeLibrary));
        }

        // Shows the small navigation keyboard popup
        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            VirtualArrowsKeyboardControl keyboard = new VirtualArrowsKeyboardControl();
            FlyoutManager.Instance.ShowCustomContextFlyout(keyboard, sender.To<FrameworkElement>(), true).Forget();
        }

        // Shows the developer info
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            DevInfoFlyout flyout = new DevInfoFlyout();
            FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("About"), flyout, null, new Thickness(0), FlyoutDisplayMode.ActualHeight).Forget();
        }

        // Displays the settings when the user taps the button
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => RequestShowSettingsPanel();

        /// <summary>
        /// Shows the settings window to the user
        /// </summary>
        private async void RequestShowSettingsPanel()
        {
            // Show the settings panel
            int
                theme = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme)),
                tabs = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength));
            bool whitespaces = AppSettingsManager.Instance.GetValue<bool>(nameof(AppSettingsKeys.RenderWhitespaces));
            string font = AppSettingsManager.Instance.GetValue<string>(nameof(AppSettingsKeys.SelectedFontName));
            SettingsPanelFlyout settings = new SettingsPanelFlyout();
            Task.Delay(100).ContinueWith(t => settings.ViewModel.LoadGroups(), TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            await FlyoutManager.Instance.ShowAsync(LocalizationManager.GetResource("Settings"), settings, null, new Thickness());
            bool
                themeChanged = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.SelectedIDETheme)) != theme,
                tabsChanged = AppSettingsManager.Instance.GetValue<int>(nameof(AppSettingsKeys.TabLength)) != tabs,
                fontChanged = AppSettingsManager.Instance.GetValue<string>(nameof(AppSettingsKeys.SelectedFontName))?.Equals(font) != true,
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
