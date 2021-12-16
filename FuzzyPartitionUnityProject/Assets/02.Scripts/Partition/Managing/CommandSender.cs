using NaughtyAttributes;
using OptimalFuzzyPartitionAlgorithm.ClientMessaging;
using OptimalFuzzyPartitionAlgorithm.Utils;
using Partition.PartitionView;
using SimpleTCP;
using System.Text;
using UnityEngine;
using Utils;

namespace Partition.Managing
{
    /// <summary>
    /// Imitates work of host app that manages calculations on Unity side.
    /// </summary>
    public class CommandSender : MonoBehaviour
    {
        [Header("Server settings")]
        [SerializeField] private bool _autoStart;
        [SerializeField] private int _portNumber;

        [Header("Commands settings")]
        [SerializeField] private PartitionSettingsHolder _partitionSettingsHolder;

        [Header("Show iteration image command settings")]
        [SerializeField] private int _iterationNumber;

        [Header("Create screenshot command")]
        [SerializeField] private bool _manualScreenshotPath;
        [SerializeField] private string _screenshotPath;

        [Header("Show current partition with settings")]
        [SerializeField] RenderingSettings _renderingSettings;

        private SimpleTcpServer _server;

        private void Awake()
        {
            if (!_autoStart) return;

            _server = new SimpleTcpServer();
            _server.Start(_portNumber);
            PlayStateNotifier.OnPlaymodeExit += PlayStateNotifier_OnPlaymodeExit;
        }

        private void PlayStateNotifier_OnPlaymodeExit()
        {
            PlayStateNotifier.OnPlaymodeExit -= PlayStateNotifier_OnPlaymodeExit;
            _server?.Stop();
        }

        [Button]
        public void CreateFuzzyPartition()
        {
            var commandAndData = new CommandAndData
            {
                CommandType = CommandType.CreateFuzzyPartition,
                PartitionSettings = _partitionSettingsHolder.GetPartitionSettings(),
                RenderingSettings = _renderingSettings
            };

            SendToClient(commandAndData);
        }

        [Button]
        public void ShowPartitionAtIteration()
        {
            var commandAndData = new CommandAndData
            {
                CommandType = CommandType.ShowPartitionAtIterationIndex,
                RenderingSettings = _renderingSettings
            };

            SendToClient(commandAndData);
        }

        [Button]
        public void CreateScreenshot()
        {
            var commandAndData = new CommandAndData
            {
                CommandType = CommandType.SavePartitionImage,
                ImageSavePath = _manualScreenshotPath ? Encoding.UTF8.GetBytes(_screenshotPath) : null
            };

            SendToClient(commandAndData);
        }

        [Button]
        public void ShowCurrentPartitionWithSettings()
        {
            var commandAndData = new CommandAndData
            {
                CommandType = CommandType.ShowCurrentPartitionWithSettings,
                RenderingSettings = _renderingSettings
            };

            SendToClient(commandAndData);
        }

        private void SendToClient(CommandAndData commandAndData)
        {
            _server.Broadcast(commandAndData.ToBytes());
        }
    }
}