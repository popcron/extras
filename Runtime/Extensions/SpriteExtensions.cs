using System.Collections.Generic;
using UnityEngine;

public static class SpriteExtensions
{
    private static Dictionary<Sprite, Texture2D> spriteToTexture = new Dictionary<Sprite, Texture2D>();

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
#endif
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        spriteToTexture = new Dictionary<Sprite, Texture2D>();
    }

    /// <summary>
    /// Cuts the sprite into a texture using its main texture.
    /// </summary>
    public static Texture2D ToTexture(this Sprite sprite) => sprite.ToTexture(sprite.texture);

    /// <summary>
    /// Cuts the texture into a smaller texture that represents the sprite.
    /// </summary>
    public static Texture2D ToTexture(this Sprite sprite, Texture2D textureToCut)
    {
        //return existing ones
        if (spriteToTexture.TryGetValue(sprite, out Texture2D existingTexture))
        {
            return existingTexture;
        }

        //not readable, so return null
        if (!textureToCut.isReadable)
        {
            Debug.LogError($"Couldn't convert to texture because {textureToCut} isn't marked as readable.");
            return null;
        }

        Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.ARGB32, false, true);
        Color[] pixels = textureToCut.GetPixels((int)sprite.rect.x,
                                                (int)sprite.rect.y,
                                                (int)sprite.rect.width,
                                                (int)sprite.rect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        croppedTexture.filterMode = FilterMode.Point;
        return croppedTexture;
    }

    public static Rect GetSpriteBounds(this Sprite sprite)
    {
        if (!sprite)
        {
            return new Rect();
        }

        int w = sprite.rect.width.RoundToInt();
        int h = sprite.rect.height.RoundToInt();
        Texture2D tex = new Texture2D(w, h);
        Color[] pixels = sprite.texture.GetPixels(sprite.rect.x.RoundToInt(), sprite.rect.y.RoundToInt(), w, h);
        tex.SetPixels(pixels);
        tex.Apply();

        int endX = 0;
        int startX = w;
        int endY = 0;
        int startY = w;
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Color c = tex.GetPixel(x, y);
                if (c.a > 0)
                {
                    if (x < startX)
                    {
                        startX = x;
                    }

                    if (x > endX)
                    {
                        endX = x;
                    }

                    if (y < startY)
                    {
                        startY = y;
                    }

                    if (y > endY)
                    {
                        endY = y;
                    }
                }
            }
        }

        return new Rect(startX, startY, endX - startX, endY - startY);
    }
}