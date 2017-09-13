using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Brainf_ck_sharp.ReturnTypes;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;

namespace Brainf_ck_sharp_UWP.UserControls.Flyouts.MemoryState
{
    public sealed partial class FunctionDefinitionsViewerControl : UserControl
    {
        public FunctionDefinitionsViewerControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the definitions to display in the control
        /// </summary>
        public IEnumerable<IndexedModelWithValue<FunctionDefinition>> Source
        {
            get => (IEnumerable<IndexedModelWithValue<FunctionDefinition>>)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(IEnumerable<IndexedModelWithValue<FunctionDefinition>>), typeof(FunctionDefinitionsViewerControl), 
            new PropertyMetadata(null, OnSourcePropertyChanged));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.To<FunctionDefinitionsViewerControl>().DefinitionsList.ItemsSource = e.NewValue.To<IEnumerable<IndexedModelWithValue<FunctionDefinition>>>();
        }
    }
}
