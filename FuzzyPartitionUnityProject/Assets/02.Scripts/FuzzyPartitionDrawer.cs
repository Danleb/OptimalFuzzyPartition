using UnityEngine;
using UnityEngine.UI;

namespace FuzzyPartitionVisualizing
{
    /// <summary>
    /// Draws fuzzy partition 2d.
    /// </summary>
    public class FuzzyPartitionDrawer : MonoBehaviour
    {
        [SerializeField] private Image _outputImage;
        [SerializeField] private Shader _partitionDrawingShader;

        private RenderTexture _renderTexture1;
        private RenderTexture _renderTexture2;

        public void CreatePartition()
        {
            //Graphics.Blit();

            //_outputImage.mainTexture = 
        }

        public void Hide()
        {
            _outputImage.gameObject.SetActive(false);
        }
    }
}