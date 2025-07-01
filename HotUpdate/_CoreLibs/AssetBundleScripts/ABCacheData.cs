using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Kubility;

#if UNITY_EDITOR
public class ABCacheDataMgr
{
	public string Version;
	public string Platform;
	public List<string> OldList;
	public List<string> NewList;
	List<ABCacheDataInfo> _AndList,_IOSList,_StandAloneList;

	public List<ABCacheDataInfo> GetList ()
	{
		#if UNITY_IPHONE || UNITY_IOS
				return _IOSList;
		#elif UNITY_ANDROID
				return _AndList;
		#else
				return _StandAloneList;
		#endif
	}

	public ABCacheDataInfo GetCacheInfo(string Abname){		
		return GetList ().Find(p=>p.Data.Abname.Equals(Abname));
	}

	public ABCacheDataMgr ()
	{
		Version = "";
		Platform = "";
		_AndList = new List<ABCacheDataInfo> ();
		_IOSList = new List<ABCacheDataInfo> ();
		_StandAloneList = new List<ABCacheDataInfo> ();
		OldList = new List<string>();
		NewList = new List<string>();
	}
}

public class UnityPack
{
	public List<string> list = new List<string> ();
}


public class ABCacheDataInfo
{
	#region 用于打包
	public bool old ;
	public string Filepath;
	public string MD5;
	#endregion

	public ABData Data;
    static Dictionary<string,List<ABCacheDataInfo>> Caches;

    public static void PushToPreDepends(string name,ABCacheDataInfo datainfo)
    {
        if(Caches == null)
            Caches = new Dictionary<string, List<ABCacheDataInfo>>();



        if(Caches.ContainsKey(name))
        {
            Caches[name].TryAdd(datainfo);
        }
        else
        {
            Caches.Add(name,new List<ABCacheDataInfo>(){datainfo});
        }

    }

    public static void PopCheck()
    {
        if (Caches != null)
        {
            foreach (var sub in Caches)
            {
				UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(sub.Key);
                if (importer != null)
                {
                    if (!string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        var depdata = DependData.Create(sub.Key);
                        foreach (var listsub in sub.Value)
                        {
                            listsub.Data.MyNeedDepends.TryAdd(depdata);
                        }
                    }
                }
                else
                {
                    LogMgr.Log("AssetImporter  -> "+ sub.Key);
                }
            }
        }
    }


	public ABCacheDataInfo()
	{
		old  = true;
		Data =new ABData ();
	}
}
#endif

/// <summary>
/// Runtime ABdata 摒弃了动态解析文件类型，文件名，加载名，提高解析速度，牺牲部分内存
/// </summary>
public class ABData
{
	/// <summary>
	/// asset名称不带后缀名
	/// </summary>
	public string Abname;
    /// <summary>
    /// 
    /// </summary>
    public long Size;
	/// <summary>
	/// 后缀名
	/// </summary>
	public short FileType;
	public string LoadName;
	public long VersionCode;
	public string Hash;
	public int RootType; 
	public List<DependData> MyNeedDepends;

    public AssetBundleCreateRequest ab_request = null;//是否正在加载中

	public ABData()
	{
		Abname ="";
		LoadName ="";
		VersionCode =0;
		FileType =0;
		RootType =0;
		Hash ="";
		MyNeedDepends = new List<DependData>();
	}
}

public class DependData:IEquatable<DependData>
{
	/// <summary>
	/// asset名称
	/// </summary>
	public string Abname;

	/// <summary>
	/// 后缀名
	/// </summary>
	public short FileType;

//		public string GetKey()
//		{
//			return Abname+ABLoadConfig.CharSplit+ FileType;
//		}

	public static implicit operator string(DependData data)
	{
		//LogUtils.Log("DependData 发生了隐氏转换");
		return data.Abname;
	}

    public static string GetUnityPath(string path)
    {

        if (path.Contains("\\"))//Application.platform != RuntimePlatform.OSXEditor
        {
            int index = path.IndexOf("Assets\\");
            return path.Substring(index);
        }
        else
        {
            int index = path.IndexOf("Assets/");
            return path.Substring(index);
        }


    }

    public static DependData Create(string file)
    {

        file = GetUnityPath(file);

        string splt_string=null;

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            splt_string = "Assets\\";
        }
        else
        {
            splt_string = "Assets/";
        }

        int index = Mathf.Max(0, file.IndexOf(splt_string));
        string Dep_filename = file.Substring(index);

        string Dep_abname = ConvertABName(file.Substring(index));

		ABFileTag Dep_fileType = KAssetBundleManger.GetDependTagWithAbName(Dep_filename);
        DependData depData = new DependData();
        depData.Abname = Dep_abname;
        depData.FileType = (short)Dep_fileType;

        return depData;
    }

    public static string ConvertABName (string filepath)
    {
        string abname = filepath.Replace ("\\", ABLoadConfig.FileCharSplit)
            //          .Replace (" ", "")
            .Replace (ABLoadConfig.FileExtensions, "")
            .Replace ("/", ABLoadConfig.FileCharSplit)
            //          .Replace ("\t", ABLoadConfig.CharSplit)
            .Replace (".", ABLoadConfig.CharSplit)
            .ToLower ();

        return abname + ABLoadConfig.FileExtensions;
    }
        

	public bool Equals (DependData other)
	{
		if(!Abname.Equals(other.Abname))
			return false;
		else if(!FileType.Equals(other.FileType))
			return false;

		return true;
	}
}
//*/