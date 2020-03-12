using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Brainf_ckSharp.Uwp.Profiler
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Task.Delay(2000);

            string result = await Brainf_ckBenchmark.RunAsync();

            MarkdownTextBlock.Text = result;
            ProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}
