using UnityEngine;

public class StarTextureGenerator : MonoBehaviour
{
    [Range(64, 2048)] public int size = 512;
    [Range(1, 500)] public int starCount = 200;
    public Gradient color;

    public Texture2D Generate()
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;

        // Fill black
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;

        // Add stars
        for (int i = 0; i < starCount; i++)
        {
            int x = Random.Range(0, size);
            int y = Random.Range(0, size);
            pixels[y * size + x] = color.Evaluate(Random.value);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
