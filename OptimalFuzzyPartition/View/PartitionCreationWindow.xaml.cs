﻿using OptimalFuzzyPartition.Model;
using OptimalFuzzyPartition.ViewModel;
using OptimalFuzzyPartitionAlgorithm;
using System.Windows;
using System.Windows.Input;

namespace OptimalFuzzyPartition.View
{
    /// <summary>
    /// Interaction logic for PartitionCreationWindow.xaml
    /// </summary>
    public partial class PartitionCreationWindow : Window
    {
        private readonly PartitionSettings _partitionSettings;

        public PartitionCreationWindow()
        {
            _partitionSettings = DefaultSettingsBuilder.GetPartitionSettings();
        }

        public PartitionCreationWindow(PartitionSettings partitionSettings)
        {
            _partitionSettings = partitionSettings;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new PartitionCreationViewModel(_partitionSettings, UnityWindowHost.SimpleTcpServer);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            //UnityWindowHost.Destroy();
        }
    }
}