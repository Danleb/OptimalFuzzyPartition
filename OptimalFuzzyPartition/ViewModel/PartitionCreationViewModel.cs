using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace OptimalFuzzyPartition.ViewModel
{
    public class PartitionCreationViewModel : INotifyPropertyChanged
    {
        public readonly PartitionSettings PartitionSettings;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly DispatcherTimer _timer;
        private readonly SimpleTcpServer _simpleTcpServer;
        private int _performedIterationCount = 0;
        private double _targetFunctionalValue;
        private double _dualFunctionalValue;

        private readonly CommandAndData _commandAndData = new CommandAndData();

        public TimeSpan TimePassed { get; set; }

        //public bool CentersPlacingTask => PartitionSettings.IsCenterPlacingTask;

        public int PerformedIterationCount
        {
            get => _performedIterationCount;
            set
            {
                _performedIterationCount = value;
                OnPropertyChanged();
            }
        }

        public int IterationNumberToInspect
        {
            get => _commandAndData.IterationNumber;
            set
            {
                _commandAndData.IterationNumber = value;
                OnPropertyChanged();
            }
        }

        public double TargetFunctionalValue
        {
            get => _targetFunctionalValue;
            set
            {
                _targetFunctionalValue = value;
                OnPropertyChanged();
            }
        }

        public double DualFunctionalValue
        {
            get => _dualFunctionalValue;
            set
            {
                _dualFunctionalValue = value;
                OnPropertyChanged();
            }
        }

        public bool DrawWithMistrust
        {
            get => _commandAndData.DrawWithMistrustCoefficient;
            set
            {
                _commandAndData.DrawWithMistrustCoefficient = value;
                OnPropertyChanged();
            }
        }

        public double MistrustCoefficient
        {
            get => _commandAndData.MistrustCoefficient;
            set
            {
                _commandAndData.MistrustCoefficient = value;
                OnPropertyChanged();
            }
        }

        public bool AlwaysShowCentersInfo
        {
            get => _commandAndData.AlwaysShowCentersInfo;
            set
            {
                _commandAndData.AlwaysShowCentersInfo = value;
                OnPropertyChanged();
            }
        }

        public List<CenterData> CenterCoordinates { get; set; } = new List<CenterData>();

        public RelayCommand RunPartitionCreationCommand { get; set; }

        public RelayCommand SavePartitionImageCommand { get; set; }

        public PartitionCreationViewModel(PartitionSettings partitionSettings, SimpleTcpServer simpleTcpServer)
        {
            _simpleTcpServer = simpleTcpServer;
            _simpleTcpServer.DataReceived += OnDataReceived;

            PartitionSettings = partitionSettings;

            RunPartitionCreationCommand = new RelayCommand(v => RunPartitionCreation());
            SavePartitionImageCommand = new RelayCommand(v => SavePartitionImage());

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        private void SavePartitionImage()
        {
            _commandAndData.CommandType = CommandType.SavePartitionImage;
            _commandAndData.ImageSavePath = null;
            _simpleTcpServer.Broadcast(_commandAndData.ToBytes());
        }

        private void OnDataReceived(object sender, Message e)
        {
            if (e.MessageString == "ClientReadyToWork")
            {
                RunPartitionCreation();
            }
            else
            {
                var data = e.Data.ConvertTo<PartitionResult>();
                TargetFunctionalValue = data.TargetFunctionalValue;
                DualFunctionalValue = data.DualFunctionalValue;

                if (data.WorkFinished)
                {
                    _timer.Stop();
                }
            }
        }

        public void RunPartitionCreation()
        {
            TimePassed = TimeSpan.Zero;
            _timer.Start();

            _commandAndData.CommandType = CommandType.CreateFuzzyPartition;
            _commandAndData.PartitionSettings = PartitionSettings;

            _simpleTcpServer.Broadcast(_commandAndData.ToBytes());
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            TimePassed += _timer.Interval;
            OnPropertyChanged(nameof(TimePassed));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}