using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp.MemoryState;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.WindowsAPIs;
using Brainf_ck_sharp_UWP.Messages.Actions;
using Brainf_ck_sharp_UWP.Messages.Flyouts;
using Brainf_ck_sharp_UWP.Messages.IDEStatus;
using Brainf_ck_sharp_UWP.UserControls.Flyouts;
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
                _HeaderEffect?.AdjustSize();
                _KeyboardEffect?.AdjustSize();
            };
            this.InitializeComponent();
            DataContext = new ShellViewModel();
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
        }

        public ShellViewModel ViewModel => DataContext.To<ShellViewModel>();

        #region UI

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
            Console.AdjustTopMargin(HeaderGrid.ActualHeight + 12);
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

        // Sends a message to request to execute the current script
        private void SendPlayRequestMessage(ScriptPlayType type)
        {
            String stdin = StdinHeader.StdinBuffer;
            StdinHeader.ResetStdin();
            Messenger.Default.Send(new PlayScriptMessage(type, stdin));
        }

        public void RequestPlay() => SendPlayRequestMessage(ScriptPlayType.Default);

        public void RequestRepeatLastConsoleScript() => SendPlayRequestMessage(ScriptPlayType.RepeatedCommand);

        public void RequestShowMemoryState()
        {
            IReadonlyTouringMachineState source = Console.ViewModel.State;
            MemoryViewerFlyout viewer = new MemoryViewerFlyout();
            Task.Run(() => IndexedModelWithValue<Brainf_ckMemoryCell>.New(source).ToArray()).ContinueWith(t =>
            {
                viewer.Source = t.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext()).Forget();
            FlyoutManager.Instance.Show(LocalizationManager.GetResource("MemoryStateTitle"), viewer);
        }

        public void RequestClearConsoleLine() => Messenger.Default.Send(new ClearConsoleLineMessage());

        public void RequestUndoConsoleCharacter() => Messenger.Default.Send(new UndoConsoleCharacterMessage());

        public void RequestRestartConsole() => Messenger.Default.Send(new RestartConsoleMessage());

        public void RequestClearScreen() => Messenger.Default.Send(new ClearScreenMessage());

        public void RequestShowUnicodeCharacters()
        {
            UnicodeCharactersGuideFlyout flyout = new UnicodeCharactersGuideFlyout();
            flyout.ViewModel.LoadAsync().Forget();
            FlyoutManager.Instance.Show(LocalizationManager.GetResource("UnicodeTitle"), flyout);
        }

        private void PivotControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = sender.To<Pivot>().SelectedIndex;
            SharedCommandBar.SwitchContent(index == 0);
            Console.ViewModel.IsEnabled = index == 0;
            RestartButton.Visibility = index == 0 ? Visibility.Visible : Visibility.Collapsed;
            RepeatScriptButton.Visibility = index == 0 ? Visibility.Visible : Visibility.Collapsed;
            IDE.ViewModel.IsEnabled = index == 1;
        }
    }
}
