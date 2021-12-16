using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Utils
{
    public static class TextureUtils
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture)
        {
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, GraphicsFormat.R32_SFloat, 0, TextureCreationFlags.None);//, TextureFormat.RGB24/*TextureFormat.ARGB32*/, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();
            return tex;
        }

        public static Sprite ToSprite(this RenderTexture renderTexture)
        {
            return renderTexture.ToTexture2D().ToSprite();
        }

        public static Sprite ToSprite(this Texture2D texture2D)
        {
            Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            return sprite;
        }
    }
}
