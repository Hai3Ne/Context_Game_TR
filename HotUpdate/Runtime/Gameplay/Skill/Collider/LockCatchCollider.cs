using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockCatchCollider: SkilCollider
{
	Vector3 mScreenCenterPos;
	int maxCatchNum;
	ColliderShape mShapeType;
	float range;
	int colliderChkTimes;
	public override void Init (EffectVo vo, ColliderTestInputData inputargs)
	{
		base.Init (vo,inputargs);
		maxCatchNum = (int)vo.Value0;
		mShapeType = (ColliderShape)(int)vo.Value1;
		range = (float)vo.Value2;
		mScreenCenterPos = mInputArgs.inputScreenPos;
		InitShapeColliderTest (mShapeType, mScreenCenterPos, range);
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
			if (fish.IsInView_Center () == false)
				continue;

			if (arrestFishIDs.Contains (fish.FishID))
				continue;

			if (CheckIsInShapeArea (fish,mShapeType, mScreenCenterPos, range)) {
				arrestFishIDs.Add (fish.FishID);
			}
			if (arrestFishIDs.Count == maxCatchNum) {
				break;
			}
		}
		colliderChkTimes++;
		return arrestFishIDs;

	}
}

