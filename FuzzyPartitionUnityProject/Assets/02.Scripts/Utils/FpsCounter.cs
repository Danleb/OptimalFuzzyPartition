using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _textFpsCounter;

    private Queue<double> _lastSecondsFramesTimes = new Queue<double>();

    private void Update()
    {
        var currentTime = Time.realtimeSinceStartup;
        _lastSecondsFramesTimes.Enqueue(currentTime);

        while (currentTime - _lastSecondsFramesTimes.Peek() > 1.0)
        {
            _lastSecondsFramesTimes.Dequeue();
        }

        _textFpsCounter.text = $"FPS: {_lastSecondsFramesTimes.Count}";
    }
}
