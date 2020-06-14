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
            var outputFolder = Path.Combine(GetBinFolder(), "Debug");
            var targetPath = Path.Combine(outputFolder, GetTargetFileName());

            var scene = SceneManager.GetSceneByName("MainScene");

            var options = new BuildPlayerOptions
            {
                locationPathName = targetPath,
                target = BuildTarget.StandaloneWindows,
                targetGroup = BuildTargetGroup.Standalone,
                scenes = new[] { scene.path },
                assetBundleManifestPath = outputFolder,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            Debug.Log($"Build result: {report.summary.result}");
        }

        [MenuItem("Build/Build to release folder")]
        public static void BuildToReleaseFolder()
        {
            var outputFolder = Path.Combine(GetBinFolder(), "Release");
            var targetPath = Path.Combine(outputFolder, GetTargetFileName());

            var scene = SceneManager.GetSceneByName("MainScene");

            var options = new BuildPlayerOptions
            {
                locationPathName = targetPath,
                target = BuildTarget.StandaloneWindows,
                targetGroup = BuildTargetGroup.Standalone,
                scenes = new[] { scene.path },
                assetBundleManifestPath = outputFolder,
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