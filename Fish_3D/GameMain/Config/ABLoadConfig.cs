using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ABLoadConfig
{
	public static string GetAssetBundleOutPutPath ()
	{
		return Application.dataPath + "/../" + ABLoadConfig.BasePath + "/" + KApplication.GetPlatfomrName ();
	}

	public static string GetABSourcePath()
	{
		return ABLoadConfig.BasePath + "/" + KApplication.GetPlatfomrName () + "/";
	}

	public static string GetCacheDepFilePath()
	{
		return Application.dataPath + "/Settings/Cache.txt";
	}

#if UNITY_EDITOR
	#if UNUSE_ASSETBOUNDLE_INEDITOR
	public const bool _Editor_MODE = true;
	#else
    public const bool _Editor_MODE = false;
	#endif
#else
    public const bool _Editor_MODE =false;
#endif
    public static bool Editor_MODE
    {
        get
        {
            return _Editor_MODE;
        }
    }

    public const string VersionPath = "assets.bytes";
    public const string VersionNO = "version.bytes";
    [SerializeField]
    public const string BasePath = "ABResource";



#if UNITY_IPHONE || UNITY_IOS
    private const bool _AbLoadFile = true;
    private const bool _OpenSecret = false;
#else
    private const bool _AbLoadFile = false;
    private const bool _OpenSecret = false;


#endif
    public static bool AbLoadFile
    {
        get
        {
            return _AbLoadFile;
        }
    }

    public static bool OpenSecret
    {
        get
        {
            return _OpenSecret;
        }
    }

    /// <summary>
    /// The load default time.
    /// </summary>
    public const float LoadDefaultTime = 1.0f;
    /// <summary>
    /// The ab key.
    /// </summary>
    public const byte Ab_Key = 25;
    /// <summary>
    /// 将目录分隔符改成此字符
    /// </summary>
    public const string FileCharSplit = "=";
    /// <summary>
    /// 将文件后缀名标记为此字符
    /// </summary>
    public const string CharSplit = "^";
    /// <summary>
    /// The file extensions.
    /// </summary>
    public const string FileExtensions = ".ab";
    /// <summary>
    /// The resource extensions.
    /// </summary>
    public const string ResourceExtensions = "assets=";

    public const string ConvertFileExtension = ".kb";

    public const short Loader_SingleSync_MaxSize = 8;
    /// <summary>
    /// 90m 峰值
    /// </summary>
    public const long Asset_POOL_MaxSize = 951210000;



    public const int Default_DeltaTimeSpan = 5;

    public const int Delta_FrameCout = 500;
}

public enum ABFileTag
{
    NONE,
    Prefab,
    //
    PNG,
    JPG,
    PSD,
    TGA,
	DDS,
    RenderTexture,
    Tif,
    //
    TXT,
    Bytes,
	XML,
    Animation,
    Material,
    FBX,
    Font,
    shader,
    MP3,
    WAV,
    MP4,
	OGG,
    Video,
    Scene,
    ASSET,
    JSON,
	AnimatorController,

}