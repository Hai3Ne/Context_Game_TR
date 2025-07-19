using System;
using UnityEngine;

using System.Collections.Generic;

/// <summary>
/// 雷霆万钧
/// </summary>
public class RandomInNumCollider : SkilCollider
{
	int randomTargetNum = 0;
	int maxMultip = 0;
	bool hasLighted = false;
	public override void Init (EffectVo vo, ColliderTestInputData inputargs)
	{
		base.Init (vo,inputargs);
		randomTargetNum = effVo.Value0;
		maxMultip = effVo.Value1;
		hasLighted = false;
	}

	public override List<ushort> ChckCollisonFish()
	{
		if (!isAvtive)
			return null;
		if (hasLighted)
			return null;
		List<Fish> avaibleFishlist = new List<Fish> ();
		List<Fish> selectFishs = new List<Fish> ();
        foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
			if (fish.IsInView_Center()) {
				avaibleFishlist.Add (fish);
			}
		}

		randomTargetNum = avaibleFishlist.Count > randomTargetNum ? randomTargetNum : avaibleFishlist.Count;
		int i = randomTargetNum;
		while (i > 0 && avaibleFishlist.Count > 0){
			var idx = UnityEngine.Random.Range (0, avaibleFishlist.Count);
			var fish = avaibleFishlist[idx];
			Utility.ListRemoveAt (avaibleFishlist, idx);
			if (fish.Catched || fish.IsDelay)
				continue;
			if (selectFishs.Contains (fish))
				continue;
			if (maxMultip > 0 && fish.vo.Multiple > maxMultip)
				continue;
			selectFishs.Add (fish);
			i--;
		}
		List<ushort> selFishIDList = new List<ushort> ();
		selectFishs.ForEach (x => selFishIDList.Add (x.FishID));
		hasLighted = true;
		return selFishIDList;
	}
}