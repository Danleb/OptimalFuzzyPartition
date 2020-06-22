using FuzzyPartitionVisualizing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm;
using OptimalFuzzyPartitionAlgorithm.Algorithm;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using Image = UnityEngine.UI.Image;

namespace FuzzyPartitionComputing
{
    /// <summary>
    /// Entry point of app.
    /// </summary>
    public class Bridge : MonoBehaviour
    {
        [SerializeField] private FuzzyPartitionPlacingCentersComputer _fuzzyPartitionPlacingCentersComputer;
        [SerializeField] private FuzzyPartitionFixedCentersComputer _fuzzyPartitionFixedCentersComputer;
        [SerializeField] private FuzzyPartitionImageCreator _fuzzyPartitionDrawer;
        [SerializeField] private CentersInfoShower _centersInfoShower;
        [SerializeField] private MuConverter _muConverter;
        [SerializeField] private ScreenshotTaker _screenshotTaker;
        [SerializeField] private ColorsGenerator _colorsGenerator;
        [SerializeField] private Image _partitionImage;

        [SerializeField] private bool _connectOnAwake;
        [SerializeField] private int _manualConnectionPort;

        private readonly Queue<CommandAndData> _commandsAndDatas = new Queue<CommandAndData>();

        private SimpleTcpClient _client;

        private CommandAndData _currentData;
        private List<Texture2D> _partitionImageByIterations;

        [Button]
        public void Connect()
        {
            ConnectToServer(_manualConnectionPort);
        }

        private void Awake()
        {
            if (!SystemInfo.supportsComputeShaders)
            {
                throw new NotSupportedException("Compute shaders are not supported on your platform.");
            }

            if (_connectOnAwake)
            {
                Debug.Log(Environment.CommandLine);

                var args = Environment.GetCommandLineArgs();

                try
                {
                    var portNumber = int.Parse(args[3]);
                    ConnectToServer(portNumber);
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse arguments. " + e);
                }
            }
        }

        private void ConnectToServer(int port)
        {
            _client = new SimpleTcpClient().Connect("127.0.0.1", port);
            _client.DataReceived += Client_DataReceived;
            _client.Write("ClientReadyToWork");
        }

        private void Client_DataReceived(object sender, Message e)
        {
            Debug.Log("Data received from the server: " + e.MessageString);

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(e.Data))
            {
                var obj = bf.Deserialize(ms);
                var data = (CommandAndData)obj;
                _commandsAndDatas.Enqueue(data);
            }
        }

        private void Update()
        {
            if (!_commandsAndDatas.Any()) return;
            if (_currentData != null) return;

            _currentData = _commandsAndDatas.Dequeue();

            Debug.Log($"Start to compute. Mode = {_currentData.CommandType}");

            var settings = _currentData.PartitionSettings;

            switch (_currentData.CommandType)
            {
                case CommandType.CreateFuzzyPartition:
                    {
                        if (_currentData.PartitionSettings.IsCenterPlacingTask)
                        {
                            StartCoroutine(PlacingCentersCoroutine(settings));
                        }
                        else
                        {
                            _fuzzyPartitionFixedCentersComputer.Init(settings);
                            var muGridsRenderTexture = _fuzzyPartitionFixedCentersComputer.Run();
                            var muGrids = _muConverter.GetMuGridValueInterpolators(muGridsRenderTexture, settings);
                            var targetFunctionalCalculator = new TargetFunctionalCalculator(settings);
                            var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muGrids);
                            var dualTargetFunctionalValue = 0d;

                            _fuzzyPartitionDrawer.Init(settings, _colorsGenerator.GetColors(settings.CentersSettings.CentersCount));
                            DrawPartitionWithSettings(_currentData, muGridsRenderTexture);

                            var resultData = new CommandAndData
                            {
                                PartitionSettings = settings,
                                WorkFinished = true,
                                TargetFunctionalValue = targetFunctionalValue,

                            };
                            _client.Write(resultData.ToBytes());

                            _currentData = null;
                        }

                        break;
                    }

                case CommandType.ShowPartitionAtIterationIndex:
                    {
                        if (_currentData.IterationNumber >= _partitionImageByIterations.Count)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error: iteration number {_currentData.IterationNumber} bigger then existing {_partitionImageByIterations.Count} partition images count.");
                            return;
                        }

                        _partitionImage.material.mainTexture = _partitionImageByIterations[_currentData.IterationNumber];

                        _currentData = null;
                        break;
                    }

                case CommandType.SavePartitionImage:
                    {
                        if (_currentData.ImageSavePath == null)
                            _screenshotTaker.TakeAndSaveScreenshot();
                        else
                        {
                            var path = Encoding.UTF8.GetString(_currentData.ImageSavePath);
                            _screenshotTaker.TakeAndSaveScreenshot(path);
                        }

                        _currentData = null;
                        break;
                    }

                case CommandType.ShowCurrentPartitionWithSettings:
                    {
                        DrawPartitionWithSettings(_currentData);
                        _currentData = null;
                        break;
                    }
            }
        }

        public IEnumerator PlacingCentersCoroutine(PartitionSettings settings)
        {
            _partitionImageByIterations = new List<Texture2D>();
            _fuzzyPartitionPlacingCentersComputer.Init(settings);

            while (true)
            {
                var (centers, finished) = _fuzzyPartitionPlacingCentersComputer.DoIteration();

                for (var i = 0; i < centers.Count; i++)
                    settings.CentersSettings.CenterDatas[i].Position = centers[i];

                _fuzzyPartitionFixedCentersComputer.Init(settings);
                var muGridsRenderTexture = _fuzzyPartitionFixedCentersComputer.Run();
                var muGrids = _muConverter.GetMuGridValueInterpolators(muGridsRenderTexture, settings);
                var targetFunctionalCalculator = new TargetFunctionalCalculator(settings);
                var targetFunctionalValue = targetFunctionalCalculator.CalculateFunctionalValue(muGrids);
                var dualTargetFunctionalValue = 0d;

                var resultData = new CommandAndData
                {
                    PartitionSettings = settings,
                    WorkFinished = false,
                    TargetFunctionalValue = targetFunctionalValue,
                    IterationNumber = _fuzzyPartitionPlacingCentersComputer.PlacingAlgorithm.PerformedIterationCount
                };
                _client.Write(resultData.ToBytes());

                _fuzzyPartitionDrawer.Init(settings, _colorsGenerator.GetColors(settings.CentersSettings.CentersCount));

                var texture = DrawPartitionWithSettings(_currentData, muGridsRenderTexture);
                _partitionImageByIterations.Add(texture);

                if (finished)
                {
                    _currentData = null;
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private Texture2D DrawPartitionWithSettings(CommandAndData data, RenderTexture muRenderTexture = null)
        {
            _fuzzyPartitionDrawer.DrawThresholdValue = data.DrawWithMistrustCoefficient;
            _fuzzyPartitionDrawer.MuThresholdValue = (float)data.MistrustCoefficient;

            _centersInfoShower.Init(data.PartitionSettings);

            if (data.AlwaysShowCentersInfo)
                _centersInfoShower.EnableShowAlways();
            else
                _centersInfoShower.DisableShowAlways();

            var partitionTexture = muRenderTexture == null ?
                _fuzzyPartitionDrawer.ReDrawWithCurrentSettings() :
                _fuzzyPartitionDrawer.CreatePartitionAndShow(muRenderTexture);

            return partitionTexture;
        }

        private void OnDestroy()
        {
            _client?.Dispose();
        }
    }
}