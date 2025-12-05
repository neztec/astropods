using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StarTextureSaver : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Generate Star Texture")]
    void GenerateTex()
    {
        var gen = GetComponent<StarTextureGenerator>();
        var tex = gen.Generate();

        byte[] png = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/Stars.png", png);

        AssetDatabase.Refresh();
    }
#endif
}
