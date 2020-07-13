using System.Windows;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.View
{
    /// <summary>
    /// Interaction logic for AlgorithmSettingsWindow.xaml
    /// </summary>
    public partial class AlgorithmSettingsWindow : Window
    {
        public AlgorithmSettingsWindow()
        {
            InitializeComponent();
        }

        private void OnTextBoxValidationError(object sender, ValidationErrorEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}