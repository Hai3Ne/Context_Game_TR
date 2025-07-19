using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleManager {
    public static Dictionary<string, AssetBundleInfo> dic_ab = new Dictionary<string, AssetBundleInfo>();
    public static void LoadABSetting(string path, List<AssetBundleInfo> list)
    {
        //加载AssetBundle相关设置文件
        foreach (var item in list)
        {
            item.FullPath = path + item.ABname;
            if (dic_ab.ContainsKey(item.ABname))
            {
                dic_ab[item.ABname] = item;
            }
            else
            {
                dic_ab.Add(item.ABname, item);
            }
        }
        AssetBundleInfo ab;
        foreach (var item in list)
        {
            foreach (var dp in item.DependList)
            {
                if (dic_ab.TryGetValue(dp, out ab))
                {
                    item.DependABList.Add(ab);
                }
                else
                {
                    LogMgr.LogError("数据错误 找不到指定的ab文件:"+dp);
                }
            }
        }
    }
    public static AssetBundleInfo GetABInfo(string ab_name) {
        AssetBundleInfo ab;
        if (dic_ab.TryGetValue(ab_name, out ab)) {
            return ab;
        } else {
            LogMgr.LogError("找不到对应的ab文件：" + ab_name);
            return null;
        }
    }
    public static AssetBundleInfo GetABInfoByPath(string path) {
        string ab_name = PathToAbName(path);
        AssetBundleInfo ab;
        if (dic_ab.TryGetValue(ab_name, out ab))
        {
            return ab;
        }
        else
        {
            Debug.LogError("找不到对应的ab文件：" + ab_name);
            return null;
        }
    }
    public static string PathToAbName(string path) {//路径转ab名称
        int lastindex = path.LastIndexOf('.');
        if (lastindex >= 0) {
            path = path.Substring(0, path.LastIndexOf('.'));//去掉后缀
        }
        path = path.Replace('/', '^');
        path = path.Replace('\\', '^');
        return path.ToLower() + ".kb";
        //return path + ".kb";
    }
    public static string AbNameToPath(string ab_name) {//ab名称转路径
        return ab_name.Substring(0, ab_name.Length - 3).Replace('^', '/');
    }

}
