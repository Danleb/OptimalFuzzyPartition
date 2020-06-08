using OptimalFuzzyPartitionAlgorithm;
using System.Threading;
using System.Windows;

namespace OptimalFuzzyPartition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Partition _partition;
        private PartitionSettings _partitionSettings;

        private Thread partitionCalculatingThread;
        private Thread ImageCreatingThread;

        public MainWindow(PartitionSettings partitionSettings)
        {
            _partitionSettings = partitionSettings;
            _partition = new Partition(_partitionSettings);
            _partition.OnNextIterationCalculated += OnNextIterationCalculated;

            InitializeComponent();
        }

        private void OnNextIterationCalculated(IterationData iterationData)
        {
            ImageCreatingThread.Abort();
            ImageCreatingThread = new Thread(CreatePartitionView);
            ImageCreatingThread.Start(iterationData);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            partitionCalculatingThread = new Thread(_partition.CreatePartition);
            partitionCalculatingThread.Start();
        }

        private void CreatePartitionView(object obj)
        {
            var iterationData = (IterationData)obj;


        }
    }
}