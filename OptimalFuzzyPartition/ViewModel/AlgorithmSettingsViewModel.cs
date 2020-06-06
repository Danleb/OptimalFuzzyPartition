using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;

namespace OptimalFuzzyPartition.ViewModel
{
    public class AlgorithmSettingsViewModel : INotifyPropertyChanged
    {
        public PartitionSettings Settings;

        public event PropertyChangedEventHandler PropertyChanged;

        public AlgorithmSettingsViewModel()
        {
            Settings = new PartitionSettings
            {
                MinCorner = VectorUtils.CreateVector(0, 0),
                MaxCorner = VectorUtils.CreateVector(10, 10),
                CentersCount = 2,
                AdditiveCoefficients = new List<double> { 0, 0 },
                MultiplicativeCoefficients = new List<double> { 1, 1 },
                IsCenterPlacingTask = true,
                MaxIterationsCount = 100,
                GridSize = new List<int> { 100, 100 },
                CentersDeltaEpsilon = 0.01d,
                H0 = 1
            };
        }

        public double MinX
        {
            get => Settings.MinCorner[0];
            set
            {
                Settings.MinCorner[0] = value;
                OnPropertyChanged();
            }
        }

        public double MaxX
        {
            get => Settings.MaxCorner[0];
            set
            {
                Settings.MaxCorner[0] = value;
                OnPropertyChanged();
            }
        }

        public double MinY
        {
            get => Settings.MinCorner[1];
            set
            {
                Settings.MinCorner[1] = value;
                OnPropertyChanged();
            }
        }

        public double MaxY
        {
            get => Settings.MaxCorner[1];
            set
            {
                Settings.MaxCorner[1] = value;
                OnPropertyChanged();
            }
        }

        public int SegmentsCountX
        {
            get => Settings.GridSize[0];
            set
            {
                Settings.GridSize[0] = value;
                OnPropertyChanged();
            }
        }

        public int SegmentsCountY
        {
            get => Settings.GridSize[1];
            set
            {
                Settings.GridSize[1] = value;
                OnPropertyChanged();
            }
        }

        public int CentersCount
        {
            get => Settings.CentersCount;
            set
            {
                Settings.CentersCount = value;
                OnPropertyChanged();
            }
        }

        public int MaxIterationsCount
        {
            get => Settings.MaxIterationsCount;
            set
            {
                Settings.MaxIterationsCount = value;
                OnPropertyChanged();
            }
        }

        public double SpaceStretchFactor
        {
            get => Settings.SpaceStretchFactor;
            set
            {
                Settings.SpaceStretchFactor = value;
                OnPropertyChanged();
            }
        }

        public bool IsCentersPlacingTask
        {
            get => Settings.IsCenterPlacingTask;
            set
            {
                Settings.IsCenterPlacingTask = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}