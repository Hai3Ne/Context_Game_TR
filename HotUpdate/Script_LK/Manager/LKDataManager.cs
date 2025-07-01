using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKDataManager {
    public static Dictionary<Type, Dictionary<int, object>> dic_data = new Dictionary<Type, Dictionary<int, object>>();
    public static LK_Audio mAudio = null;

    public static void InitData() {
        dic_data.Clear();
        LKDataManager.Load<LK_FishVo>("LK_Fish");
        LKDataManager.Load<LK_LauncherVo>("LK_Launcher");

        mAudio = LKDataManager.Parse<LK_Audio>("LK_Audio");
    }
    public static void Clear() {
        dic_data.Clear();
    }

    public static T FindData<T>(int id) where T : class {
        Type type = typeof(T);
        Dictionary<int, object> __dic;
        if (dic_data.TryGetValue(type, out __dic) == false) {
#if UNITY_EDITOR
            Debug.LogError(string.Format("找不到该类型数据 type:{0}", type.ToString()));
#endif
            return null;
        }
        object obj;
        if (__dic.TryGetValue(id, out obj) == false) {
#if UNITY_EDITOR
            Debug.LogError(string.Format("找不到数据id:{0}  type:{1}", id, type.ToString()));
#endif
            return null;
        }
        return obj as T;
    }
    public static List<T> GetData<T>() where T : class {
        Type type = typeof(T);
        Dictionary<int, object> __dic;
        List<T> list = new List<T>();
        if (dic_data.TryGetValue(type, out __dic) == false) {
#if UNITY_EDITOR
            Debug.LogError(string.Format("找不到该类型数据 type:{0}", type.ToString()));
#endif
            return list;
        }
        foreach (var item in __dic.Values) {
            list.Add(item as T);
        }
        return list;
    }

    private static void Load<T>(string path) where T : new() {
        Type type = typeof(T);
        byte[] bytes = ResManager.LoadBytes(GameEnum.Fish_LK, LKGameConfig.DB_Path + path);
        Dictionary<int, T> dic = ConfigTables.Instance.SetobjectDic<int, T>(bytes);
        Dictionary<int,object> __dic;
        if (dic_data.TryGetValue(type,out __dic) == false) {
            __dic = new Dictionary<int,object>();
            dic_data.Add(type, __dic);
        }else{
            LogMgr.LogError("多次加载：" + type.ToString());
        }
        foreach (var item in dic) {
            __dic.Add(item.Key, item.Value);
        }
    }
    private static T Parse<T>(string path) where T : new() {
        byte[] bytes = ResManager.LoadBytes(GameEnum.Fish_LK, LKGameConfig.DB_Path + path);
        return ConfigTables.Instance.ParseKVData<T>(bytes);
    }
}
