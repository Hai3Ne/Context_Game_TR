using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PirateBoxCatchCollider: SkilCollider
{
	Vector3 mScreenCenterPos;
	float range;
	int colliderChkTimes;
    private int mMaxMul;//能捕获到的最大鱼的倍率
	public override void Init (EffectVo vo, ColliderTestInputData inputargs)
	{
		base.Init (vo,inputargs);
        this.mMaxMul = vo.Value2;
		range = (float)vo.Value3;
		mScreenCenterPos = mInputArgs.inputScreenPos;
		InitShapeColliderTest (ColliderShape.Circle, mScreenCenterPos, range);
		colliderChkTimes = 0;
	}

	public override List<ushort> ChckCollisonFish()
	{
		if (!isAvtive)
			return null;
		if (colliderChkTimes > 0)
			return null;

		List<ushort> arrestFishIDs = new List<ushort> ();
		if (mInputArgs.tarFishID != 0xFFFF && mInputArgs.tarFishID != 0x0) {
			arrestFishIDs.Add (mInputArgs.tarFishID);
		}
        foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
			if (fish.FishID == 0) {
				LogMgr.LogError ("fishID is 0.....");
				continue;
			}
			if (fish.Catched || fish.IsDelay)
				continue;
            if (fish.vo.Multiple > this.mMaxMul) {//超过指定倍率
                continue;
            }
			if (fish.IsInView_Center () == false)
				continue;

			if (arrestFishIDs.Contains (fish.FishID))
				continue;

			if (CheckIsInShapeArea (fish, ColliderShape.Circle, mScreenCenterPos, range)) {
				arrestFishIDs.Add (fish.FishID);
			}
		}
		colliderChkTimes++;
		return arrestFishIDs;

	}
}
