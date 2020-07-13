using FuzzyPartitionComputing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Utils;

namespace Partition.Managing
{
    public class CommandReceiver : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField] private bool _connectOnAwake;
        [SerializeField] private int _manualConnectionPort;
        [SerializeField] private PartitionRunner _partitionRunner;

        private SimpleTcpClient _client;
        private CommandAndData _currentData;
        private readonly Queue<CommandAndData> _commandsAndDatas = new Queue<CommandAndData>();

        [Button]
        public void Connect()
        {
            ConnectToServer(_manualConnectionPort);
        }

        private void Awake()
        {
            if (!SystemInfo.supportsComputeShaders)
            {
                Logger.Fatal("Compute shaders are not supported on this platform.");
                throw new NotSupportedException("Compute shaders are not supported on this platform.");
            }

            if (_connectOnAwake)
            {
                Logger.Info(Environment.CommandLine);

                var args = Environment.GetCommandLineArgs();

                try
                {
                    var portNumber = int.Parse(args[3]);
                    ConnectToServer(portNumber);
                }
                catch (Exception e)
                {
                    Logger.Fatal("Failed to parse arguments. " + e);
                }
            }
        }

        private void ConnectToServer(int port)
        {
            const string ipAddress = "127.0.0.1";

            Logger.Info($"Connecting to {ipAddress}:{port}");

            _client = new SimpleTcpClient().Connect(ipAddress, port);
            _client.DataReceived += Client_DataReceived;
            _client.Write("ClientReadyToWork");
            Logger.Info("Connected");

            PlayStateNotifier.OnPlaymodeExit += PlayStateNotifier_OnPlaymodeExit;
        }

        private void PlayStateNotifier_OnPlaymodeExit()
        {
            PlayStateNotifier.OnPlaymodeExit -= PlayStateNotifier_OnPlaymodeExit;
            _client?.Disconnect();
            _client?.Dispose();
        }

        private void Client_DataReceived(object sender, Message e)
        {
            Logger.Info("Data received from the server: " + e.MessageString);

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

            Logger.Info($"Start to process command. CommandType = {_currentData.CommandType}");

            var settings = _currentData.PartitionSettings;

            switch (_currentData.CommandType)
            {
                case CommandType.CreateFuzzyPartition:
                    {
                        if (_currentData.PartitionSettings.IsCenterPlacingTask)
                        {
                            var result = _partitionRunner.CreateFuzzyPartitionWithCentersPlacing(settings, _currentData.DrawWithMistrustCoefficient, _currentData.MistrustCoefficient);
                            _client.Write(result.ToBytes());
                        }
                        else
                        {
                            var result = _partitionRunner.CreateFuzzyPartitionWithFixedCenters(settings, _currentData.DrawWithMistrustCoefficient, _currentData.MistrustCoefficient);
                            _client.Write(result.ToBytes());

                            _currentData = null;
                        }

                        break;
                    }

                case CommandType.ShowPartitionAtIterationIndex:
                    {
                        //if (_currentData.IterationNumber >= _partitionImageByIterations.Count)
                        //{
                        //    Logger.Error($"Error: iteration number {_currentData.IterationNumber} bigger then existing {_partitionImageByIterations.Count} partition images count.");
                        //    return;
                        //}

                        //_partitionRunner.show

                        _partitionRunner.DrawPartitionAtIteration(_currentData.IterationNumber);
                        break;
                    }

                case CommandType.SavePartitionImage:
                    {
                        if (_currentData.ImageSavePath == null)
                            _partitionRunner.SavePartitionImage(null);
                        else
                        {
                            var path = Encoding.UTF8.GetString(_currentData.ImageSavePath);
                            _partitionRunner.SavePartitionImage(path);
                        }

                        _currentData = null;
                        break;
                    }

                case CommandType.ShowCurrentPartitionWithSettings:
                    {
                        _partitionRunner.RedrawPartitionWithSettings(_currentData.DrawWithMistrustCoefficient, _currentData.MistrustCoefficient);
                        _currentData = null;
                        break;
                    }
            }
        }
    }
}