using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 资源版本管理类
/// </summary>
public class ResVersionManager {
    private static Dictionary<GameEnum, bool> dic_check = new Dictionary<GameEnum, bool>();//资源更新记录
    private static Dictionary<GameEnum, long> dic_downsize = new Dictionary<GameEnum, long>();//需要更新资源大小

    public static long TryGetDownSize(GameEnum type)
    {
        return dic_downsize.GetValueOrDefault(type, -1);
    }
    public static void GetDownSize(MonoBehaviour mono, GameEnum type, VoidCall<bool> need_download, VoidCall<long> call)
    {
        long size = 0;
        if (dic_downsize.TryGetValue(type, out size))
        {
            if (size >= 0)
            {
                call(size);
                return;
            }
        }
        else
        {
            dic_downsize.Add(type, -1);
        }

        string path = GameManager.GetAbPath(type);
        string local_path = string.Format("{0}/{1}", KApplication.persistentDataPath, path);
        string net_path = string.Format("{0}/{1}/{2}", GameParams.Instance.AbDownLoadSite, KApplication.PlatformName, path);

        //本地版本文件路径
        string local_version_path = local_path + ABLoadConfig.VersionNO;
        //资源服务器版本文件路径
        string net_version_path = net_path + ABLoadConfig.VersionNO;

        long local_version = -1;

        if (File.Exists(local_version_path))
        {
            local_version = long.Parse(System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(local_version_path)));
        }

        List<AssetBundleInfo> local_list = null;
        if (File.Exists(local_path + ABLoadConfig.VersionPath))
        {
            try {
                string local_asset = File.ReadAllText(local_path + ABLoadConfig.VersionPath);
                local_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(local_asset);
            } catch {
                local_list = new List<AssetBundleInfo>();
            }
        }

