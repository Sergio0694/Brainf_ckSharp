using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ckSharp.Helpers;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    [TemplatePart(Name = "HeaderContentPresenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "BorderElement", Type = typeof(Border))]
    [TemplatePart(Name = "ContentElement", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PlaceholderTextContentPresenter", Type = typeof(TextBlock))]
    public sealed partial class Brainf_ckEditBox
    {
        /// <summary>
        /// The name of the <see cref="ContentPresenter"/> instance for the header
        /// </summary>
        private const string HeaderContentPresenterName = "HeaderContentPresenter";

        /// <summary>
        /// The name of the <see cref="Border"/> instance for the control
        /// </summary>
        private const string BorderElementName = "BorderElement";

        /// <summary>
        /// The name of the <see cref="ScrollViewer"/> instance for the main content
        /// </summary>
        private const string ContentElementName = "ContentElement";

        /// <summary>
        /// The name of the <see cref="TextBlock"/> instance for the placeholder
        /// </summary>
        private const string PlaceholderTextContentPresenterName = "PlaceholderTextContentPresenter";

        /// <summary>
        /// The <see cref="ContentPresenter"/> instance for the header
        /// </summary>
        private ContentPresenter? _HeaderContentPresenter;

        /// <summary>
        /// The <see cref="Border"/> instance for the control
        /// </summary>
        private Border? _BorderElement;

        /// <summary>
        /// The <see cref="ScrollViewer"/> instance for the main content
        /// </summary>
        private ScrollViewer? _ContentElement;

        /// <summary>
        /// The <see cref="TextBlock"/> instance for the placeholder
        /// </summary>
        private TextBlock? _PlaceholderTextContentPresenter;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _HeaderContentPresenter = (ContentPresenter)GetTemplateChild(HeaderContentPresenterName);
            _BorderElement = (Border)GetTemplateChild(BorderElementName);
            _ContentElement = (ScrollViewer)GetTemplateChild(ContentElementName);
            _PlaceholderTextContentPresenter = (TextBlock)GetTemplateChild(PlaceholderTextContentPresenterName);

            Guard.MustBeNotNull(_HeaderContentPresenter, HeaderContentPresenterName);
            Guard.MustBeNotNull(_BorderElement, BorderElementName);
            Guard.MustBeNotNull(_ContentElement, ContentElementName);
            Guard.MustBeNotNull(_PlaceholderTextContentPresenter, PlaceholderTextContentPresenterName);
        }
    }
}
