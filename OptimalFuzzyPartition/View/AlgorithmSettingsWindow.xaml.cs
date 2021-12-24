using OptimalFuzzyPartition.ViewModel;
using System.ComponentModel;
using System.Windows;

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
            App.LanguageChanged += App_LanguageChanged;
        }

        ~AlgorithmSettingsWindow()
        {
            App.LanguageChanged -= App_LanguageChanged;
        }

        private AlgorithmSettingsViewModel ViewModel => (AlgorithmSettingsViewModel)DataContext;

        private void App_LanguageChanged(object sender, System.EventArgs e)
        {
            // Combobox selected item text doesn't get updated when language is changed,
            // so here is workaround with force reload.
            DensityCombobox.SelectedItem = null;
            DensityCombobox.ItemsSource = null;
            DensityCombobox.ItemsSource = ViewModel.DensityTypes;
            DensityCombobox.SelectedItem = ViewModel.DensityType;

            MetricsCombobox.SelectedItem = null;
            MetricsCombobox.ItemsSource = null;
            MetricsCombobox.ItemsSource = ViewModel.MetricsTypes;
            MetricsCombobox.SelectedItem = ViewModel.MetricsType;
        }

        private void AlgorithmSettingsWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ((AlgorithmSettingsViewModel)DataContext).OnClosing.Execute(null);
        }
    }
}
