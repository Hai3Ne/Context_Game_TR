using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjPoolsItem
{
	public Vector3 initPositon, initScale;
	public Quaternion initRot;
	public List<GameObject> usinggGoList = new List<GameObject>();
	public List<GameObject> unActivegGoList = new List<GameObject>();
}

public class GameObjectPools
{
	static Dictionary<int, GameObjPoolsItem> mGoPoolsDicts = new Dictionary<int, GameObjPoolsItem>();
	public static GameObject GetGameObjInst(GameObject prefab)
	{
		GameObjPoolsItem poolItem;
		int prefabId = prefab.GetInstanceID ();
        if (mGoPoolsDicts.TryGetValue(prefabId, out poolItem) == false) {
			poolItem = new GameObjPoolsItem ();
			mGoPoolsDicts [prefabId] = poolItem;
		}
		
		GameObject inst = null;
		if (poolItem.unActivegGoList.Count > 0) 
		{
			inst = poolItem.unActivegGoList [0];
			poolItem.unActivegGoList.RemoveAt (0);
			if (!inst.activeSelf)
				inst.SetActive (true);
			inst.transform.position = poolItem.initPositon;
			inst.transform.localScale = poolItem.initScale;
			inst.transform.rotation = poolItem.initRot;
		}
		else {
            inst = GameObject.Instantiate(prefab, SceneObjMgr.Instance.UIPanelTransform);
            if (!inst.activeSelf)
                inst.SetActive(true);
			poolItem.initPositon = inst.transform.position;
			poolItem.initScale = inst.transform.localScale;
			poolItem.initRot = inst.transform.rotation;
		}
		poolItem.usinggGoList.Add (inst);
		return inst;
	}

    public static void Reback(GameObject go, Action<GameObject> hideFn = null) {
        //int goInstId = go.GetInstanceID();
        foreach (var kk in mGoPoolsDicts) {
            if (kk.Value.usinggGoList.Remove(go)) {
                kk.Value.unActivegGoList.Add(go);
                go.SetActive(false);
            }
        }
    }
}
