#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NLogExtending
{
    /// <summary>
    /// Opens default text editor for NLog editor logging configuration.
    /// </summary>
    public class NLogConfigOpenShortcut
    {
        public static string PathToEditorNLogConfig => Path.Combine(Application.dataPath, LocalPathToEditorNLogConfig);

        private const string LocalPathToEditorNLogConfig = "Plugins/NLog/NLog.dll.nlog";

        /// <summary>
        /// Shortcut for logging config opening (Control + l).
        /// </summary>
        [MenuItem("Tools/Open NLog configuration file %l")]
        private static void OpenNLogConfigurationFile()
        {
            Process.Start(PathToEditorNLogConfig);
        }
    }
}
#endif
