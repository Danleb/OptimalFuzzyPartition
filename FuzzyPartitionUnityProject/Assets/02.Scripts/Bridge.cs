using FuzzyPartitionComputing;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    /// <summary>
    /// Entry point of app.
    /// </summary>
    public class Bridge : MonoBehaviour
    {
        [SerializeField] private string pipeNameIn;
        [SerializeField] private string pipeNameOut;

        [SerializeField] private FuzzyPartition2dComputer fuzzyPartition2DComputer;
        [SerializeField] private FuzzyPartition2dDrawer fuzzyPartition2DDrawer;

        private AnonymousPipeClientStream _pipeClientIn;
        private AnonymousPipeClientStream _pipeClientOut;

        private Queue<CommandAndData> _commandsAndDatas = new Queue<CommandAndData>();

        private Thread _commandsListeningThread;

        private void Awake()
        {
            var args = Environment.GetCommandLineArgs();
            pipeNameIn = args[2];
            pipeNameOut = args[3];

            _pipeClientOut = new AnonymousPipeClientStream(PipeDirection.Out, pipeNameIn);
            _pipeClientIn = new AnonymousPipeClientStream(PipeDirection.In, pipeNameOut);

            _commandsListeningThread = new Thread(ListenCommands);
            _commandsListeningThread.Start();
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (!_commandsAndDatas.Any()) return;

            var data = _commandsAndDatas.Dequeue();

            if (data.CommandType == CommandType.CreateFuzzyPartitionWithoutCentersPlacing)
            {
                fuzzyPartition2DComputer.Run(data.PartitionSettings);
            }
        }

        private void ListenCommands()
        {
            PipesMessaging.ReadMessages(_pipeClientIn, OnNewCommandReceived);
        }

        private void OnNewCommandReceived(byte[] buffer, int offset, int count)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(buffer, offset, count))
            {
                var obj = bf.Deserialize(ms);
                var data = (CommandAndData)obj;
                _commandsAndDatas.Enqueue(data);
            }
        }

        private void OnDestroy()
        {
            _pipeClientIn.Close();
            _pipeClientOut.Close();
        }
    }
}