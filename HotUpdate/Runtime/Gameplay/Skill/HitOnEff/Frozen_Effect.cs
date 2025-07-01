using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frozen_Effect : SpeedAlta_Effect
{
	protected override void Start ()
	{
		base.Start ();

	}

	protected override void FilterTargetFishes(List<CatchFishData> catchDatas)
	{
		mTargetFishList.Clear ();
		List<Fish> kingFishes = new List<Fish> ();
		foreach (var fid in mEffectArgs.FishList) {

			if (fid.FishObj != null) {
				mTargetFishList.Add (fid.FishObj);
				if (fid.FishObj.IsKing)
					kingFishes.Add (fid.FishObj);
			}
		}

		foreach(var ff in kingFishes)
			SceneLogic.Instance.FishMgr.SearchInvoleFishes (ff, mTargetFishList);
	}

	protected override GameObjRef AddFishEffect(Fish fish, uint effectid)
	{
		GameObjRef gameRef = base.AddFishEffect (fish, effectid);
		if (gameRef == null)
			return null;
		FishBuffEffectSetup.SetupFrozen (fish, gameRef.mTrans);
		return gameRef;
	}
}
