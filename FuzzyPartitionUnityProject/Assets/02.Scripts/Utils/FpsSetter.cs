using System.Threading;
using UnityEngine;

namespace Utils
{
    public class FpsSetter : MonoBehaviour
    {
        [SerializeField] private int targetFps;

        private void Update()
        {
            var targetTime = 1000.0 / targetFps;
            var delta = targetTime - Time.deltaTime;
            Thread.Sleep((int)delta);
        }
    }
}
