using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ColliderShape{
	Circle = 0,
	Linear = 1,
}

/// <summary>
/// 星辰陨落
/// </summary>
public class SustainRangeAttackCollider : SkilCollider
{
	uint duration = 1; // 持续多少毫秒
	float radius = 1f;
	uint overMilisec = 1;
	Vector3 mScreenCenterPos;
	Vector4 _Bounds = ConstValue.UNVALIDE_SCREEN_RECT;
	ColliderShape mShape = ColliderShape.Circle;
	uint mTimesCount = 1;
	uint nowTimesCount = 0;
	bool mIsSustainEff = true;
	public override void Init (EffectVo vo, ColliderTestInputData inputargs)
	{
		base.Init (vo,inputargs);
		mIsSustainEff = true;
		mTimesCount = (uint)vo.Value0;
		duration	= (uint)vo.Value1;
		mShape = (ColliderShape)(int)vo.Value2;
		radius = (float)vo.Value3;
		overMilisec =UTool.GetTickCount ();
		mScreenCenterPos = mInputArgs.inputScreenPos;
		if (mTimesCount == 1) {
			mIsSustainEff = false;
		}
		nowTimesCount = 0;
		InitShapeColliderTest (mShape, mScreenCenterPos, radius);
	}

	public override List<ushort> ChckCollisonFish()
	{
		if (!isAvtive)
			return null;
		if (nowTimesCount >= mTimesCount)
			return null;
		uint tc =UTool.GetTickCount ();
		if (tc >= overMilisec)
		{
			nowTimesCount++;
			overMilisec = tc + duration;
			List<ushort> arrestFishIDs = new List<ushort> ();
            foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
				if (fish.Catched || fish.IsDelay)
					continue;
				if (fish.IsInView_Center() == false)
					continue;
				
				if (arrestFishIDs.Contains (fish.FishID))
					continue;
				if (CheckIsInShapeArea(fish, mShape, mScreenCenterPos, radius)) {
					arrestFishIDs.Add (fish.FishID);
				}
			}
			return arrestFishIDs;
		} else {
			return null;
		}
	}

	public override bool isSustainEff {
		get {
			return mIsSustainEff;
		}
	}
}