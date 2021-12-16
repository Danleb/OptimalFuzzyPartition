using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class BuildToFolder : MonoBehaviour
    {
        [MenuItem("Build/Build to debug folder")]
        public static void BuildToDebugFolder()
        {
            BuildToBinFolder("Debug");
        }

        [MenuItem("Build/Build to release folder")]
        public static void BuildToReleaseFolder()
        {
            BuildToBinFolder("Release");
        }

        private static void BuildToBinFolder(string buildType)
        {
            var targetPath = Path.Combine(GetBinFolder(), buildType, GetTargetFileName());

            var scene = SceneManager.GetSceneByName("MainScene");

            var options = new BuildPlayerOptions
            {
                locationPathName = targetPath,
                target = BuildTarget.StandaloneWindows,
                targetGroup = BuildTargetGroup.Standalone,
                scenes = new[] { scene.path },
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"Build result: {report.summary.result}");
        }

        private static string GetBinFolder()
        {
            var assetsFolder = Application.dataPath;
            var rootFolder = Path.GetDirectoryName(Path.GetDirectoryName(assetsFolder));
            var binFolder = Path.Combine(rootFolder, "OptimalFuzzyPartition", "bin");
            Debug.Log($"Bin folder: {binFolder}");
            return binFolder;
        }

        private static string GetTargetFileName()
        {
            return "FuzzyPartitionUnityProject.exe";
        }
    }
}