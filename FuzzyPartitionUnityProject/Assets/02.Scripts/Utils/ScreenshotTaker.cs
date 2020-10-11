using NaughtyAttributes;
using System;
using System.IO;
using UnityEngine;

namespace Utils
{
    public class ScreenshotTaker : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public string TakeAndSaveScreenshot(string folderPath)
        {
            const string fileName = "PartitionImage_";
            var fileCount = 0;
            string path;

            do
            {
                fileCount++;
                path = Path.ChangeExtension(Path.Combine(folderPath, fileName + fileCount), "png");
            } while (File.Exists(path));

            Logger.Info($"Screenshot saving path = {path}");

            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            ScreenCapture.CaptureScreenshot(path);

            //var texture = ScreenCapture.CaptureScreenshotAsTexture();
            //byte[] bytes = texture.EncodeToPNG();
            //Logger.Info(bytes.Length);
            //File.WriteAllBytes(path, bytes);
            //Logger.Info(bytes.Length / 1024 + "Kb was saved as: " + path);

            return path;
        }

        [Button("Take and save screenshot")]
        public string TakeAndSaveScreenshot()
        {
#if UNITY_EDITOR
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#else
            var folderPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "PartitionImages");
#endif            

            return TakeAndSaveScreenshot(folderPath);
        }
    }
}