using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OptimalFuzzyPartition.ViewModel
{
    public class PartitionCreationViewModel : INotifyPropertyChanged
    {
        public readonly PartitionSettings PartitionSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan TimePassed { get; set; }

        public int PerformedIterationCount { get; set; } = 0;

        public BitmapImage PartitionImage { get; set; }

        public List<CenterCoordinateData> CenterCoordinates { get; set; } = new List<CenterCoordinateData>();

        private readonly DispatcherTimer _timer;

        private SimpleTcpServer _simpleTcpServer;

        public PartitionCreationViewModel(PartitionSettings partitionSettings, SimpleTcpServer simpleTcpServer)
        {
            _simpleTcpServer = simpleTcpServer;
            _simpleTcpServer.DataReceived += OnDataReceived;

            PartitionSettings = partitionSettings;

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);

            //RunPartitionCreation();
        }

        private void OnDataReceived(object sender, Message e)
        {
            if (e.MessageString == "ClientReadyToWork")
            {
                RunPartitionCreation();
            }
        }

        public void RunPartitionCreation()
        {
            _timer.Start();

            var data = new CommandAndData
            {
                CommandType = CommandType.CreateFuzzyPartitionWithoutCentersPlacing,
                PartitionSettings = PartitionSettings
            };

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                var bytes = ms.ToArray();
                _simpleTcpServer.Broadcast(bytes);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            TimePassed += _timer.Interval;
            OnPropertyChanged(nameof(TimePassed));
        }

        private void OnIterationPerformed()
        {


            OnPropertyChanged(nameof(PartitionImage));
            OnPropertyChanged(nameof(PerformedIterationCount));
            OnPropertyChanged(nameof(CenterCoordinates));
        }

        private void OnCalculationsEnd()
        {

        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}