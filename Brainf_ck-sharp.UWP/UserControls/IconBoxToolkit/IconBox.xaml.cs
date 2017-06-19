using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.IconBoxToolkit
{
    /// <summary>
    /// An SVG image control, credits to Rafael Yousuf
    /// </summary>
    public sealed partial class IconBox : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The data property
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(string), typeof(IconBox), new PropertyMetadata(DependencyProperty.UnsetValue, OnStringDataChanged));

        /// <summary>
        /// The path thickness property
        /// </summary>
        public static readonly DependencyProperty PathThicknessProperty = DependencyProperty.Register(
            nameof(PathThickness), typeof(double), typeof(IconBox), new PropertyMetadata(1d));

        /// <summary>
        /// The data geometry property
        /// </summary>
        public static readonly DependencyProperty DataGeometryProperty = DependencyProperty.Register(nameof(DataGeometry), typeof(PathGeometry), typeof(IconBox), new PropertyMetadata(null));

        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(IconBox), null);

        /// <summary>
        /// The command parameter property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(IconBox), null);

        /// <summary>
        /// The enable greedy click handler property
        /// </summary>
        public static readonly DependencyProperty EnableGreedyClickHandlerProperty = DependencyProperty.Register(nameof(EnableGreedyClickHandler), typeof(bool), typeof(IconBox), null);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data
        {
            get { return GetValue(DataProperty).ToString(); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path thickness.
        /// </summary>
        /// <value>
        /// The path thickness.
        /// </value>
        public double PathThickness
        {
            get { return (double)GetValue(PathThicknessProperty); }
            set { SetValue(PathThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data geometry.
        /// </summary>
        /// <value>
        /// The data geometry.
        /// </value>
        public PathGeometry DataGeometry
        {
            get { return (PathGeometry)GetValue(DataGeometryProperty); }
            set { SetValue(DataGeometryProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>
        /// The command parameter.
        /// </value>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable greedy click handler].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable greedy click handler]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableGreedyClickHandler
        {
            get { return (bool)GetValue(EnableGreedyClickHandlerProperty); }
            set { SetValue(EnableGreedyClickHandlerProperty, value); }
        }
        #endregion

        #region Custom Events

        public delegate void ClickEventHandler(object sender);

        /// <summary>
        /// Occurs when clicked.
        /// </summary>
        public event ClickEventHandler Clicked;

        /// <summary>
        /// Clicked event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void OnClicked(object sender)
        {
            if (Clicked != null && IsEnabled)
                Clicked(sender);
        }

        /// <summary>
        /// Called when [pointer pressed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Input.PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if ((Clicked != null || Command != null) && IsEnabled)
                VisualStateManager.GoToState(this, "Pressed", false);
        }

        /// <summary>
        /// Called when [pointer released].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Input.PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (Command != null && IsEnabled && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);

            OnClicked(sender); //raise the clicked event

            //change the state of this control
            VisualStateManager.GoToState(this, "Normal", false);

            //tell the system this control will be the only handler if the user enabled the greedy click handler
            e.Handled = EnableGreedyClickHandler;
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="IconBox"/> class.
        /// </summary>
        public IconBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when string data changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStringDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<IconBox>().DataGeometry = e.NewValue.ToString().Parse();
        }
    }
}
