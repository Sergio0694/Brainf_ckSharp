using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Brainf_ckSharp.Uwp.Controls.Ide
{
    /// <summary>
    /// A class with an attached property for handlers in the <see cref="Brainf_ckEditBox"/> control
    /// </summary>
    internal static class RoutedEventHandlerHelper
    {
        /// <summary>
        /// Gets the value of <see cref="ClickHandlerNameProperty"/> for a given <see cref="Button"/>
        /// </summary>
        /// <param name="element">The input <see cref="Button"/> for which to get the property value</param>
        /// <returns>The value of the <see cref="ClickHandlerNameProperty"/> property for the input <see cref="Button"/> instance</returns>
        public static string GetClickHandlerName(Button element)
        {
            return (string)element.GetValue(ClickHandlerNameProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="ClickHandlerNameProperty"/> for a given <see cref="Button"/>
        /// </summary>
        /// <param name="element">The input <see cref="Button"/> for which to set the property value</param>
        /// <param name="value">The value to set for the <see cref="string"/> property</param>
        public static void SetClickHandlerName(Button element, string value)
        {
            element.SetValue(ClickHandlerNameProperty, value);
        }

        /// <summary>
        /// An attached property that indicates whether a given element has an active blinking animation
        /// </summary>
        public static readonly DependencyProperty ClickHandlerNameProperty = DependencyProperty.RegisterAttached(
            "ClickHandlerName",
            typeof(string),
            typeof(RoutedEventHandlerHelper),
            new PropertyMetadata(DependencyProperty.UnsetValue, OnClickHandlerNamePropertyChanged));

        /// <summary>
        /// Adds the event handler when <see cref="ClickHandlerNameProperty"/> changes
        /// </summary>
        /// <param name="d">The source <see cref="DependencyObject"/> instance</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> info for the current update</param>
        private static void OnClickHandlerNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button @this = (Button)d;
            string name = (string)e.NewValue;

            void Handler(object sender, RoutedEventArgs args)
            {
                // This attached property is needed as methods from the control can't be accessed
                // from items in the context menu from the style. To work around this, the property
                // uses the name of the target method (with no parameters) to dynamically attach
                // an event handler to the Click method. It does so by subscribing once to the Loaded
                // event, to make sure the target button is in the visual tree and the data context is
                // available, then removes that handler and register a proxy Click handler that will
                // use reflection to invoke the target handler, with the supplied name.
                @this.Loaded -= Handler;
                @this.Click += (_, __) =>
                {
                    Brainf_ckEditBox editBox = (Brainf_ckEditBox)((Button)sender).DataContext;

                    editBox.ContextFlyout?.Hide();

                    MethodInfo methodInfo = (
                        from m in typeof(Brainf_ckEditBox).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                        where m.Name == name &&
                              m.GetParameters().Length == 0
                        select m).First();

                    methodInfo.Invoke(editBox, null);
                };
            }

            @this.Loaded += Handler;
        }
    }

}
