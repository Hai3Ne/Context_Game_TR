using UnityEngine;
using System.Collections;
using Kubility;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 资源管理
/// </summary>
public class ResManager {

    public static byte[] LoadBytes(GameEnum type, string path) {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        return File.ReadAllBytes(path + ".byte");
#else
        return File.ReadAllBytes(string.Format("{0}/{1}{2}", KApplication.persistentDataPath, GameManager.GetAbPath(type), AssetBundleManager.PathToAbName(path)));
#endif
    }
    public static string[] LoadLines(GameEnum type, string path) {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        return File.ReadAllLines(path + ".dat");
#else
        return File.ReadAllLines(string.Format("{0}/{1}{2}", KApplication.persistentDataPath, GameManager.GetAbPath(type), AssetBundleManager.PathToAbName(path)));
#endif
    }

    private static void AddNeed(AssetBundleInfo ab_info)
    {
        if(dic_need.ContainsKey(ab_info.ABname))
            dic_need[ab_info.ABname]++;
        foreach (var item in ab_info.DependABList) {
            ResManager.AddNeed(item);
        }
    }

    public static void LoadText(GameEnum type, string path,out string text)
    {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        path += ".txt";
        TextAsset asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        text = asset.text;
        return;
#endif
        AssetBundleInfo ab_info = AssetBundleManager.GetABInfoByPath(path);

        string filePath = string.Format("{0}/{1}", Application.persistentDataPath, ab_info.FullPath);

        AssetBundle ab = AssetBundle.LoadFromFile(filePath);

        TextAsset assetText = ab.LoadAsset<TextAsset>(Path.GetFileNameWithoutExtension(path));

        text = assetText.text;
        ab.Unload(true);
    }

    public static void LoadAsset<T>(string path, VoidCall<AssetBundleInfo,T> call,GameEnum type) where T : UnityEngine.Object
    {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        if (File.Exists(path + ".prefab"))
        {
            path = path + ".prefab";
        }
        else
        {
            FileInfo file = new FileInfo(path);
            FileInfo[] files = file.Directory.GetFiles(file.Name + ".*");
            file = null;
            foreach (var item in files)
            {
                if (item.Name.EndsWith(".meta"))
                {
                    continue;
                }
                file = item;
                break;
            }
            if (file != null)
            {
                path += file.Extension;
            }
        }

        T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

        if (call != null)
            call(null, asset);
        return;
#endif
        AssetBundleInfo ab_info = AssetBundleManager.GetABInfoByPath(path);
        if (ab_info == null)
        {
            call(null, null);
            return;
        }
        AssetBundle ab;
        UnityEngine.Object obj;
        if (dic_obj.TryGetValue(ab_info.ABname, out obj) == false)
        {
            ab = ResManager.LoadAB(ab_info);
            obj = ab.LoadAsset<T>(Path.GetFileNameWithoutExtension(path));
            if (dic_count.ContainsKey(ab_info.ABname))
            {
                dic_count[ab_info.ABname]++;
            }
            else
            {
                dic_count.Add(ab_info.ABname, 1);
            }
        }
        else
        {
            if (dic_bundle.TryGetValue(ab_info.ABname, out ab))
            {
                ResManager.AddNeed(ab_info);
            }
        }
        ResManager.mLoadList.Add(ab_info);
        ResManager.Save(ab_info.ABname, obj);

        if (call != null)
            call(ab_info, obj as T);
    }

    public static T LoadAsset<T>(GameEnum type, string path) where T : UnityEngine.Object
    {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        if (File.Exists(path + ".prefab"))
        {
            path = path + ".prefab";
        }
        else
        {
            FileInfo file = new FileInfo(path);
            FileInfo[] files = file.Directory.GetFiles(file.Name + ".*");
            file = null;
            foreach (var item in files) {
                if (item.Name.EndsWith(".meta"))
                {
                    continue;
                }
                file = item;
                break;
            }
            if (file!=null)
            {
                path += file.Extension;
            }
        }
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#endif
        AssetBundleInfo ab_info = AssetBundleManager.GetABInfoByPath(path);
        if (ab_info == null)
        {
            return null;
        }
        AssetBundle ab;
        UnityEngine.Object obj;
        if (dic_obj.TryGetValue(ab_info.ABname, out obj) == false)
        {
            ab = ResManager.LoadAB(ab_info);
            obj = ab.LoadAsset<T>(Path.GetFileNameWithoutExtension(path));
            if (dic_count.ContainsKey(ab_info.ABname))
            {
                dic_count[ab_info.ABname]++;
            }
            else
            {
                dic_count.Add(ab_info.ABname, 1);
            }
        }
        else
        {
            if (dic_bundle.TryGetValue(ab_info.ABname, out ab))
            {
                ResManager.AddNeed(ab_info);
            }
        }
        ResManager.mLoadList.Add(ab_info);
        ResManager.Save(ab_info.ABname, obj);
        return obj as T;
    }

