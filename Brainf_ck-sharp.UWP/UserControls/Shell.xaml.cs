using System;
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

        private void ManageFlyoutUI(bool shown)
        {
            FadeCanvas.IsHitTestVisible = shown;
            FadeCanvas.StartCompositionFadeAnimation(null, shown ? 1 : 0, 250, null, EasingFunctionNames.Linear);
        }

        public ShellViewModel ViewModel => DataContext.To<ShellViewModel>();

        // Acrylic brush for the header
        private AttachedStaticCompositionEffect<Border> _HeaderEffect;

        // Acrylic brush for the virtual keyboard
        private AttachedStaticCompositionEffect<Border> _KeyboardEffect;

        // Initialize the effects
        private async void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            // UI setup
            FadeCanvas.SetVisualOpacity(0);
            Messenger.Default.Send(new IDEStatusUpdateMessage(IDEStatus.Console, "Ready", 0, 0, String.Empty));
            Console.AdjustTopMargin(HeaderGrid.ActualHeight + 12);
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

        private void Scroller_PointerIn(object sender, PointerRoutedEventArgs e)
        {
            sender.To<ScrollViewer>().HorizontalScrollMode = ScrollMode.Disabled;
        }

        private void Scroller_PointerOut(object sender, PointerRoutedEventArgs e)
        {
            sender.To<ScrollViewer>().HorizontalScrollMode = ScrollMode.Enabled;
        }

        public void RequestPlay()
        {
            String stdin = StdinHeader.StdinBuffer;
            StdinHeader.ResetStdin();
            Messenger.Default.Send(new PlayScriptMessage(stdin));
        }

        public void RequestShowMemoryState()
        {
            MemoryViewerFlyout viewer = new MemoryViewerFlyout
            {
                Source = IndexedModelWithValue<Brainf_ckMemoryCell>.New(Console.ViewModel.State)
            };
            FlyoutManager.Instance.Show(LocalizationManager.GetResource("MemoryStateTitle"), viewer);
        }

        public void RequestClearConsoleLine() => Messenger.Default.Send(new ClearConsoleLineMessage());

        public void RequestUndoConsoleCharacter() => Messenger.Default.Send(new UndoConsoleCharacterMessage());

        public void RequestRestartConsole() => Messenger.Default.Send(new RestartConsoleMessage());

        public void RequestClearScreen() => Messenger.Default.Send(new ClearScreenMessage());
    }
}
