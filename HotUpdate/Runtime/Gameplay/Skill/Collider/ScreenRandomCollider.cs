using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 地狱炼魂
/// </summary>
public class ScreenRandomCollider : SkilCollider
{
	bool hasLighted = false;
	uint fishCfgID,fishCfgID1;
	public override void Init (EffectVo vo, ColliderTestInputData inputargs)
	{
		base.Init (vo,inputargs);
		fishCfgID = (uint)vo.Value0;
		fishCfgID1 = (uint)vo.Value1;
		hasLighted = false;
	}

	public override List<ushort> ChckCollisonFish()
	{
		if (!isAvtive)
			return null;
		if (hasLighted)
			return null;
		
		List<ushort> arrestFishIDs = new List<ushort> ();
        foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
			if (fish.Catched || fish.IsDelay)
				continue;

			if (!chkFishCfgID (fish))
				continue;
			
			if (arrestFishIDs.Contains(fish.FishID))
				continue;
			if (fish.IsInView_Center())
				arrestFishIDs.Add (fish.FishID);
			else
			{
				//Debug.Log ("Rect:"+screenRect+" ScreenPos: "+fish.ScreenPos);
			}
		}
		hasLighted = true;
        //if (arrestFishIDs.Count == 0) {
        //    LogMgr.Log ("~~~~~~~~~~~~~~~ arrestFishIDs.Count == 0");
        //}
		return arrestFishIDs;
	}

	bool chkFishCfgID(Fish fish){
		if (fishCfgID == 0 && fishCfgID1 == 0)
			return true;
		if (fishCfgID != 0 && fish.FishCfgID == fishCfgID)
			return true;
		if (fishCfgID1 != 0 && fish.FishCfgID == fishCfgID1)
			return true;
		return false;
	}
}
