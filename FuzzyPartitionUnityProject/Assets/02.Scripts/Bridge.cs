using System;
using System.IO;
using System.IO.Pipes;
using UnityEngine;

namespace FuzzyPartitionVisualizing
{
    /// <summary>
    /// Entry point of app.
    /// </summary>
    public class Bridge : MonoBehaviour
    {
        [SerializeField] private string pipeName;

        private AnonymousPipeClientStream _pipeClient;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;

        private void Awake()
        {
            var args = Environment.GetCommandLineArgs();
            pipeName = args[2];

            _pipeClient = new AnonymousPipeClientStream(PipeDirection.InOut, pipeName);
            _streamReader = new StreamReader(_pipeClient);
            _streamWriter = new StreamWriter(_pipeClient);
        }

        private void ListenPipe()
        {
            while (true)
            {
                _pipeClient.WaitForPipeDrain();

            }
        }

        private void OnDestroy()
        {
            _pipeClient.Close();

        }
    }
}