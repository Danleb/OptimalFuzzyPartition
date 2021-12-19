using Microsoft.WindowsAPICodePack.Dialogs;
using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartition.Properties;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
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

        private Stopwatch _timePassStopWatch;
        private string _lastPartitionImageSavePath;
        private bool _isManualSavePathEnabled;
        private string _partitionImageSavePath;
        private readonly CommandAndData _commandAndData = new CommandAndData
        {
            RenderingSettings = new RenderingSettings
            {
                BorderWidth = 3
            }
        };

        public PartitionCreationViewModel(PartitionSettings partitionSettings, SimpleTcpServer simpleTcpServer)
        {
            _simpleTcpServer = simpleTcpServer;
            _simpleTcpServer.DataReceived += OnDataReceived;

            PartitionSettings = partitionSettings;

            _ = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);

            AlwaysShowCentersInfo = Settings.Default.AlwaysShowCentersInfo;
            DrawGrayscale = Settings.Default.DrawGrayscalePartition;
            DrawWithMistrust = Settings.Default.DrawPartitionWithMistrustCoef;
            MistrustCoefficient = Settings.Default.MistrustCoef;
            PartitionImageSavePath = Settings.Default.PartitionImageSavePath;

            ChoosePartitionImageSavePathCommand = new RelayCommand(v => ChoosePartitionImageSavePath());
            RunPartitionCreationCommand = new RelayCommand(v => RunPartitionCreation());
            SavePartitionImageCommand = new RelayCommand(v => SavePartitionImage());
            UpdatePartitionCommand = new RelayCommand(v => UpdatePartition());

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        public TimeSpan TimePassed { get; set; }

        #region Render settings

        public bool AlwaysShowCentersInfo
        {
            get => _commandAndData.RenderingSettings.AlwaysShowCentersInfo;
            set
            {
                _commandAndData.RenderingSettings.AlwaysShowCentersInfo = value;
                Settings.Default.AlwaysShowCentersInfo = _commandAndData.RenderingSettings.AlwaysShowCentersInfo;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool DrawGrayscale
        {
            get => _commandAndData.RenderingSettings.DrawGrayscale;
            set
            {
                _commandAndData.RenderingSettings.DrawGrayscale = value;
                Settings.Default.DrawGrayscalePartition = _commandAndData.RenderingSettings.DrawGrayscale;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool DrawWithMistrust
        {
            get => _commandAndData.RenderingSettings.DrawWithMistrustCoefficient;
            set
            {
                _commandAndData.RenderingSettings.DrawWithMistrustCoefficient = value;
                Settings.Default.DrawPartitionWithMistrustCoef = _commandAndData.RenderingSettings.DrawWithMistrustCoefficient;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public double MistrustCoefficient
        {
            get => _commandAndData.RenderingSettings.MistrustCoefficient;
            set
            {
                _commandAndData.RenderingSettings.MistrustCoefficient = value;
                Settings.Default.MistrustCoef = _commandAndData.RenderingSettings.MistrustCoefficient;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        #endregion

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
            get => _commandAndData.RenderingSettings.IterationNumber;
            set
            {
                _commandAndData.RenderingSettings.IterationNumber = value;
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

        public bool IsManualSavePathEnabled
        {
            get => _isManualSavePathEnabled; set
            {
                _isManualSavePathEnabled = value;
                OnPropertyChanged();
            }
        }

        public string PartitionImageSavePath
        {
            get => _partitionImageSavePath;
            set
            {
                _partitionImageSavePath = value;
                OnPropertyChanged();
            }
        }

        public List<CenterData> CenterCoordinates { get; set; } = new List<CenterData>();

        public RelayCommand RunPartitionCreationCommand { get; }

        public RelayCommand SavePartitionImageCommand { get; }

        public RelayCommand UpdatePartitionCommand { get; }

        public RelayCommand ChoosePartitionImageSavePathCommand { get; }

        private void ChoosePartitionImageSavePath()
        {
            var directory = (_lastPartitionImageSavePath == null || string.IsNullOrEmpty(_lastPartitionImageSavePath)) ? Environment.CurrentDirectory : _lastPartitionImageSavePath;

            var commonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                DefaultDirectory = directory
            };

            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                directory = commonOpenFileDialog.FileName;
                _lastPartitionImageSavePath = directory;
                PartitionImageSavePath = directory;

                Properties.Settings.Default.PartitionImageSavePath = directory;
                Properties.Settings.Default.Save();
            }
        }

        private void SavePartitionImage()
        {
            _commandAndData.CommandType = CommandType.SavePartitionImage;

            if (IsManualSavePathEnabled && PartitionImageSavePath != null)
            {
                _commandAndData.ImageSavePath = Encoding.UTF8.GetBytes(PartitionImageSavePath);
            }
            else
            {
                _commandAndData.ImageSavePath = null;
            }

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
                PerformedIterationCount = data.PerformedIterationsCount;

                if (data.WorkFinished)
                {
                    _timer.Stop();
                    _timePassStopWatch.Stop();
                    TimePassed = TimeSpan.FromMilliseconds(_timePassStopWatch.ElapsedMilliseconds);
                    OnPropertyChanged(nameof(TimePassed));
                }
            }
        }

        public void RunPartitionCreation()
        {
            TimePassed = TimeSpan.Zero;
            _timer.Start();
            _timePassStopWatch = new Stopwatch();
            _timePassStopWatch.Start();

            _commandAndData.CommandType = CommandType.CreateFuzzyPartition;
            _commandAndData.PartitionSettings = PartitionSettings;

            _simpleTcpServer.Broadcast(_commandAndData.ToBytes());
        }

        public void UpdatePartition()
        {
            _commandAndData.CommandType = CommandType.ShowCurrentPartitionWithSettings;
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