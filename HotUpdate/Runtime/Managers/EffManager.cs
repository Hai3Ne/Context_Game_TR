using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffManager {
    private static Dictionary<int, GameObject> dic_eff = new Dictionary<int, GameObject>();
    public static void AddEff(int id, GameObject obj) {
        dic_eff.Add(id, obj);
    }
    public static void RemoveEff(int id) {
        dic_eff.Remove(id);
    }
    public static void ClearAll() {
        List<GameObject> list = new List<GameObject>(dic_eff.Values);
        for (int i = 0; i < list.Count; i++) {
            GameObject.Destroy(list[i]);
        }
        dic_eff.Clear();
    }

    /// <summary>
    /// 手机震动
    /// </summary>
    public static void Vibrate() {
        if (GameConfig.OP_Shake) {//调用手机震动
            //Handheld.Vibrate();
        }
    }
}
