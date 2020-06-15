using NaughtyAttributes;
using System;
using System.IO;
using UnityEngine;

namespace Utils
{
    public class ScreenshotTaker : MonoBehaviour
    {
        public string TakeAndSaveScreenshot(string path)
        {
            ScreenCapture.CaptureScreenshot(path);
            return path;
        }

        [Button("Take and save screenshot")]
        public string TakeAndSaveScreenshot()
        {
#if UNITY_EDITOR
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#else
            var folderPath = Path.Combine(Application.dataPath, "PartitionImages");
#endif

            const string fileName = "PartitionImage_";
            var fileCount = 0;
            var path = "";

            do
            {
                fileCount++;
                path = Path.ChangeExtension(Path.Combine(folderPath, fileName + fileCount), "exe");
            } while (File.Exists(path));

            ScreenCapture.CaptureScreenshot(path);

            return path;
        }
    }
}