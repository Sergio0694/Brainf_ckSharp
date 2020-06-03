using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#nullable enable

namespace Brainf_ckSharp.Uwp.Controls.Host.InputPanel.VirtualKeyboard
{
    /// <summary>
    /// A templated <see cref="Control"/> for a Brainf*ck/PBrain operator
    /// </summary>
    public sealed class OperatorButton : Control
    {
        /// <summary>
        /// Gets or sets the operator for the current control
        /// </summary>
        public char Operator
        {
            get => (char)GetValue(OperatorProperty);
            set => SetValue(OperatorProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Operator"/>
        /// </summary>
        public static readonly DependencyProperty OperatorProperty = DependencyProperty.Register(
            nameof(Operator),
            typeof(char),
            typeof(OperatorButton),
            new PropertyMetadata(default(char)));

        /// <summary>
        /// Gets or sets the description for the current control
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Description"/>
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(OperatorButton),
            new PropertyMetadata(string.Empty));

        /// <inheritdoc cref="Button.Command"/>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// The dependency property for <see cref="Command"/>
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(OperatorButton),
            new PropertyMetadata(null));
    }
}
