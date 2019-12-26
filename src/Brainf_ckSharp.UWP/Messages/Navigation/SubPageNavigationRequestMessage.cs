using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.UWP.Messages.Navigation
{
    /// <summary>
    /// A message that signals a request to display a page in the sub page host
    /// </summary>
    public sealed class SubPageNavigationRequestMessage
    {
        /// <summary>
        /// Gets the page to display
        /// </summary>
        public UserControl SubPage { get; }

        /// <summary>
        /// Creates a new <see cref="SubPageNavigationRequestMessage"/> instance with the specified parameters
        /// </summary>
        /// <param name="subPage">The page to display</param>
        private SubPageNavigationRequestMessage(UserControl subPage) => SubPage = subPage;

        /// <summary>
        /// Creates a new request message to the target sub page type
        /// </summary>
        /// <param name="subPage">The page to display</param>
        public static SubPageNavigationRequestMessage To(UserControl subPage) => new SubPageNavigationRequestMessage(subPage);

        /// <summary>
        /// Creates a new request message to the target sub page type
        /// </summary>
        /// <typeparam name="T">The type of page to display</typeparam>
        public static SubPageNavigationRequestMessage To<T>() where T : UserControl, new() => new SubPageNavigationRequestMessage(new T());
    }
}