    public static void LoadAsyn<T>(string path, VoidCall<AssetBundleInfo, T> call, GameEnum type) where T : UnityEngine.Object
    {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        if (File.Exists(path + ".prefab"))
        {
            path = path + ".prefab";
        }
        else
        {
            FileInfo file = new FileInfo(path);
            FileInfo[] files = file.Directory.GetFiles(file.Name + ".*");
            file = null;
            foreach (var item in files)
            {
                if (item.Name.EndsWith(".meta"))
                {
                    continue;
                }
                file = item;
                break;
            }
            if (file != null)
            {
                path += file.Extension;
            }
        }
        call(null, UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path));
        return;
#endif
        AssetBundleInfo ab_info = AssetBundleManager.GetABInfoByPath(path);
        if (ab_info == null)
        {
            call(null, null);
            return;
        }

        AssetBundle ab;
        UnityEngine.Object obj;
        if (dic_obj.TryGetValue(ab_info.ABname, out obj) == false)
        {
            if (dic_bundle.TryGetValue(ab_info.ABname, out ab))
            {
                obj = ab.LoadAsset<T>(Path.GetFileNameWithoutExtension(path));
                ResManager.AddNeed(ab_info);
            }
            else
            {
                TimeManager.Mono.StartCoroutine(_load_asyn_res(ab_info, path, call));
                return;
            }
        }
        else
        {
            if (dic_bundle.TryGetValue(ab_info.ABname, out ab))
            {
                ResManager.AddNeed(ab_info);
            }
        }
        _load_ab(obj, ab_info);
        call(ab_info, obj as T);
    }

    private static AssetBundle LoadAB(AssetBundleInfo ab_info)
    {
        if (ab_info.DependABList.Count > 0)
        {
            foreach (var item in ab_info.DependABList)
            {
                ResManager.LoadAB(item);
            }
        }
        AssetBundle ab;
        if (dic_bundle.TryGetValue(ab_info.ABname, out ab) == false)
        {
            ab = AssetBundle.LoadFromFile(string.Format("{0}/{1}", KApplication.persistentDataPath, ab_info.FullPath));
            dic_bundle.Add(ab_info.ABname, ab);
            dic_need.Add(ab_info.ABname, 1);
        }
        else
        {
            dic_need[ab_info.ABname]++;
        }
        return ab;
    }

    private static void _load_ab(UnityEngine.Object t, AssetBundleInfo ab_info) {
        if (dic_count.ContainsKey(ab_info.ABname)) {
            dic_count[ab_info.ABname]++;
        } else {
            dic_count.Add(ab_info.ABname, 1);
        }
        ResManager.Save(ab_info.ABname, t);
        ResManager.mLoadList.Add(ab_info);
    }

    private static IEnumerator _load_asyn_res<T>(AssetBundleInfo ab_info, string path, VoidCall<AssetBundleInfo,T> call) where T : UnityEngine.Object
    {
        while (ab_info.ab_request != null)
        {
            yield return null;
        }

        int count = ab_info.DependABList.Count;
        if (count > 0)
        {
            foreach (var item in ab_info.DependABList)
            {
                yield return TimeManager.Mono.StartCoroutine(_load_anyn_main(item));
            }
        }

        yield return TimeManager.Mono.StartCoroutine(_load_anyn_main(ab_info));
        T obj = dic_bundle[ab_info.ABname].LoadAsset<T>(Path.GetFileNameWithoutExtension(path));
        _load_ab(obj, ab_info);
        call(ab_info,obj as T);
    }

    private static IEnumerator _load_anyn_main(AssetBundleInfo ab_info)
    {
        while (ab_info.ab_request != null)
        {
            yield return null;
        }
        AssetBundle ab;

        if (dic_bundle.TryGetValue(ab_info.ABname, out ab))
        {
            dic_need[ab_info.ABname]++;
        }
        else
        {
            string path = string.Format("{0}/{1}", KApplication.persistentDataPath, ab_info.FullPath);

            ab_info.ab_request = AssetBundle.LoadFromFileAsync(path);
            yield return ab_info.ab_request;
            ab = ab_info.ab_request.assetBundle;
            if (ab != null)
            {
                dic_bundle.Add(ab_info.ABname, ab);
                dic_need.Add(ab_info.ABname, 1);
            }
            ab_info.ab_request = null;
        }
    }
    public static void UnloadAB(AssetBundleInfo ab_info,bool is_unload_all = false)
    {
        int count;
        string key = ab_info.ABname;

        if (dic_count.TryGetValue(key, out count))
        {
            if (count > 1)
            {
                dic_count[key]--;
                return;
            }
            dic_count.Remove(key);
        }
        else
        {
            return;
        }

        if (dic_bundle.ContainsKey(key) == false)
        {
            return;
        }

        if (dic_need.TryGetValue(key, out count))
        {
            if (count > 1)
            {
                dic_need[key]--;
            }
            else
            {
                dic_bundle[key].Unload(is_unload_all);
                dic_bundle.Remove(key);
                dic_need.Remove(key);
            }
        }
        for (int i = 0; i < ab_info.DependABList.Count; i++)
        {
            key = ab_info.DependABList[i].ABname;
            if (dic_need.TryGetValue(key, out count))
            {
                if (count > 1)
                {
                    dic_need[key]--;
                }
                else
                {
                    dic_bundle[key].Unload(is_unload_all);
                    dic_bundle.Remove(key);
                    dic_need.Remove(key);
                }
            }
        }
    }
    
    public static List<AssetBundleInfo> mLoadList = new List<AssetBundleInfo>();
    public static void ClearLoadList() {//清除已加载列表
        foreach (var item in mLoadList)
        {
            if(item != null)
                ResManager.UnloadAB(item);
        }
        mLoadList.Clear();
    }
    public static void Clear() {
        ResManager.ClearLoadList();
        dic_obj.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }






    //捕鱼3D专用
    public static ResList __res;
    public static Dictionary<string, UnityEngine.Object> dic_obj = new Dictionary<string, UnityEngine.Object>();//已加载资源
    public static List<UnityEngine.Object> save_list = new List<UnityEngine.Object>();//常驻列表
    public static Dictionary<string, int> dic_count = new Dictionary<string, int>();//加载次数
    public static Dictionary<string, AssetBundle> dic_bundle = new Dictionary<string, AssetBundle>();//assetbundle缓存列表
    public static Dictionary<string, int> dic_need = new Dictionary<string, int>();//引用次数列表
    public class ResList : MonoBehaviour
    {
        public Dictionary<string, UnityEngine.Object> dic_obj;//已加载资源
        public List<UnityEngine.Object> save_list;
    }
    public static void Save(string ab_name, UnityEngine.Object obj)
    {
        //保存资源  防止被释放
        if (__res == null)
        {
            __res = new GameObject("res_list").AddComponent<ResList>();
            GameObject.DontDestroyOnLoad(__res.gameObject);
            __res.dic_obj = dic_obj;
            __res.save_list = save_list;
        }
        if(obj != null)
            dic_obj[ab_name] = obj;
    }

    public static void UnloadAB<T>(string path) where T : UnityEngine.Object
    {
        //ResManager.UnloadAB(KAssetBundleManger.Instance.ReadFromCache<T>(path));

        AssetBundleInfo assetBundleInfo = FindAssetBundleInfo(path,GameEnum.Fish_3D);

        if (assetBundleInfo != null)
        {
            UnloadAB(assetBundleInfo);
        }
    }

    public static AssetBundleInfo FindAssetBundleInfo(string path,GameEnum type)
    {
#if UNITY_EDITOR
        return null;
#endif
        path = GameManager.GetResPath(type) + path;
        AssetBundleInfo ab_info = AssetBundleManager.GetABInfoByPath(path);
        if (ab_info == null)
            return null;
        AssetBundle ab;
        UnityEngine.Object obj;
        if (dic_obj.TryGetValue(ab_info.ABname, out obj) == false)
        {
            return null;
        }

        return ab_info;
    }

    //private static void AddNeed(ABData data)
    //{
    //    dic_need[data.Abname]++;
    //    foreach (var item in data.MyNeedDepends)
    //    {
    //        if (Kubility.KAssetBundleManger.Instance.CacheContains(item))
    //        {
    //            ResManager.AddNeed(Kubility.KAssetBundleManger.Instance.ReadFromCache<UnityEngine.Object>(item));
    //        }
    //    }
    //}


    public static GameObject LoadAndCreate(GameEnum type, string path, Transform parent)
    {
        path = GameManager.GetResPath(type) + path;
#if UNITY_EDITOR
        if (File.Exists(path + ".prefab"))
        {
            path = path + ".prefab";
        }
        else
        {
            FileInfo file = new FileInfo(path);
            FileInfo[] files = file.Directory.GetFiles(file.Name + ".*");
            file = null;
            foreach (var item in files)
            {
                if (item.Name.EndsWith(".meta"))
                {
                    continue;
                }
                file = item;
                break;
            }
            if (file != null)
            {
                path += "." + file.Extension;
            }
        }
        return GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path), parent);
        //#else
#endif
        AssetBundleInfo ab_info = AssetBundleManager.GetABInfoByPath(path);
        if (ab_info == null)
        {
            return null;
        }
        AssetBundle ab;
        UnityEngine.Object obj;
        if (dic_obj.TryGetValue(ab_info.ABname, out obj) == false)
        {
            ab = ResManager.LoadAB(ab_info);
            obj = ab.LoadAsset<GameObject>(Path.GetFileNameWithoutExtension(path));
            if (dic_count.ContainsKey(ab_info.ABname))
            {
                dic_count[ab_info.ABname]++;
            }
            else
            {
                dic_count.Add(ab_info.ABname, 1);
            }
        }
        else
        {
            if (dic_bundle.TryGetValue(ab_info.ABname, out ab))
            {
                ResManager.AddNeed(ab_info);
            }
        }

        ResManager.Save(ab_info.ABname, obj);
        GameObject go = GameObject.Instantiate(obj as GameObject, parent);
        if (ab != null)
        {
            go.AddComponent<ResCount>().ab_info = ab_info;
        }
        return go;
    }
}
