using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public class ColorsGenerator : MonoBehaviour
    {
        [SerializeField] private Color[] manualColors;

        [SerializeField] private Vector2 hueRange;
        [SerializeField] private Vector2 saturationRange;
        [SerializeField] private Vector2 valueRange;

        public Color[] GetColors(int count)
        {
            var colors = new Color[count];
            for (var i = 0; i < Math.Min(manualColors.Length, count); i++)
            {
                colors[i] = manualColors[i];
            }

            for (var i = manualColors.Length; i < count; i++)
            {
                var c = Random.ColorHSV(hueRange.x, hueRange.y, saturationRange.x, saturationRange.y, valueRange.x, valueRange.y);
                colors[i] = c;
            }

            return colors;
        }
    }
}
