using System;
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
    /// Converts this collection of sprites into an array of texture.
    /// </summary>
    public static Texture2D[] ToTextures(this ICollection<Sprite> sprites)
    {
        int count = sprites.Count;
        int index = 0;
        Texture2D[] textures = new Texture2D[count];
        foreach (Sprite sprite in sprites)
        {
            textures[index] = ToTexture(sprite);
            index++;
        }

        return textures;
    }

    /// <summary>
    /// Cuts the sprite into a texture using its main texture.
    /// </summary>
    public static Texture2D ToTexture(this Sprite sprite)
    {
        if (!sprite)
        {
            return null;
        }

        return sprite.ToTexture(sprite.texture);
    }

    /// <summary>
    /// Cuts the texture into a smaller texture that represents the sprite.
    /// </summary>
    public static Texture2D ToTexture(this Sprite sprite, Texture2D textureToCut)
    {
        if (!sprite)
        {
            throw new NullReferenceException("Sprite being given is null.");
        }

        if (!textureToCut)
        {
            throw new NullReferenceException("Texture being given is null.");
        }

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

        float minX = 1f;
        float maxX = 0f;
        float minY = 1f;
        float maxY = 0f;
        foreach (Vector2 uv in sprite.uv)
        {
            minX = Mathf.Min(minX, uv.x);
            maxX = Mathf.Max(maxX, uv.x);
            minY = Mathf.Min(minY, uv.y);
            maxY = Mathf.Max(maxY, uv.y);
        }

        int x = (int)sprite.rect.xMin;
        int y = (int)sprite.rect.yMin;
        int width = (int)sprite.rect.width;
        int height = (int)sprite.rect.height;

        Texture2D croppedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        Color[] pixels = textureToCut.GetPixels(x, y, width, height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        croppedTexture.filterMode = FilterMode.Point;
        spriteToTexture[sprite] = croppedTexture;
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