        GameUtils.DownLoadTxt(mono, net_version_path, null, (is_success, server_version) =>
        {
            if (is_success)
            {
                if (long.Parse(server_version) > local_version)
                {
                    if (need_download != null)
                        need_download(true);
                    GameUtils.DownLoadTxt(mono, net_path + ABLoadConfig.VersionPath, null, (success, assets) =>
                    {
                        List<AssetBundleInfo> new_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(assets);
                        List<AssetBundleInfo> update_list = GetDiffList(local_path, local_list,new_list);
                        foreach (var item in update_list)
                        {
                            size += item.Size;
                        }
                        if (dic_downsize.ContainsKey(type))
                            dic_downsize[type] = size;
                        else
                            dic_downsize.Add(type, size);

                        if (call != null)
                            call(size);
                    });
                }
                else
                {
                    if (need_download != null)
                        need_download(false);
                    dic_downsize[type] = 0;
                    if (call != null)
                        call(0);
                }
            }
            else
            {
                dic_downsize.Remove(type);
                LogMgr.LogWarning("版本文件version.bytes拉取失败");
                if (call != null)
                    call(-1);
            }
        });
    }

    /// <summary>
    /// 除大厅资源下载外的其他游戏资源下载
    /// </summary>
    /// <param name="type"></param>
    /// <param name="call"></param>
    public static void CheckVersion(GameEnum type, VoidCall call)
    {
        //检查资源是否需要更新
// #if UNITY_EDITOR
//         // call();
//        if (!dic_check.ContainsKey(type))
//        {
//             string path1 = GameManager.GetAbPath(type);
//             var str = File.ReadAllText(Application.persistentDataPath + "/" + path1 + "assets.bytes");
//             var list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(str);
//
//             EnterGame(type, list, call);
//         }
//         return;
// #endif
 /*       if (!dic_check.ContainsKey(type)) {
            string path1 = GameManager.GetAbPath(type);
            var str = File.ReadAllText(Application.persistentDataPath + "/" + path1 + "assets.bytes");
            var list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(str);

            EnterGame(type, list, call);
            return;
        }*/
        

        if (dic_check.ContainsKey(type))
        {
            call();
            return;
        }

        string path = GameManager.GetAbPath(type);
        string local_path = string.Format("{0}/{1}/", KApplication.persistentDataPath, path);
        string net_path = string.Format("{0}/{1}/{2}", GameParams.Instance.AbDownLoadSite, KApplication.PlatformName, path);

        long local_version = -1;

        if (File.Exists(local_path + ABLoadConfig.VersionNO))
        {
            local_version = long.Parse(File.ReadAllText(local_path + ABLoadConfig.VersionNO));
        }

        List<AssetBundleInfo> local_list = null;
        if (File.Exists(local_path + ABLoadConfig.VersionPath))
        {
            try {
                string local_asset = File.ReadAllText(local_path + ABLoadConfig.VersionPath);
                local_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(local_asset);
            } catch {
                local_list = new List<AssetBundleInfo>();
            }
        }

        GameUtils.DownLoadTxt(TimeManager.Mono, net_path+ ABLoadConfig.VersionNO, null, (is_success, server_version) =>
        {
            if (long.Parse(server_version) > local_version)
            {
                MainEntrace.Instance.ShowLoad("正在检查资源...", 10);
                GameUtils.DownLoadTxt(TimeManager.Mono, net_path + ABLoadConfig.VersionPath, null, (success, assetStr) =>
                {
                    MainEntrace.Instance.HideLoad();
                    List<AssetBundleInfo> new_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(assetStr);
                    List<AssetBundleInfo> update_list = GetDiffList(local_path, local_list, new_list);
                    long size = 0;
                    foreach (var item in update_list)
                    {
                        size += item.Size;
                    }
                    if (dic_downsize.ContainsKey(type))
                        dic_downsize[type] = size;
                    else
                        dic_downsize.Add(type, size);

                    if (size > 0)
                    {
                        DownLoadAndEnterGame(local_path, net_path, update_list, new_list, server_version, type, assetStr, call);
                    }
                    else
                    {
                        EnterGame(type, new_list, call);
                    }
                });
            }
            else
            {
                EnterGame(type, local_list, call);
            }
        });
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    /// <param name="type"></param>
    /// <param name="asset_list"></param>
    /// <param name="call"></param>
    private static void EnterGame(GameEnum type,List<AssetBundleInfo> asset_list,VoidCall call)
    {
        string path = GameManager.GetAbPath(type);
        AssetBundleManager.LoadABSetting(path, asset_list);
        AddNoUpdate(type);

        if (type == GameEnum.Fish_3D)
        {
            OnLoad3DFishConfig(call, null);
        }
        else
        {
            call();
        }
    }

    /// <summary>
    /// 下载资源并进入游戏
    /// </summary>
    /// <param name="local_path"></param>
    /// <param name="net_path"></param>
    /// <param name="update_list"></param>
    /// <param name="new_list"></param>
    /// <param name="server_version"></param>
    /// <param name="type"></param>
    /// <param name="assetStr"></param>
    /// <param name="call"></param>
    private static void DownLoadAndEnterGame(string local_path,string net_path,List<AssetBundleInfo> update_list, List<AssetBundleInfo> new_list, string server_version,GameEnum type,string assetStr,VoidCall call)
    {
        string path = GameManager.GetAbPath(type);

        if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            WndManager.Instance.ShowDialog("现在处在非wifi情况下,是否继续下载?", (code) =>
            {
                if (code == 1)
                {
                    UI_DownloadRes_new downLoadUI = UI.EnterUI<UI_DownloadRes_new>(GameEnum.All);
                    downLoadUI.InitData(local_path, net_path, update_list, () =>
                    {
                        File.WriteAllText(local_path + ABLoadConfig.VersionNO, server_version);
                        File.WriteAllText(local_path + ABLoadConfig.VersionPath, assetStr);
                        AssetBundleManager.LoadABSetting(path, new_list);
                        AddNoUpdate(type);

                        if (type == GameEnum.Fish_3D)
                            OnLoad3DFishConfig(call, downLoadUI);
                        else
                            call();
                    }, type);
                }
            });
        }
        else
        {
            UI_DownloadRes_new downLoadUI = UI.EnterUI<UI_DownloadRes_new>(GameEnum.All);
            downLoadUI.InitData(local_path, net_path, update_list, () =>
            {
                File.WriteAllText(local_path + ABLoadConfig.VersionNO, server_version);
                File.WriteAllText(local_path + ABLoadConfig.VersionPath, assetStr);
                AssetBundleManager.LoadABSetting(path, new_list);
                AddNoUpdate(type);

                if (type == GameEnum.Fish_3D)
                    OnLoad3DFishConfig(call, downLoadUI);
                else
                    call();
            }, type);
        }
    }

    public static void Clear()
    {
        dic_check.Clear();
    }
    public static void AddNoUpdate(GameEnum type) {//添加不需要进行更新的列表，或者已经更新完成
        dic_check.Add(type, true);
        if (dic_downsize.ContainsKey(type)) {
            dic_downsize[type] = 0;
        } else {
            dic_downsize.Add(type, 0);
        }
    }

    /// <summary>
    /// 大厅资源下载
    /// </summary>
    /// <param name="type"></param>
    /// <param name="successCall"></param>
    /// <param name="call"></param>
    public static void CheckVersion(GameEnum type, Action successCall, Action<bool> call)
    {
#if UNITY_EDITOR

     /*   call.TryCall(false);
        successCall.TryCall();
        return;*/
#endif
/*        string path1 = GameManager.GetAbPath(type);
        var str = File.ReadAllText(Application.persistentDataPath + "/" + path1 + "assets.bytes");
        var list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(str);
        AssetBundleManager.LoadABSetting(path1, list);
        AddNoUpdate(type);
     
        call.TryCall(false);
        successCall.TryCall();
        return;*/
        string path = GameManager.GetAbPath(type);
        string local_path = string.Format("{0}/{1}/", KApplication.persistentDataPath, path);
        string net_path = string.Format("{0}/{1}/{2}", GameParams.Instance.AbDownLoadSite, KApplication.PlatformName, path);

        //本地版本文件路径
        string local_version_path = local_path + ABLoadConfig.VersionNO;
        //资源服务器版本文件路径
        string net_version_path = net_path + ABLoadConfig.VersionNO;

        long local_version = -1;

        if (File.Exists(local_version_path))
        {
            local_version = long.Parse(System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(local_version_path)));
        }

        List<AssetBundleInfo> local_list = null;

        if (File.Exists(local_path + ABLoadConfig.VersionPath))
        {
            try {
                string local_asset = File.ReadAllText(local_path + ABLoadConfig.VersionPath);
                local_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(local_asset);
            } catch {
                local_list = new List<AssetBundleInfo>();
            }
        }

        GameUtils.DownLoadTxt(TimeManager.Mono, net_version_path, null, (is_success, server_version) =>
        {
            if (is_success)
            {
                if (long.Parse(server_version) > local_version)
                {
                    GameUtils.DownLoadTxt(TimeManager.Mono, net_path + ABLoadConfig.VersionPath, null, (success, assets) =>
                    {
                        if (success)
                        {
                            List<AssetBundleInfo> net_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(assets);
                            List<AssetBundleInfo> update_list = GetDiffList(local_path, local_list,net_list);
                            long size = 0;
                            foreach (var item in update_list)
                            {
                                size += item.Size;
                            }
                            if (dic_downsize.ContainsKey(type))
                            {
                                dic_downsize[type] = size;
                            }
                            else
                            {
                                dic_downsize.Add(type, size);
                            }
                            string tick = string.Format("资源大小：[ff0000]{0}[-]", GetDownSpdStr(size));
                            if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
                            {
                                UI_VersionTickDialog.Show().InitData(tick, "(现在处在非wifi情况下,是否继续下载?)", () =>
                                {
                                    VersionManager.Instance.InitData(local_path, net_path, update_list, () =>
                                    {
                                        File.WriteAllText(local_path + ABLoadConfig.VersionNO, server_version);
                                        File.WriteAllText(local_path + ABLoadConfig.VersionPath, assets);
                                        AssetBundleManager.LoadABSetting(path, net_list);
                                        AddNoUpdate(type);
                                        successCall();
                                    });
                                });
                            }
                            else
                            {
                                UI_VersionTickDialog.Show().InitData(tick, "(建议在wifi环境下下载)", () =>
                                {
                                    VersionManager.Instance.InitData(local_path, net_path, update_list, () =>
                                    {
                                        File.WriteAllText(local_path + ABLoadConfig.VersionNO, server_version);
                                        File.WriteAllText(local_path + ABLoadConfig.VersionPath, assets);
                                        AssetBundleManager.LoadABSetting(path, net_list);
                                        AddNoUpdate(type);
                                        successCall();
                                    });
                                });
                            }
                        }
                    });
                }
                else
                {
                    AssetBundleManager.LoadABSetting(path, local_list);
                    AddNoUpdate(type);
                    successCall();
                }
            }
            else
            {
                if (dic_downsize.ContainsKey(type))
                    dic_downsize.Remove(type);
                LogMgr.LogWarning("版本文件version.bytes拉取失败");
            }
        });
    }

    private static string GetDownSpdStr(long size)
    {
        size = Math.Max(size, 0);
        if (size > 1000000)
        {
            //M
            return string.Format("{0:0.##}M", size / 1024f / 1024f);
        }
        else if (size > 1000)
        {
            //KB
            return string.Format("{0:0.##}KB", size / 1024f);
        }
        else
        {
            //B
            return string.Format("{0:0.##}B", size);
        }
    }

    private static void OnLoad3DFishConfig(VoidCall call,UI_DownloadRes_new resUpdateUI)
    {
        List<ResLoadItem> loadList = new List<ResLoadItem>();
        //如果是3D捕鱼那么资源下载完成后需要加载3D捕鱼的配置
        ConfigTables.Instance.Lunch3DfishConf(loadList);

        if (loadList.Count > 0)
        {
            if (resUpdateUI == null)
            {
                resUpdateUI = UI.EnterUI<UI_DownloadRes_new>(GameEnum.All);
            }
            resUpdateUI.mBtnCancel.GetComponent<BoxCollider>().enabled = false;
            resUpdateUI.mlbCancel.text = "加载配置";
            MonoDelegate.Instance.StartCoroutine(StartLoad3DFishConfig(loadList, call, resUpdateUI));
        }
        else
        {
            call();
        }
    }

    private static IEnumerator StartLoad3DFishConfig(List<ResLoadItem> loadList,VoidCall call,UI_DownloadRes_new resUpdateUI)
    {
        int error_count = 0;

        int configNum = loadList.Count;

        for (int i = 0; i < loadList.Count; i++)
        {
            error_count = 0;
            var itm = loadList[i];
            while (true)
            {
                try
                {
                    byte[] data = ResManager.LoadBytes(itm.configType, itm.resId);

                    itm.finishCb.TryCall(new BinaryAsset { bytes = data });
                    break;
                }
                catch (Exception e)
                {
                    LogMgr.LogError(e.Message);
                    LogMgr.LogError(e.StackTrace);
                    error_count++;
                }

                if (error_count > 3)
                {
                    LogMgr.LogError("文件加载失败：" + itm.resId);
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            float LoadProgress = (float)(i+1) / configNum;
            resUpdateUI.LoadConfig(LoadProgress);
            yield return null;
        }

        call();
    }

    private static List<AssetBundleInfo> GetDiffList(string local_path, List<AssetBundleInfo> local_list,  List<AssetBundleInfo> new_list)
    {
        List<AssetBundleInfo> list = new List<AssetBundleInfo>();
        Dictionary<string, AssetBundleInfo> dic_ab = new Dictionary<string, AssetBundleInfo>();
        if (local_list != null)
        {
            foreach (var item in local_list)
            {
                dic_ab.Add(item.ABname, item);
            }
        }
        AssetBundleInfo ab;
        foreach (var item in new_list)
        {
            if (dic_ab.TryGetValue(item.ABname, out ab) == false || item.MD5 != ab.MD5)
            {
                if (File.Exists(local_path + item.ABname) == false || Tools.GetMD5HashFromFile(local_path + item.ABname) != item.MD5)
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }
}
