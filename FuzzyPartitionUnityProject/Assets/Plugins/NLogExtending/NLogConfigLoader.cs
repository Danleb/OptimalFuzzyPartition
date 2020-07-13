using NLog;
using UnityEngine;

namespace NLogExtending
{
    /// <summary>
    /// Sets up configuration of NLog logging in the scene only in builds.
    /// In editor configuration is set up automatically.
    /// </summary>
    public class NLogConfigLoader : MonoBehaviour
    {
        public const string ResourcesNLogConfigPath = "NLogConfiguration";

        private static readonly object _locker = new object();

        private static bool _configured;

        [SerializeField] private bool _loadFromResourcesInBuild;

        private void Awake()
        {
            lock (_locker)
            {
                if (_configured) return;

#if UNITY_EDITOR
                var source = NLogConfigOpenShortcut.PathToEditorNLogConfig;
                LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(source);
#else
                if (_loadFromResourcesInBuild)
                {
                    var nlogConfiguration = (TextAsset)Resources.Load(ResourcesNLogConfigPath);
                    var xmlStream = new System.IO.StringReader(nlogConfiguration.text);
                    var xmlReader = System.Xml.XmlReader.Create(xmlStream);
                    LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(xmlReader);
                    xmlReader.Close();
                    xmlStream.Close();
                }
#endif

                var logger = LogManager.GetCurrentClassLogger();
                logger.Info("NLog initialized");

                _configured = true;
            }
        }
    }
}