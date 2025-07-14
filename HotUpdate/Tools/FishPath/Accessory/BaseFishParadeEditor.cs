using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
public class BaseFishParadeEditor : MonoBehaviour
{
	public IFishGraphic mFishGraphic;
    //bool zReverse = false;
	List<GameObject> fishGoList = new List<GameObject>();
	Dictionary<int, GameObject> fishInfoMap = new Dictionary<int, GameObject> ();
	public void UpdateSceneObjStatus(FishParadeData FishParadeData)
	{
		Transform mTrans = this.transform;

		CFishDataConfigManager mgr = GameObject.FindObjectOfType<CFishDataConfigManager> ();
		if (mgr == null || mgr.transform.childCount <= 0 || FishParadeData == null)
			return;
		Transform tt = mgr.transform.GetChild (0);
		int len = FishParadeData.GroupDataArray.Length;
		ClearFish ();
		for (int eidx = 0; eidx < len; eidx++) 
		{
			uint pathID = 0;
			if (eidx < FishParadeData.PathList.Length)
				pathID = FishParadeData.PathList[eidx];
			
			var gdata = FishParadeData.GroupDataArray[eidx];
			Transform pathTran = tt.Find ("path" + pathID);
			CPathLinearRender lrender = pathTran.GetComponent<CPathLinearRender> ();
			CPathLinearRender.OnlyShow (lrender);
			lrender.RefreshData ();

            //zReverse = false;
			Quaternion rot;
			if (pathTran.GetChild (0).position.x > pathTran.GetChild (pathTran.childCount - 1).position.x) {
				rot = Quaternion.Euler (Vector3.zero);
                //zReverse = true;
			}
			else
				rot = Quaternion.Euler (Vector3.up * 180f);
			
			mTrans.position = pathTran.GetChild (0).position;

	
			int k = eidx;
			GameObject prefab = GetPrefab (gdata.FishCfgID);
			FishVo fishvo = CFishPathEditor.only.GetFishVo (gdata.FishCfgID);
			if (prefab == null)
				return;
            if (gdata.FishShapeID > 0) {
                string path = string.Format("Assets/" + ResPath.FishShapePath + ".prefab", gdata.FishShapeID);
                GameObject shape_prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (shape_prefab == null) {
                    Debug.LogError("形状模型加载错误：" + path);
                }
                GameObject fish_content = GameObject.Instantiate(shape_prefab);
                fish_content.transform.SetParent(mTrans);
                fish_content.transform.localPosition = Vector3.zero;
                fish_content.transform.localScale *= gdata.ShapeScale;
                MeshFilter[] mfs = fish_content.GetComponentsInChildren<MeshFilter>();
                fishGoList.Add(fish_content);

                List<Vector3> all_pos_list = new List<Vector3>();
                List<Vector3> pos_list;
                Vector3 pos;
                foreach (var item in mfs) {
                    pos_list = GameUtils.CreateFishPos(item.sharedMesh, gdata.Density);

                    for (int i = 0; i < pos_list.Count; i++) {
                        pos = mTrans.InverseTransformPoint(item.transform.TransformPoint(pos_list[i]));
                        all_pos_list.Add(pos);

                        GameObject fgo = GameObject.Instantiate(prefab);
                        fgo.transform.SetParent(mTrans);
                        fgo.transform.localPosition = pos + gdata.ShapeOffset;
                        fgo.transform.localScale = Vector3.one * gdata.FishScaling * fishvo.Scale;
                        fgo.transform.localRotation = rot;
                        fgo.transform.name = "Fish" + gdata.FishCfgID;
                        fishGoList.Add(fgo);
                        fgo.AddComponent<SPoint>();
                        int v = (k << 16) | i;
                        fishInfoMap.Add(v, fgo);
                    }
                }
                gdata.FishNum = (ushort)all_pos_list.Count;
                gdata.PosList = all_pos_list.ToArray();
            } else {
                for (int i = 0; i < gdata.PosList.Length; i++) {
                    Vector3 poss = new Vector3(gdata.PosList[i].x, gdata.PosList[i].y, gdata.PosList[i].z);
                    //poss.z *= (zReverse ? -1f : 1f);
                    GameObject fgo = GameObject.Instantiate(prefab);
                    fgo.transform.SetParent(mTrans);
                    fgo.transform.localPosition = (i < gdata.PosList.Length ? poss : Vector3.zero);
                    fgo.transform.localScale = Vector3.one * gdata.FishScaling * fishvo.Scale;
                    fgo.transform.localRotation = rot;
                    fgo.transform.name = "Fish" + gdata.FishCfgID;
                    fishGoList.Add(fgo);
                    fgo.AddComponent<SPoint>();
                    int v = (k << 16) | i;
                    fishInfoMap.Add(v, fgo);
                }
            }
		}
	}

