using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartition.Model;
using OptimalFuzzyPartition.View;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm.Common;
using OptimalFuzzyPartitionAlgorithm.Settings;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace OptimalFuzzyPartition.ViewModel
{
    public class AlgorithmSettingsViewModel : INotifyPropertyChanged
    {
        private string _configurationSavePath;
        private PartitionSettings _settings;
        private PartitionCreationWindow _partitionCreationWindow;

        public AlgorithmSettingsViewModel()
        {
            Settings = DefaultSettingsBuilder.GetPartitionSettings();

            CreatePartitionCommand = new RelayCommand(_ =>
                {
                    if (_partitionCreationWindow == null || !_partitionCreationWindow.IsLoaded)
                    {
                        _partitionCreationWindow = new PartitionCreationWindow(Settings);
                        _partitionCreationWindow.Show();
                    }
                    else
                    {
                        _partitionCreationWindow.Activate();
                        var vm = (PartitionCreationViewModel)_partitionCreationWindow.DataContext;
                        vm.RunPartitionCreation();
                    }

                },
                obj =>
                {
                    var valid = true;//Utils.IsValid();

                    if (!valid)
                    {
                        MessageBox.Show("Введені некоректні дані! Щоб продовжити, введіть коректні дані.", "Некоректні дані",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    return true;
                });

            OnClosing = new RelayCommand(_ =>
            {
                _partitionCreationWindow?.Close();
            });

            App.LanguageChanged += App_LanguageChanged;
            App_LanguageChanged(null, null);

            NewConfig = new RelayCommand(_ => NewConfigExec());
            SaveConfig = new RelayCommand(_ => Save());
            SaveAsConfig = new RelayCommand(_ => SaveAs());
            LoadConfig = new RelayCommand(_ => LoadConfiguration());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentConfigurationFile
        {
            get => _configurationSavePath;
            set
            {
                _configurationSavePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConfigFileChosen));
            }
        }

        public bool IsConfigFileChosen => CurrentConfigurationFile != null;

        public PartitionSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                OnPropertyChanged("");
            }
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
                Settings.CentersSettings.CenterDatas.ResizeList(value, () => DefaultSettingsBuilder.GetDefaultCenterData(Settings));

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
            get => Settings.GaussLegendreIntegralOrder;
            set
            {
                Settings.GaussLegendreIntegralOrder = value;
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

        public double CentersDeltaEpsilon
        {
            get => Settings.FuzzyPartitionPlacingCentersSettings.CentersDeltaEpsilon;
            set
            {
                Settings.FuzzyPartitionPlacingCentersSettings.CentersDeltaEpsilon = value;
                OnPropertyChanged();
            }
        }

        public int IterationsCountToIncreaseStep
        {
            get => Settings.RAlgorithmSettings.IterationsCountToIncreaseStep;
            set
            {
                Settings.RAlgorithmSettings.IterationsCountToIncreaseStep = value;
                OnPropertyChanged();
            }
        }

        public double StepIncreaseMultiplier
        {
            get => Settings.RAlgorithmSettings.StepIncreaseMultiplier;
            set
            {
                Settings.RAlgorithmSettings.StepIncreaseMultiplier = value;
                OnPropertyChanged();
            }
        }

        public double StepDecreaseMultiplier
        {
            get => Settings.RAlgorithmSettings.StepDecreaseMultiplier;
            set
            {
                Settings.RAlgorithmSettings.StepDecreaseMultiplier = value;
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

        public double FixedPartitionPsiStartValue
        {
            get => Settings.FuzzyPartitionFixedCentersSettings.PsiStartValue;
            set
            {
                Settings.FuzzyPartitionFixedCentersSettings.PsiStartValue = value;
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

        public RelayCommand OnClosing { get; }

        #endregion

        #region Menu commands

        public RelayCommand SaveConfig { get; }

        public RelayCommand SaveAsConfig { get; }

        public RelayCommand LoadConfig { get; }

        public RelayCommand NewConfig { get; }

        public RelayCommand LightTheme { get; }

        public RelayCommand DarkTheme { get; }

        public RelayCommand AboutCommand { get; } = new RelayCommand(_ =>
        {
            var window = new AboutWindow();
            window.ShowDialog();
        });

        #endregion

        #region Localization commands
        public RelayCommand SwitchToEnglish { get; } = SelectLanguageCommand("en-US");
        public RelayCommand SwitchToUkrainian { get; } = SelectLanguageCommand("uk-UA");
        public RelayCommand SwitchToRussian { get; } = SelectLanguageCommand("ru-RU");

        public static RelayCommand SelectLanguageCommand(string code)
        {
            return new RelayCommand(obj =>
            {
                var menuItem = (MenuItem)obj;
                var cultureInfo = new CultureInfo(code);
                App.Language = cultureInfo;
            });
        }

        private void App_LanguageChanged(object _, System.EventArgs e)
        {
            CurrentCulture = App.Language;
            OnPropertyChanged(nameof(CurrentCulture));
        }

        public CultureInfo CurrentCulture { get; private set; }

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NewConfigExec()
        {
            Settings = DefaultSettingsBuilder.GetPartitionSettings();
            CurrentConfigurationFile = null;
        }

        private void Save()
        {
            if (CurrentConfigurationFile == null)
            {
                SaveAs();
            }
            else
            {
                SaveSettings(CurrentConfigurationFile);
            }
        }

        private void SaveAs()
        {
            var openDialog = new CommonSaveFileDialog
            {
                Title = "Save partition configuration",
                DefaultFileName = $"Partition_{CentersCount}_{SegmentsCountX}x{SegmentsCountY}",
                DefaultExtension = "json",
                InitialDirectory = GetSaveDirectory(),
                AlwaysAppendDefaultExtension = true,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureValidNames = true,
            };
            openDialog.Filters.Add(new CommonFileDialogFilter("JSON files", "*.json"));

            if (openDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filePath = openDialog.FileName;

                Properties.Settings.Default.ConfigurationPath = filePath;
                Properties.Settings.Default.Save();

                CurrentConfigurationFile = filePath;
                SaveSettings(CurrentConfigurationFile);
            }
        }

        private void LoadConfiguration()
        {
            var commonOpenFileDialog = new CommonOpenFileDialog
            {
                Title = "Load partition configuration",
                //DefaultFileName = $"Partition_{CentersCount}_{SegmentsCountX}x{SegmentsCountY}",
                DefaultExtension = "json",
                InitialDirectory = GetSaveDirectory(),
            };

            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var filePath = commonOpenFileDialog.FileName;

                Properties.Settings.Default.ConfigurationPath = filePath;
                Properties.Settings.Default.Save();

                CurrentConfigurationFile = filePath;
                LoadSettings(CurrentConfigurationFile);
            }
        }

        private string GetSaveDirectory()
        {
            var defaultDirectory = Path.Combine(Environment.CurrentDirectory, "Configurations");
            var directory = defaultDirectory;
            if (CurrentConfigurationFile != null && !string.IsNullOrEmpty(CurrentConfigurationFile))
            {
                directory = Path.GetDirectoryName(CurrentConfigurationFile);
            }
            else if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.ConfigurationPath))
            {
                directory = Path.GetDirectoryName(Properties.Settings.Default.ConfigurationPath);
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        private void SaveSettings(string path)
        {
            var writer = new StreamWriter(path);
            var data = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            writer.Write(data);
            writer.Close();
        }

        private void LoadSettings(string path)
        {
            var reader = new StreamReader(path);
            var data = reader.ReadToEnd();
            reader.Close();
            Settings = JsonConvert.DeserializeObject<PartitionSettings>(data, new VectorJsonConverter());
        }
    }
}
