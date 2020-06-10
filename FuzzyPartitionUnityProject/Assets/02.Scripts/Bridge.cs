using FuzzyPartitionVisualizing;
using OptimalFuzzyPartitionAlgorithm.Utils;
using SimpleTCP;
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
        [SerializeField] private FuzzyPartitionDrawer _fuzzyPartitionDrawer;

        private readonly Queue<CommandAndData> _commandsAndDatas = new Queue<CommandAndData>();

        private SimpleTcpClient client;

        private SimpleTcpServer simpleTcpServer;

        private void Awake()
        {
#if !UNITY_EDITOR
            Debug.Log(Environment.CommandLine);

            var args = Environment.GetCommandLineArgs();
            var portNumber = int.Parse(args[3]);

            client = new SimpleTcpClient().Connect("127.0.0.1", portNumber);
            client.DataReceived += Client_DataReceived;
            client.Write("ClientReadyToWork");
#endif
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
                _fuzzyPartitionFixedCentersComputer.Run(data.PartitionSettings);
            }
            else if (data.CommandType == CommandType.CreateFuzzyPartitionWithCentersPlacing)
            {
                _fuzzyPartitionPlacingCentersComputer.Run(data.PartitionSettings);
            }
        }

        private void OnDestroy()
        {
            simpleTcpServer.Stop();
        }
    }
}