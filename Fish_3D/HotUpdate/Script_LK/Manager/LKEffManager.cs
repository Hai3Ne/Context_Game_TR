using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效管理
/// </summary>
public class LKEffManager {
    public static Dictionary<string, GameObject> dic_eff = new Dictionary<string, GameObject>();//prefab保存
    private static Dictionary<string, GameObject> dic_obj = new Dictionary<string, GameObject>();//游戏中粒子特效保存

    public const string scene_bg = "scene_bg";//场景背景
    public const string Anim_Gold = "ui/anim_gold";//金币特效
    public const string EF_table = "ui/EF_table";//欢迎提示
    public const string Eff_Gold = "ui/eff_gold";//鱼捕获飘字特效
    public const string Eff_RandomTotal = "ui/UI_RandomTotal";//随机倍率鱼收益统计
    public const string Eff_BigTotal = "ui/UI_BigTotal";//大鱼收益统计
    public const string Eff_BigFishTip = "ui/fish_appear_tip";//高倍鱼出现提示
    public const string Eff_LeaveTip = "ui/notice_tip_leave";//长时间未操作提示
    public const string Eff_Fatmosphere = "E0_Fatmosphere";//闪屏特效
    public const string Ef_hx = "Ef_hx";//海啸来袭
    public const string Eff_DingPing = "Eff_dingping";//定屏特效
    public const string Eff_ShanDian = "Eff_shandian";//闪电效果
    public const string Eff_BigDie = "Eff_BigDie";//大鱼死亡特效
    public const string Eff_RandomDie = "Eff_RandomDie";//随机倍率死亡特效
    public const string Eff_MengMianYu = "Eff_megnmianyu";//蒙面鱼王死亡特效
    public const string Eff_ShenYang = "Eff_shenyang";//神羊死亡特效
    public const string Eff_JinChan = "Eff_jinchan";//金蟾死亡特效
    public const string Eff_YanHua = "Eff_yanhua";//开场烟花特效
    public const string Eff_BoLang = "eff_bolang";//开场波浪特效
    public const string Eff_Boom = "Eff_boom";//小炸弹动画
    public const string Eff_BaoJi = "eff_baoji";//全屏炸弹暴击特效
    public const string eff_yuzhen = "eff_yuzhen";//圆形处于特效

    public static List<string> GetPreLoadList() {//获取预加载列表
        return new List<string>() {
            scene_bg,
            Anim_Gold,
            EF_table,
            Eff_Gold,
            Eff_RandomTotal,
            Eff_BigTotal,
            Eff_BigFishTip,
            Eff_LeaveTip,
            Eff_Fatmosphere,
            Ef_hx,
            Eff_DingPing,
            Eff_ShanDian,
            Eff_BigDie,
            Eff_RandomDie,
            Eff_MengMianYu,
            Eff_ShenYang,
            Eff_JinChan,
            Eff_YanHua,
            Eff_BoLang,
            Eff_Boom,
            Eff_BaoJi,
            eff_yuzhen,
        };
    }
    public static GameObject PreLoadEff(string path) {
        GameObject obj;
        if (dic_eff.TryGetValue(path, out obj) == false) {
            obj = ResManager.LoadAsset<GameObject>(GameEnum.Fish_LK, LKGameConfig.Effect_Path + path);
            dic_eff.Add(path, obj);
        }
        return obj;
    }
    public static void ClearEff() {//清除特效
        foreach (var item in dic_obj.Values) {
            if (item != null) {
                GameObject.Destroy(item);
            }
        }
        dic_obj.Clear();
        foreach (var item in dic_list) {
            foreach (var obj in item.Value.mUseList) {
                if (obj.Value != null) {
                    GameObject.Destroy(obj.Key);
                }
            }
            foreach (var obj in item.Value.mUnuseList) {
                if (obj != null) {
                    GameObject.Destroy(obj);
                }
            }
        }
        dic_list.Clear();
    }
    public static void Clear() {
        dic_eff.Clear();
        LKEffManager.ClearEff();
    }

    public static GameObject CreateEff(string path, Transform parent) {
        return GameObject.Instantiate(LKEffManager.PreLoadEff(path), parent);
    }

    public static void AddObjEff(string name, GameObject obj) {
        if (dic_obj.ContainsKey(name)) {
            GameObject old = dic_obj[name];
            if (old != null) {
                GameObject.Destroy(old);
            }
            dic_obj.Remove(name);
        }
        dic_obj.Add(name, obj);
    }
    public static void RemoveObjEff(string name) {
        GameObject obj;
        if (dic_obj.TryGetValue(name, out obj)) {
            GameObject.Destroy(obj);
            dic_obj.Remove(name);
        }
    }


    private class ObjList {
        public GameObject mPrefab;
        public Dictionary<GameObject, bool> mUseList = new Dictionary<GameObject, bool>();//已使用
        public List<GameObject> mUnuseList = new List<GameObject>();//未使用
    }
    private static Dictionary<string, ObjList> dic_list = new Dictionary<string, ObjList>();
    public static GameObject CreateObj(string path, Transform parent) {
        ObjList ol;
        if(dic_list.TryGetValue(path,out ol) == false){
            ol = new ObjList();
            ol.mPrefab = LKEffManager.PreLoadEff(path);
            dic_list.Add(path, ol);
        }

        GameObject obj;
        if (ol.mUnuseList.Count > 0) {
            obj = ol.mUnuseList[0];
            ol.mUnuseList.RemoveAt(0);
        } else {
            obj = GameObject.Instantiate(ol.mPrefab);
        }
        ol.mUseList.Add(obj,true);
        obj.SetActive(true);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = ol.mPrefab.transform.localPosition;
        obj.transform.localRotation = ol.mPrefab.transform.localRotation;
        obj.transform.localScale = ol.mPrefab.transform.localScale;

        return obj;
    }
    public static void Reback(string key, GameObject obj) {//回收
        ObjList ol;
        if (dic_list.TryGetValue(key, out ol)) {
            if (ol.mUseList.Remove(obj)) {
                ol.mUnuseList.Add(obj);
                obj.SetActive(false);
                obj.transform.SetParent(TimeManager.Mono.transform);
            }
        }
    }
    public static void Reback(string key, GameObject obj, float time) {//延迟回收
        TimeManager.DelayExec(time, () => {
            if (obj != null) {
                LKEffManager.Reback(key, obj);
            }
        });
    }
}
