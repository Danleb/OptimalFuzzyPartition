using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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
            else
            {
                Assert.AreEqual(count, manualColors.Length);
                return manualColors.ToArray();
            }
        }

        private Color[] AutoGenerateColors(int count)
        {
            var colors = new Color[count];

            for (int i = 0; i < count; i++)
            {
                var c = Random.ColorHSV(hueRange.x, hueRange.y, saturationRange.x, saturationRange.y, valueRange.x, valueRange.y);
                colors[i] = c;
            }

            return colors;
        }
    }
}