using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

public class ImportSetting : AssetPostprocessor
{
    static int[] maxSizes = { 32, 64, 128, 256, 512, 1024, 2048};
    static readonly string DEFAULTS_KEY = "DEFAULTS_DONE";
    static readonly uint DEFAULTS_VERSION = 2;

    public bool IsAssetProcessed
    {
        get
        {
            string key = string.Format("{0}_{1}", DEFAULTS_KEY, DEFAULTS_VERSION);
            return assetImporter.userData.Contains(key);
        }
        set
        {
            string key = string.Format("{0}_{1}", DEFAULTS_KEY, DEFAULTS_VERSION);
            assetImporter.userData = value ? key : string.Empty;
        }
    }

    void OnPreprocessTexture()
    {


        if (assetPath.IndexOf("ResData") < 0)
            return;
        if (assetPath.IndexOf("ResData/UI/Prefabs") >= 0)
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;
        if (textureImporter == null)
            return;

        if (assetPath.IndexOf("UI/GameTexture") >= 0 || assetPath.IndexOf("UI/Texture") >= 0)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
        }
        else
        {
            textureImporter.textureType = TextureImporterType.Default;
        }
        
        textureImporter.textureShape = TextureImporterShape.Texture2D;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.mipmapEnabled = false;
        textureImporter.alphaIsTransparency = true;


        int width = 0;
        int height = 0;
        GetOriginalSize(textureImporter, out width, out height);

        var size = GetMaxSize(Mathf.Max(width, height));

        TextureImporterPlatformSettings setting = textureImporter.GetDefaultPlatformTextureSettings();
        setting.overridden = true;
        setting.maxTextureSize = size;
        setting.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        setting.format = TextureImporterFormat.RGBA32;
        setting.compressionQuality =(int)TextureCompressionQuality.Best;
        textureImporter.SetPlatformTextureSettings(setting);

        TextureImporterPlatformSettings setting1 = textureImporter.GetPlatformTextureSettings("Android");
        setting1.overridden = true;
        setting1.maxTextureSize = size;
        setting1.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        setting1.format = TextureImporterFormat.ASTC_4x4;
        textureImporter.SetPlatformTextureSettings(setting1);


    }

    /// <summary>
    /// 获取texture的原始文件尺寸
    /// </summary>
    /// <param name="importer"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void GetOriginalSize(TextureImporter importer, out int width, out int height)
    {
        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(importer, args);
        width = (int)args[0];
        height = (int)args[1];
    }

    bool GetIsMultipleOf4(int f)
    {
        return f % 4 == 0;
    }

    bool GetIsPowerOfTwo(int f)
    {
        return (f & (f - 1)) == 0;
    }

    int GetMaxSize(int longerSize)
    {
        int index = 6;
        for (int i = 0; i < maxSizes.Length; i++)
        {
            if (longerSize <= maxSizes[i])
            {
                index = i;
                break;
            }
        }
        return maxSizes[index];
    }
}