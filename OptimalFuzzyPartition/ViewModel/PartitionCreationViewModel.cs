using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading;
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

        private AnonymousPipeServerStream _pipeServerIn;
        private AnonymousPipeServerStream _pipeServerOut;

        private Thread _listeningThread;

        public PartitionCreationViewModel(PartitionSettings partitionSettings, AnonymousPipeServerStream pipeServerIn, AnonymousPipeServerStream pipeServerOut)
        {
            PartitionSettings = partitionSettings;

            _pipeServerIn = pipeServerIn;
            _pipeServerOut = pipeServerOut;

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            //_listeningThread = new Thread();

            RunPartitionCreation();
        }

        public void RunPartitionCreation()
        {
            _timer.Start();

            var data = new CommandAndData
            {
                CommandType = CommandType.CreateFuzzyPartitionWithoutCentersPlacing,
                PartitionSettings = PartitionSettings
            };

            PipesMessaging.SendObject(_pipeServerOut, data);
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