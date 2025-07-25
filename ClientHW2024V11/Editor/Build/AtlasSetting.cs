using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AtlasSetting : EditorWindow
{
    /// <summary>
    /// 纹理最大大小
    /// </summary>
    int m_nTexMaxSize = 2048;
    /// <summary>
    /// 是否重写其他设置（PC，安卓，IOS）
    /// </summary>
    bool m_bIsOverride = true;
    /// <summary>
    /// 默认纹理导入格式
    /// </summary>
    TextureImporterFormat m_nDefaultPlatformImporterFormat = TextureImporterFormat.RGBA32;
    /// <summary>
    /// 安卓平台纹理导入格式
    /// </summary>
    TextureImporterFormat m_nAndroidPlatformImporterFormat = TextureImporterFormat.ASTC_4x4;
    /// <summary>
    /// PC纹理导入格式
    /// </summary>
    TextureImporterFormat m_nPCPlatformImporterFormat = TextureImporterFormat.Automatic;
    /// <summary>
    /// 苹果纹理导入格式
    /// </summary>
    TextureImporterFormat m_nIphonePlatformImporterFormat = TextureImporterFormat.Automatic;
    /// <summary>
    /// 纹理压缩质量
    /// </summary>
    TextureImporterCompression m_nImporterCompression = TextureImporterCompression.Compressed;

    [MenuItem("Tools/AtlasSetting")]
    static void ChangeTextureSetting()
    {
        //创建窗口
        Rect window = new Rect(500, 500, 500, 500);
        AtlasSetting setting = (AtlasSetting)EditorWindow.GetWindowWithRect(typeof(AtlasSetting), window, false, "AtlasSetting");
        setting.Show();
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();//空一行
        EditorGUILayout.LabelField("修改纹理的设置", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 16 }, GUILayout.Height(30));
        EditorGUILayout.Space();//空一行
        m_nTexMaxSize = int.Parse(EditorGUILayout.TextField("Sprite Max Size", "2048"));
        EditorGUILayout.Space();//空一行
        m_bIsOverride = EditorGUILayout.Toggle("Is Override Other Device", true);
        EditorGUILayout.Space();//空一行
        m_nDefaultPlatformImporterFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Default", m_nDefaultPlatformImporterFormat);
        EditorGUILayout.Space();//空一行
        m_nPCPlatformImporterFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup(" PC", m_nPCPlatformImporterFormat);
        EditorGUILayout.Space();//空一行
        m_nAndroidPlatformImporterFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Android", m_nAndroidPlatformImporterFormat);
        EditorGUILayout.Space();//空一行
        m_nIphonePlatformImporterFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("Iphone", m_nIphonePlatformImporterFormat);
        EditorGUILayout.Space();//空一行
        if (GUILayout.Button("开始设置", GUILayout.ExpandWidth(true), GUILayout.Height(50)))
        {
            EditorUtility.DisplayProgressBar("Loading", "in progress", m_fSliderValue);
            ChangeAllFileSetting();
        }
    }
    bool m_bIsShowProgressBar = false;
    float m_fSliderValue = 0;
    /// <summary>
    /// 此方法弃用
    /// </summary>
    static void StartSetting()
    {
        //var a = Packer.atlasNames;
        //var b = Packer.GetTexturesForAtlas(a[0]);
        //SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
        //var c = Packer.GetAlphaTexturesForAtlas(a[1]);
        //var d =Packer.
        //SpriteAtlasExtensions.GetTextureSettings()
        //Debug.Log(a.Length.ToString());
    }
    /// <summary>
    /// 按钮点击的时候开始改变文件设置
    /// </summary>
    void ChangeAllFileSetting()
    {
        imageCount = 0;
        string filePath = GetSpritePath();
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);
        CheckDirectory(dirInfo);
        var flag = EditorUtility.DisplayDialog("修改完成", $"共修改了{imageCount}个图片", "完成");
        EditorUtility.ClearProgressBar();
    }
    int imageCount = 0;
    /// <summary>
    /// 这里使用一个迭代函数来处理所有文件夹里面嵌套的文件夹内容
    /// </summary>
    /// <param name="directoryInfo"></param>
    void CheckDirectory(DirectoryInfo directoryInfo)
    {
        DirectoryInfo[] infos = directoryInfo.GetDirectories();
        for (int i = 0, length = infos.Length; i < length; i++)
        {
            CheckDirectory(infos[i]);
            //break;
        }
        FileInfo[] files = directoryInfo.GetFiles();
        ChangeSettingByFolder(files);
    }
    /// <summary>
    /// 改变文件数组中所有文件的设置
    /// </summary>
    /// <param name="files"></param>
    void ChangeSettingByFolder(FileInfo[] files)
    {
        for (int i = 0, length = files.Length; i < length; i++)
        {
            if (!files[i].Name.Contains("meta"))
            {
                string onePath = GetImageProjectPath(files[i].FullName);
                TextureImporter m_pImporter = AssetImporter.GetAtPath(onePath) as TextureImporter;
                if (m_pImporter == null)
                    continue;
                ChangeTextureSetting(m_pImporter);
                imageCount++;
                m_fSliderValue = imageCount * 1.0f / 3000;
                EditorUtility.DisplayProgressBar("Loading", "in progress", m_fSliderValue);
            }
        }
    }
    /// <summary>
    /// 根据固定字符串得到资源路径
    /// </summary>
    /// <param name="fullName"></param>
    /// <returns></returns>
    string GetImageProjectPath(string fullName)
    {
        string result = "";
        int index = fullName.LastIndexOf("Assets");
        result = fullName.Substring(index, fullName.Length - index);
        return result;
    }
    /// <summary>
    /// 改变纹理设置
    /// </summary>
    /// <param name="textureImporter"></param>
    void ChangeTextureSetting(TextureImporter textureImporter)
    {
        if (textureImporter == null)
        {
            Debug.LogError("获取设置失败");
            return;
        }
        ChangeDefaultSetting(textureImporter);
      //  ChangePCSetting(textureImporter);
      //  ChangeIphoneSetting(textureImporter);
        ChangeAndroidSetting(textureImporter);
        textureImporter.SaveAndReimport();
    }
    /// <summary>
    /// 修改默认设置
    /// </summary>
    /// <param name="textureImporter"></param>
    void ChangeDefaultSetting(TextureImporter textureImporter)
    {
        TextureImporterPlatformSettings defaultSetting = textureImporter.GetDefaultPlatformTextureSettings();
        defaultSetting.maxTextureSize = m_nTexMaxSize;
        //defaultSetting.compressionQuality = defaultSetting.compressionQuality;

        defaultSetting.crunchedCompression = false;
        defaultSetting.textureCompression = m_nImporterCompression;
        defaultSetting.format = m_nDefaultPlatformImporterFormat;
        textureImporter.SetPlatformTextureSettings(defaultSetting);
    }
    /// <summary>
    /// 设置所有的设备
    /// </summary>
    /// <param name="platformSettings"></param>
    void SetTextureValue(TextureImporterPlatformSettings platformSettings)
    {
        platformSettings.maxTextureSize = m_nTexMaxSize;
        platformSettings.overridden = m_bIsOverride;
    }
    /// <summary>
    /// 修改PC设置
    /// </summary>
    /// <param name="textureImporter"></param>
    void ChangePCSetting(TextureImporter textureImporter)
    {
        TextureImporterPlatformSettings pcSetting = textureImporter.GetPlatformTextureSettings("Standalone");
        SetTextureValue(pcSetting);
        pcSetting.format = m_nPCPlatformImporterFormat;
        textureImporter.SetPlatformTextureSettings(pcSetting);
    }
    /// <summary>
    /// 修改安卓设置
    /// </summary>
    /// <param name="textureImporter"></param>
    void ChangeAndroidSetting(TextureImporter textureImporter)
    {
        TextureImporterPlatformSettings androidSetting = textureImporter.GetPlatformTextureSettings("Android");
        SetTextureValue(androidSetting);
        androidSetting.format = m_nAndroidPlatformImporterFormat;
        textureImporter.SetPlatformTextureSettings(androidSetting);
    }
    /// <summary>
    /// 修改ios设置
    /// </summary>
    /// <param name="textureImporter"></param>
    void ChangeIphoneSetting(TextureImporter textureImporter)
    {
        TextureImporterPlatformSettings iphoneSetting = textureImporter.GetPlatformTextureSettings("Iphone");
        SetTextureValue(iphoneSetting);
        iphoneSetting.format = m_nIphonePlatformImporterFormat;
        textureImporter.SetPlatformTextureSettings(iphoneSetting);
    }
    static string GetSpritePath()
    {
  
        string parentPath = Application.dataPath;
        string resultPath = parentPath + "/ResData";
        return resultPath;
    }
}