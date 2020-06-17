﻿using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartition.Model;
using OptimalFuzzyPartition.View;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OptimalFuzzyPartition.ViewModel
{
    public class AlgorithmSettingsViewModel : INotifyPropertyChanged
    {
        public readonly PartitionSettings Settings;

        public event PropertyChangedEventHandler PropertyChanged;

        private PartitionCreationWindow _partitionCreationWindow;

        public AlgorithmSettingsViewModel()
        {
            Settings = DefaultSettingsKeeper.GetPartitionSettings();

            CreatePartitionCommand = new RelayCommand(obj =>
                {
                    if (_partitionCreationWindow == null)
                    {
                        _partitionCreationWindow = new PartitionCreationWindow(Settings);
                    }

                    _partitionCreationWindow.Show();
                },
                obj =>
                {
                    //#TODO add validation check
                    var valid = true;

                    if (!valid)
                    {
                        MessageBox.Show("Введені некоректні дані! Щоб продовжити, введіть коректні дані.", "Некоректні дані",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    return true;
                });
        }

        #region Settings for the: space

        public double MinX
        {
            get => Settings.SpaceSettings.MinCorner[0];
            set
            {
                Settings.SpaceSettings.MinCorner[0] = value;
                OnPropertyChanged();
            }
        }

        public double MaxX
        {
            get => Settings.SpaceSettings.MaxCorner[0];
            set
            {
                Settings.SpaceSettings.MaxCorner[0] = value;
                OnPropertyChanged();
            }
        }

        public double MinY
        {
            get => Settings.SpaceSettings.MinCorner[1];
            set
            {
                Settings.SpaceSettings.MinCorner[1] = value;
                OnPropertyChanged();
            }
        }

        public double MaxY
        {
            get => Settings.SpaceSettings.MaxCorner[1];
            set
            {
                Settings.SpaceSettings.MaxCorner[1] = value;
                OnPropertyChanged();
            }
        }

        public int SegmentsCountX
        {
            get => Settings.SpaceSettings.GridSize[0];
            set
            {
                Settings.SpaceSettings.GridSize[0] = value;
                OnPropertyChanged();
            }
        }

        public int SegmentsCountY
        {
            get => Settings.SpaceSettings.GridSize[1];
            set
            {
                Settings.SpaceSettings.GridSize[1] = value;
                OnPropertyChanged();
            }
        }

        public DensityType[] DensityTypes => new[]
        {
            DensityType.Everywhere1
        };

        public DensityType DensityType
        {
            get => Settings.SpaceSettings.DensityType;
            set
            {
                Settings.SpaceSettings.DensityType = value;
                OnPropertyChanged();
            }
        }

        public MetricsType[] MetricsTypes => new[]
        {
            MetricsType.Euclidean,
            MetricsType.Manhattan,
            MetricsType.Chebyshev,
            MetricsType.CustomFunction
        };

        public MetricsType MetricsType
        {
            get => Settings.SpaceSettings.MetricsType;
            set
            {
                Settings.SpaceSettings.MetricsType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Settings for the: centers

        public int CentersCount
        {
            get => Settings.CentersSettings.CentersCount;
            set
            {
                Settings.CentersSettings.CentersCount = value;
                Settings.CentersSettings.CenterDatas.ResizeList(CentersCount, () => new CenterData
                {
                    IsFixed = true,
                    Position = (Settings.SpaceSettings.MaxCorner + Settings.SpaceSettings.MinCorner) / 2,
                    A = 0,
                    W = 1
                });//???

                OnPropertyChanged();
                OnPropertyChanged(nameof(CenterDatas));
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

        public List<CenterData> CenterDatas => Settings.CentersSettings.CenterDatas;

        #endregion

        #region Settings for the: fuzzy partition with placing centers

        public int GaussLegendreIntegralOrder
        {
            get => Settings.FuzzyPartitionPlacingCentersSettings.GaussLegendreIntegralOrder;
            set
            {
                Settings.FuzzyPartitionPlacingCentersSettings.GaussLegendreIntegralOrder = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Settings for the R-algorithm:

        public int RAlgorithmMaxIterationsCount
        {
            get => Settings.RAlgorithmSettings.MaxIterationsCount;
            set
            {
                Settings.RAlgorithmSettings.MaxIterationsCount = value;
                OnPropertyChanged();
            }
        }

        public double InitialStepH0
        {
            get => Settings.RAlgorithmSettings.H0;
            set
            {
                Settings.RAlgorithmSettings.H0 = value;
                OnPropertyChanged();
            }
        }

        public double SpaceStretchFactor
        {
            get => Settings.RAlgorithmSettings.SpaceStretchFactor;
            set
            {
                Settings.RAlgorithmSettings.SpaceStretchFactor = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Settings for the: Fuzzy partition with fixed centers

        public double FixedPartitionGradientStep
        {
            get => Settings.FuzzyPartitionFixedCentersSettings.GradientStep;
            set
            {
                Settings.FuzzyPartitionFixedCentersSettings.GradientStep = value;
                OnPropertyChanged();
            }
        }

        public double FixedPartitionGradientThreshold
        {
            get => Settings.FuzzyPartitionFixedCentersSettings.GradientEpsilon;
            set
            {
                Settings.FuzzyPartitionFixedCentersSettings.GradientEpsilon = value;
                OnPropertyChanged();
            }
        }

        public int FixedPartitionMaxIteration
        {
            get => Settings.FuzzyPartitionFixedCentersSettings.MaxIterationsCount;
            set
            {
                Settings.FuzzyPartitionFixedCentersSettings.MaxIterationsCount = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CreatePartitionCommand { get; }

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}