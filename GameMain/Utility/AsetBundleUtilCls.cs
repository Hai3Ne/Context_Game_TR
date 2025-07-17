
//#define Cor_Manager

//#define ASYNAC_LOAD
//#define SHOW_LOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using System.IO;

public enum PlatformType
{
    Windows,
    Android,
    IOS,
    OSX,
    MAX
}

public enum ABNodeTag
{
    /// <summary>
    /// 根节点
    /// </summary>
    ROOT = 1,
    /// <summary>
    /// 普通资源
    /// </summary>
    ASSET = 2,
    /// <summary>
    /// 互相依赖的资源
    /// </summary>
    ROOTASSET = ROOT | ASSET,
    /// <summary>
    /// 独立资源，
    /// </summary>
    STANDALONE = ROOT << 3,
    /// <summary>
    /// 不摧毁的资源,用于挂接在gameobject下，用户负责卸载
    /// </summary>
    DONTDESTROY = ROOT << 4,

    /// <summary>
    /// 场景资源，预加载负责
    /// </summary>
    SCENEASSET = ROOT << 5,
}

public class KApplication
{
    private static bool _isPlaying;
    
#if UNITY_ANDROID
    public const string PlatformName = "ANDROID";//
#elif UNITY_IPHONE || UNITY_IOS
    public const string PlatformName = "IOS";
#else
    public const string PlatformName = "WINDOWS";
    // public const string PlatformName = "ANDROID";
#endif
    public static string GetPlatfomrName()
	{
		string platformName = "WINDOWS";
		if (KApplication.isAndroid) {
			platformName = "Android";
		} else if (KApplication.isIOS) {
			platformName = "iOS";
		} else {
			platformName = "Windows";
		}
		return platformName.ToUpper();
	}

	public static string GetExt(PlatformType pt)
	{
		switch(pt)
		{
		case PlatformType.Android:
			return ".apk";
		case PlatformType.IOS:
			return ".ipa";
		case PlatformType.Windows:
			return ".exe";
		case PlatformType.OSX:
			return ".pkg";
		}
		return ".exe";
	}

    public static bool isPlaying
    {
		set { _isPlaying = value;}
        get
        {
			return _isPlaying;
        }
    }

    public static bool is32
    {
        get
        {
            return IntPtr.Size == 4;
        }
    }

    public static bool is64
    {
        get
        {
            return IntPtr.Size == 8;
        }
    }

    public static bool isIOS
    {
        get
        {
#if UNITY_IPHONE || UNITY_IOS
			return true;	
#endif
            return false;
        }

    }

    public static bool isAndroid
    {
        get
        {
#if UNITY_ANDROID
            return true;
#else
			return false;
#endif

        }
    }

    private static string _persistentDataPath;
    public static string persistentDataPath
    {
        get
        {
            if (_persistentDataPath == null)
            {
                _persistentDataPath = Application.persistentDataPath;
            }
            return _persistentDataPath;
        }
    }


    private static string _DataPath;

    public static string DataPath
    {
        get
        {
            if (_DataPath == null)
            {
                _DataPath = Application.dataPath;
            }
            return _DataPath;
        }
    }


	public static void Shutdown()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}