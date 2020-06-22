using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public class ColorsGenerator : MonoBehaviour
    {
        [SerializeField] private bool autoGenerateColors;

        [SerializeField] private Color[] manualColors;

        [SerializeField] private Vector2 hueRange;
        [SerializeField] private Vector2 saturationRange;
        [SerializeField] private Vector2 valueRange;

        //[SerializeField] private float minDistance;

        public Color[] GetColors(int count)
        {
            if (autoGenerateColors)
                return AutoGenerateColors(count);

            if (manualColors.Length < count)
                throw new Exception($"Not enough manual colors are set ({manualColors.Length} vs {count})");

            return manualColors.ToArray();
        }

        private Color[] AutoGenerateColors(int count)
        {
            var colors = new Color[count];

            for (var i = 0; i < count; i++)
            {
                var c = Random.ColorHSV(hueRange.x, hueRange.y, saturationRange.x, saturationRange.y, valueRange.x, valueRange.y);
                colors[i] = c;
            }

            return colors;
        }
    }
}