	public void ClearFish()
	{
		while (fishGoList.Count > 0) {
			GameObject.DestroyImmediate (fishGoList [0]);
			fishGoList.RemoveAt (0);
		}
		fishInfoMap.Clear ();
	}
	public void GetPoslist(FishParadeData FParadeData)
	{
		int k = 0;
		Transform mTrans = this.transform;
		if (mTrans.childCount > 0) 
		{
			for (int i = 0; i < FParadeData.GroupDataArray.Length; i++) 
			{
				for (int j = 0; j < FParadeData.GroupDataArray [i].PosList.Length; j++) {					
					if (k < mTrans.childCount) 
					{
						Transform ch = mTrans.GetChild (k);	
						FParadeData.GroupDataArray [i].PosList [j] = ch.localPosition;
						k++;
					} else {
						FParadeData.GroupDataArray [i].PosList [j] = Vector3.zero;
					}
				}
			}
		}
		if (mFishGraphic != null)
			FParadeData.FishGraphicData = mFishGraphic.GraphicData;
		/*
		foreach (var kv in fishInfoMap) {
			int i = kv.Key >> 16;
			int j = kv.Key & 0xFF;
			if (j >= FParadeData.GroupDataArray [i].PosList.Length) {
				FParadeData.GroupDataArray [i].PosList = ArrangesizeVector3 (FParadeData.GroupDataArray [i].PosList, j+1);
			}
			Vector3 fishpos = kv.Value.transform.localPosition;
			fishpos.z = (zReverse ? -1f : 1f) * fishpos.z;
			FParadeData.GroupDataArray [i].PosList [j] = fishpos;
		}
		//*/
	}

	GameObject GetPrefab(uint fcfgID)
	{
		#if UNITY_EDITOR
		string fishPath = "Assets/Arts/GameRes/Prefabs/Models/Fish{0}.prefab";
		string resid = string.Format (fishPath, fcfgID);
		GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject> (resid);
		return prefab;		
		#else
		return null;
		#endif
	}

	Vector3[] ArrangesizeVector3(Vector3[] array, int len)
	{
		Vector3[] newAry = new Vector3[len];
		int i = array.Length;
		for (int j = 0; j < newAry.Length; j++) {
			if (j < array.Length)
				newAry [j] = array [j];
			else
				newAry [j] = Vector3.zero;
		}
		return newAry;
	}

	float[] ArrangesizeFloat(float[] array, int len)
	{
		float[] newAry = new float[len];
		for (int j = 0; j < newAry.Length; j++) {
			if (j < array.Length)
				newAry [j] = array [j];
			else
				newAry [j] = 0;
		}
		return newAry;
	}

	uint[] ArrangesizeUshort(uint[] array, int len)
	{
		uint[] newAry = new uint[len];
		for (int j = 0; j < newAry.Length; j++) {
			if (j < array.Length)
				newAry [j] = array [j];
			else
				newAry [j] = 0;
		}
		return newAry;
	}

	public void CheckDataOK(FishParadeData fParadeData)
	{
		if (fParadeData.PathList.Length < fParadeData.GroupDataArray.Length && fParadeData.GroupDataArray.Length > 0) {
			fParadeData.PathList = ArrangesizeUshort(fParadeData.PathList, fParadeData.GroupDataArray.Length);
		}

		for (int i = 0; i < fParadeData.GroupDataArray.Length; i++) 
		{
			if (fParadeData.GroupDataArray [i].FishNum > 0 &&
			    fParadeData.GroupDataArray [i].PosList.Length < fParadeData.GroupDataArray [i].FishNum) {
				fParadeData.GroupDataArray [i].PosList = ArrangesizeVector3 (fParadeData.GroupDataArray [i].PosList, fParadeData.GroupDataArray [i].FishNum);
			}

			if (fParadeData.GroupDataArray [i].FishNum > 0 &&
				fParadeData.GroupDataArray [i].DelayList.Length < fParadeData.GroupDataArray [i].FishNum) {
				fParadeData.GroupDataArray [i].DelayList = ArrangesizeFloat (fParadeData.GroupDataArray [i].DelayList, fParadeData.GroupDataArray [i].FishNum);
			}
		}


		
	}
}

#endif