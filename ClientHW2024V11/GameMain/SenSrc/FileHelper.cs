using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;



public static class FileHelper
{
    static List<string> m_bigBundles = new List<string>();
    public static void AddBigBundleName(string name)
    {
        if (!m_bigBundles.Contains(name))
        {
            m_bigBundles.Add(name);
        }
    }

    private static string CheckInBigBundles(string path)
    {
        for (int i = 0; i < m_bigBundles.Count; i++)
        {
            if (path.Contains(m_bigBundles[i]))
            {
                path = m_bigBundles[i];
                break;
            }
        }

        return path;
    }

    public static string SearchSubFilePath(string subName, string manifest, string subpath)
    {
        string fullpath = string.Format("{0}{1}/{2}/{3}/{4}", GameConst.DataPath, AppConst.SubPackName, subName, manifest, subpath);
        return fullpath;
    }

    public static string SearchFilePath(string manifest, string subpath)
    {
        string fullpath = string.Empty;

        if (LoadModule.Instance != null) {
            if (LoadModule.Instance.ManifestHasBundleInfo(subpath)) {
                fullpath = string.Format("{0}{1}/{2}", GameConst.DataPath, manifest, subpath);
            }
            else {
                Util.LogWarning("SearchFilePath: null");
                return string.Empty;
            }
        } else {
            // 没有找到,直接用包里面的
            fullpath = string.Format("{0}{1}/{2}", GameConst.DataPath, manifest, subpath);
        }

        var path = manifest + "/" + subpath;
        var isFind = false;
        if (!File.Exists(fullpath) && HotStart.ins != null && HotStart.ins.SubPackNameArr != null)
        {
            var newPath = "";
            foreach (var item in LoadModule.Instance.m_SubBundle)
            {
                if (item.Value.Contains(subpath))
                {
                    fullpath = FileHelper.SearchSubFilePath(item.Key, manifest, subpath);
                    if (File.Exists(fullpath))
                    {
                        break;
                    }
                    else
                    {
                        if (AppConst.SubPackArr.Contains(item.Key))
                        {
                            isFind = true;
                            fullpath = string.Format("{0}{1}/{2}/{3}/{4}", GameConst.AppContentPath(), AppConst.SubPackName, item.Key, manifest, subpath);
                        }
                    }
                }
            }


        }
 

        if (!File.Exists(fullpath) && !isFind)
        {
            fullpath = Util.AppContentPath() + path;
            return fullpath;
        }
        /*        if (!File.Exists(fullpath))
                {
                    fullpath = string.Format("{0}{1}/{2}", ClientConst.DataPath, manifest, subpath);
                }*/



        Util.Log("SearchFilePath: " + fullpath);
        return fullpath;
    }
	
    public static string GetWWWPath(string path)
    {
        bool addFileHead = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        // 如果是读取apk里的资源,不需要加file:///,其它情况都要加
        if (path.Contains (Application.streamingAssetsPath)) {
            addFileHead = false;
        }
#endif

        if (addFileHead) {
            path = string.Format ("file:///{0}", path);
        }

        Util.Log("GetWWWPath: " + path);
        return path;
    }

    /// <summary>
    /// 检测包的扩展名和小写 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
	public static string CheckBundleName(string path)
	{
        path = CheckInBigBundles(path);
        path = path.ToLower();

        if(path.StartsWith("data/") || path.StartsWith("lua/"))
        {
            path = path.Substring(0,path.LastIndexOf('/'));
        }

        if (path.StartsWith("ui/prefabs/"))
        {
            string subPath = path.Replace("ui/prefabs/", "");
            string[] items = subPath.Split('/');
            path = "ui/prefabs/" + subPath.Replace("/" + items[items.Length - 1], "");
        }

        if (path.StartsWith("sound/") && !path.StartsWith("sound/scene/"))
        {
            string subPath = path.Replace("sound/", "");
            string[] items = subPath.Split('/');
            path = "sound/" + subPath.Replace("/" + items[items.Length - 1], "");
        }

        if (path.StartsWith("ui/font/"))
        {
            string subPath = path.Replace("ui/font/", "");
            path = "ui/font";
        }

        string name = path;
        if (!path.Contains(AppConst.ExtName))
        {
            name = string.Format("{0}{1}", path, AppConst.ExtName);
            Util.Log(string.Format("GenBundlePath,before:{0},  after:{1}", path, name));
        }

        return name.ToLower();
	}

    public static string GetAPKPath(string path)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = path.Replace (Application.streamingAssetsPath, Application.dataPath + "!assets");
#endif
        Util.Log("GetAPKPath: " + path);
        return path;
    }

	public static bool CheckFileExist(string path, bool isFile = true)
	{
		bool exist = false;
		if (isFile)
		{
			exist = File.Exists(path);
		}
		else {
			exist = Directory.Exists(path);
		}

		if (!exist)
		{
			return false;
		}

		return true;
	}

    public static string LoadFile(string path)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path);
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        string str = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();

        return str;
    }

    public static byte[] LoadFileBytes(string path)
    {
        byte[] bytes = null;
        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (Exception e)
        {
            Util.LogError(e.Message);
        }

        return bytes;
    }
}