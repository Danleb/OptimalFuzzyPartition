#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NLogExtending
{
    /// <summary>
    /// Before building copies the relevant version of the NLog config to the Resources folder in order
    /// it to be loaded in built player.
    /// </summary>
    [InitializeOnLoad]
    public class ConfigurationCopier
    {
        static ConfigurationCopier()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(CopyBeforeBuild);
        }

        private static void CopyBeforeBuild(BuildPlayerOptions buildPlayerOptions)
        {
            var destination = Path.ChangeExtension(Path.Combine(Application.dataPath, "Resources", NLogConfigLoader.ResourcesNLogConfigPath), "xml");
            File.Copy(NLogConfigOpenShortcut.PathToEditorNLogConfig, destination, true);

            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }
    }
}
#endif