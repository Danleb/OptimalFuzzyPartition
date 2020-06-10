using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OptimalFuzzyPartition.ViewModel
{
    public class AlgorithmSettingsViewModel : INotifyPropertyChanged
    {
        public PartitionSettings Settings;
        private Tuple<DensityFunctionType, string> _selectedDensityFunctionType;
        private Tuple<MetricsType, string> _selectedMetricsType;

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
                //IsCenterPlacingTask = true,
                IsCenterPlacingTask = false,
                MaxIterationsCount = 100,
                GridSize = new List<int> { 100, 100 },
                CentersDeltaEpsilon = 0.01d,
                H0 = 1,
                SpaceStretchFactor = 2,

            };

            SelectedDensityFunctionType = DensityFunctionTypes[0];
            SelectedMetricsType = MetricsTypes[0];

            CreatePartitionCommand = new RelayCommand(obj =>
                {
                    Settings.AdditiveCoefficients = AdditiveCoefficients.Select(v => v.Coefficient).ToList();
                    Settings.MultiplicativeCoefficients =
                        MultiplicativeCoefficients.Select(v => v.Coefficient).ToList();
                    Settings.CenterPositions =
                        CenterCoordinates.Select(v => VectorUtils.CreateVector(v.X, v.Y)).ToList();

                    var settingsCopy = Settings.GetCopy();
                    var partitionCreationWindow = new PartitionCreationWindow(settingsCopy);
                    //partitionCreationWindow.ShowActivated = true;
                    partitionCreationWindow.Show();
                },
                obj =>
                {
                    //#TOD add validation check
                    var valid = true;

                    if (!valid)
                    {
                        MessageBox.Show("Введені некоректні дані! Щоб продовжити, введіть коректні дані.", "Некоректні дані",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    return true;
                });
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
                Settings.AdditiveCoefficients.ResizeList(CentersCount);

                OnPropertyChanged();
                OnPropertyChanged(nameof(AdditiveCoefficients));
                OnPropertyChanged(nameof(MultiplicativeCoefficients));
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
                OnPropertyChanged(nameof(ShowCentersCoordinates));
            }
        }

        public Visibility ShowCentersCoordinates => IsCentersPlacingTask ? Visibility.Collapsed : Visibility.Visible;

        public Tuple<DensityFunctionType, string> SelectedDensityFunctionType
        {
            get => _selectedDensityFunctionType;
            set
            {
                _selectedDensityFunctionType = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tuple<DensityFunctionType, string>> DensityFunctionTypes { get; set; } =
            new ObservableCollection<Tuple<DensityFunctionType, string>>
            {
                Tuple.Create(DensityFunctionType.Constant1, "Тотожна одиниця")
            };

        public Tuple<MetricsType, string> SelectedMetricsType
        {
            get => _selectedMetricsType;
            set
            {
                _selectedMetricsType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Tuple<MetricsType, string>> MetricsTypes { get; set; } =
            new ObservableCollection<Tuple<MetricsType, string>>
            {
                Tuple.Create(MetricsType.Euclidean, "Евклідова метрика"),
                Tuple.Create(MetricsType.Manhattan, "Манхеттенська  метрика"),
            };

        public List<CenterCoordinateData> CenterCoordinates { get; set; } = new List<CenterCoordinateData>
        {
            new CenterCoordinateData
            {
                CenterIndex = 0,
                X = 3.33,
                Y = 5
            },
            new CenterCoordinateData
            {
                CenterIndex = 1,
                X = 6.66,
                Y = 5
            }
        };

        public List<CoefficientData> AdditiveCoefficients { get; set; } = new List<CoefficientData>
        {
            new CoefficientData{ CenterIndex = 0, Coefficient = 1 },
            new CoefficientData{ CenterIndex = 1, Coefficient = 2 },
        };

        public List<CoefficientData> MultiplicativeCoefficients { get; set; } = new List<CoefficientData>
        {
            new CoefficientData{ CenterIndex = 0, Coefficient = 2 },
            new CoefficientData{ CenterIndex = 1, Coefficient = 3 },
        };

        public double InitialStepH0
        {
            get => Settings.H0;
            set
            {
                Settings.H0 = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CreatePartitionCommand { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}