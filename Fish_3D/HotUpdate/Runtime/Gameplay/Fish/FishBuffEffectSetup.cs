using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBuffEffectSetup
{
	static List<ClsTuple<GameObject, float, float>> effectGoList = new List<ClsTuple<GameObject, float, float>> ();
	public static void SetupDizzy(Fish fish, GameObject effGo)
	{
		Transform fishTrans = fish.Transform;
        Transform effContainer = fish.Anim.transform.Find("Eff_Dizzy");
		Vector3 locpos = Vector3.zero;
		float sc = 1f;
		if (effContainer == null) {
			effContainer = fish.Transform;
			BoxCollider bc = fishTrans.GetComponent<BoxCollider> ();
			Vector3[] corns = Fish.GetWorldCorners (bc);
			float minv = Mathf.Max (bc.size.x, bc.size.y);
			sc = minv * 0.1f * 0.5f;
			Vector3 wp = Vector3.Lerp (Vector3.Lerp (corns [1], corns [5], 0.5f), Vector3.Lerp (corns [2], corns [6], 0.5f), 0.2f);
			locpos = fishTrans.InverseTransformPoint (wp);
		}
		effGo.transform.SetParent (effContainer);
		effGo.transform.localPosition = locpos;
		effGo.transform.localScale = Vector3.one * sc;
	}

	public static void SetupDizzy(Transform fishTrans){
		Animator Anim = fishTrans.GetComponentInChildren<Animator> ();
		Transform effContainer = Anim.transform.Find ("Eff_Dizzy");
		if (effContainer != null)
        {
            //string rid = string.Format (ResPath.SkillEff, 11021);
            //Kubility.KAssetBundleManger.Instance.LoadGameObject (rid, delegate(SmallAbStruct obj)
            //         {
            //	GameObject effGo =  GameObject.Instantiate(obj.MainObject as GameObject);
            //	effGo.transform.SetParent (effContainer);
            //	effGo.transform.localPosition = Vector3.zero;
            //	effGo.transform.localScale = Vector3.one;
            //});

            string rid = string.Format (ResPath.NewSkillEff, 11021);

            GameObject go = ResManager.LoadAsset<GameObject>(GameEnum.Fish_3D,rid);
            GameObject effGo = GameObject.Instantiate(go as GameObject);
            effGo.transform.SetParent(effContainer);
            effGo.transform.localPosition = Vector3.zero;
            effGo.transform.localScale = Vector3.one;
        }
	}

	public static void SetupFrozen(Fish fish, GameObject iceGo, float duration = -1f)
	{
		iceGo.transform.SetParent (fish.Transform);

        BoxCollider fishBox = fish.Collider;
		BoxCollider iceBox = iceGo.GetComponent<BoxCollider> ();

        //缩放
        Vector3 fish_ls = fishBox.transform.lossyScale;
        Vector3 ice_ls = iceBox.transform.lossyScale;
        Vector3 ls = Vector3.one;
        ls.x = fish_ls.x * fishBox.size.x / (ice_ls.x * iceBox.size.x);
        ls.y = fish_ls.y * fishBox.size.y / (ice_ls.y * iceBox.size.y);
        ls.z = fish_ls.z * fishBox.size.z / (ice_ls.z * iceBox.size.z);
        ice_ls = iceBox.transform.localScale;
        iceBox.transform.localScale = new Vector3(ls.x * ice_ls.x, ls.y * ice_ls.y, ls.z * ice_ls.z)*1.1f;

        //方向
        iceBox.transform.rotation = fishBox.transform.rotation;

        //坐标
        iceBox.transform.position = fishBox.transform.TransformPoint(fishBox.center);
        iceBox.transform.position = iceBox.transform.TransformPoint(-iceBox.center);
		effectGoList.Add (new ClsTuple<GameObject, float, float>(iceGo, 0f, duration));
	}

	static Vector3[] CalBoxWorldCorns(BoxCollider box)
	{
		Vector3[] corns = new Vector3[ConstValue.BoxCorns.Length];
		for (byte i = 0; i < corns.Length; i++) 
		{
			corns [i] = box.center + Vector3.Scale(ConstValue.BoxCorns [i], box.size * 0.5f);
			corns [i] = box.transform.localToWorldMatrix.MultiplyPoint (corns [i]);
		}
		return corns;
	}


	public static void Update(float delta)
	{
		for (int i = 0; i < effectGoList.Count;) {
			if (effectGoList [i].field2 > 0f) {
				effectGoList [i].field1 += delta;
				if (effectGoList [i].field1 >= effectGoList [i].field2) {
					GameObject.Destroy (effectGoList [i].field0);
					Utility.ListRemoveAt(effectGoList, i);
					continue;
				}
			}
			++i;
		}
	}

	public static void Clear()
	{
		for (int i = 0; i < effectGoList.Count; i++) {
			GameObject.Destroy (effectGoList [i].field0);
		}
		effectGoList.Clear ();
	}
}