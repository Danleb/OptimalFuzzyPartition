using FuzzyPartitionVisualizing;
using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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

        [SerializeField] private int _manualConnectionPort;

        private readonly Queue<CommandAndData> _commandsAndDatas = new Queue<CommandAndData>();

        private SimpleTcpClient _client;

        [Button]
        public void Connect()
        {
            ConnectToServer(_manualConnectionPort);
        }

        private void Awake()
        {
            if (!SystemInfo.supportsComputeShaders)
                throw new NotSupportedException("Compute shaders are not supported on your platform.");

#if !UNITY_EDITOR
            Debug.Log(Environment.CommandLine);

            var args = Environment.GetCommandLineArgs();
            var portNumber = int.Parse(args[3]);

            ConnectToServer(portNumber);
#endif
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
            _commandsAndDatas.Enqueue(new CommandAndData());

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

            var data = _commandsAndDatas.Dequeue();

            Debug.Log($"Start to compute. Mode = {data.CommandType}");

            if (data.CommandType == CommandType.CreateFuzzyPartitionWithoutCentersPlacing)
            {
                _fuzzyPartitionFixedCentersComputer.Init(data.PartitionSettings);
                var renderTexture = _fuzzyPartitionFixedCentersComputer.Run();
                //_fuzzyPartitionDrawer.DrawPartition();
            }
            else if (data.CommandType == CommandType.CreateFuzzyPartitionWithCentersPlacing)
            {
                _fuzzyPartitionPlacingCentersComputer.Init(data.PartitionSettings);
                _fuzzyPartitionPlacingCentersComputer.Run();
            }
            else if (data.CommandType == CommandType.AlwaysShowCentersValueChange)
            {
                if (data.AlwaysShowCentersInfo)
                    _centersInfoShower.EnableShowAlways();
                else
                    _centersInfoShower.DisableShowAlways();
            }
            else if (data.CommandType == CommandType.ShowPartitionAtIterationIndex)
            {

            }
        }

        private void OnDestroy()
        {
            _client?.Dispose();
        }
    }
}