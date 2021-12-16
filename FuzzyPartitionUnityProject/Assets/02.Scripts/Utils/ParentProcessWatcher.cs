using UnityEngine;

namespace Utils
{
    public class ParentProcessWatcher : MonoBehaviour
    {
        private void Update()
        {
            if (!Application.isEditor && !isParentProcessAlive())
            {
                Application.Quit();
            }
        }

        private bool isParentProcessAlive()
        {
            var parentProcess = ParentProcessUtilities.GetParentProcess();
            return !parentProcess.HasExited;
        }
    }
}
