using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class TextureEditor : EditorWindow
{
    [MenuItem("Tools/压缩UI图集", priority = 151)]
    public static void CompressUITextures()
    {
        List<string> allAtlas = GetAllFileExtension("Assets/Atlas", "*.png");
        if (null == allAtlas || 0 == allAtlas.Count)
        {
            return;
        }

        for (int i = 0; i < allAtlas.Count; i++)
        {
            string file = allAtlas[i].Replace("\\", "/");
            EditorUtility.DisplayProgressBar("压缩UI图集", file, i / (float)allAtlas.Count);

            CompressTexture(file);
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("Assets/压缩图集", priority = 3)]
    [MenuItem("Tools/压缩图集", priority = 151)]
    public static void CompressSelectedTexture()
    {
        Object[] selectTextures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        if (null == selectTextures || 0 == selectTextures.Length)
        {
            Debug.LogError("select no textures");

            return;
        }
        for (int i = 0; i < selectTextures.Length; i++)
        {
            EditorUtility.DisplayProgressBar("压缩贴图", selectTextures[i].name, i / (float)selectTextures.Length);

            CompressTexture(selectTextures[i] as Texture2D);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static List<string> GetAllFileExtension(string dir, string searchPattern)
    {
        if (!Directory.Exists(dir))
        {
            Debug.LogError("Directory not exists:" + dir);

            return null;
        }

        return new List<string>(Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories));
    }

    public static void CompressTexture(string path)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        CompressTexture(texture);
    }
    //是否2的次方
    static bool isTwoPow(int index)
    {
        if (index < 1) return false;
        return (index & (index - 1)) == 0;
    }
    public static void CompressTexture(Texture2D texture)
    {
        if (null == texture)
        {
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }
        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        int maxSize = texture.width > texture.height ? texture.width : texture.height;
        TextureImporterFormat androidFormat = TextureImporterFormat.RGBA32;
        TextureImporterFormat iosFormat = TextureImporterFormat.RGBA32;
        int tmpSize = 0;
        textureImporter.GetPlatformTextureSettings("Android", out tmpSize, out androidFormat);
        if (androidFormat == TextureImporterFormat.RGBA32 || androidFormat == TextureImporterFormat.Automatic)
        {
            textureImporter.SetPlatformTextureSettings("Android", maxSize, TextureImporterFormat.ETC2_RGBA8, false);
            textureImporter.SaveAndReimport();
        }
        textureImporter.GetPlatformTextureSettings("iPhone", out tmpSize, out iosFormat);
        if ((iosFormat == TextureImporterFormat.RGBA32 || iosFormat == TextureImporterFormat.Automatic) && isTwoPow(texture.width) && isTwoPow(texture.height))
        {
            textureImporter.SetPlatformTextureSettings("iPhone", maxSize, TextureImporterFormat.PVRTC_RGBA4, false);
            textureImporter.SaveAndReimport();
        }
    }
}
