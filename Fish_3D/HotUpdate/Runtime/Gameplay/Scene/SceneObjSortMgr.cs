using System;
using UnityEngine;
using System.Collections.Generic;
public interface ISortZObj
{
	Vector3 Position{ get;}
	void SetRenderQueue(int queue);
	void SetMatIntVal(string nameID, int val);
}


public class SceneObjSortMgr : ISceneMgr
{
	
	public void Init (){}
	public void Shutdown()
	{
		
	}

	public void Update (float delta)
	{
		if (MainEntrace.Instance.issort)
			SortDepth (delta);		
	}

	float sorttime = 0f;
	void SortDepth(float delta)
	{
		sorttime += delta;
		if (sorttime < 2f)
			return;
		sorttime = 0f;

		List<Fish> inViewFishList = SceneLogic.Instance.FishMgr.GetViewFishTrans();
		List<ISortZObj> list = SceneLogic.Instance.HeroMgr.GetFishHeroObjs ();
        for (int i = 0; i < inViewFishList.Count; i++) {
            list.Add(inViewFishList[i]);
        }
        
        int val;
        foreach (var item in list) {
            val = Mathf.FloorToInt(item.Position.z * 0.05f);
            item.SetRenderQueue(2500+val);
            item.SetMatIntVal("_RefVal", -val);
        }
        //list.Sort(delegate(ISortZObj A, ISortZObj B) {
        //    if (A != null && B != null) {
        //        int az = Mathf.FloorToInt(A.Position.z * 0.05f);
        //        int bz = Mathf.FloorToInt(B.Position.z * 0.05f);
        //        return bz.CompareTo(az);
        //    }
        //    return 0;
        //});

        //int val = 1;
        //int queue = 2000 + list.Count;
        //for (int i = 0; i < list.Count; i++) {
        //    list[i].SetRenderQueue(queue);
        //    list[i].SetMatIntVal("_RefVal", val);
        //    val++;
        //    queue--;
        //}
	}

}
