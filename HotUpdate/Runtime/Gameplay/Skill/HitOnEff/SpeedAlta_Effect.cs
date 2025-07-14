using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct SpeedAltaData
{
	public Fish fish;
	public GameObjRef dizzyTrans;
	public IFishOpt fishOpt;
}


public class GameObjRef{
	public int prefabID;
	public GameObject mTrans;
	public uint RefCount;
}
public class SpeedAlta_Effect : BaseSkillEffect
{
	uint mCastEffID = 0;
	float duration = 0f;
	float speedrate = 0.15f;
	protected List<Fish> mTargetFishList = new List<Fish> ();
	protected GameObject effGoPrefab = null;

	List<SpeedAltaData> altSpeedFishList = new List<SpeedAltaData>();
	protected override void Start ()
	{
		base.Start ();
		mCastEffID = (uint)mEffVo.EffID;
		duration 	= mEffVo.Value0 * 0.001f;
		speedrate 	= mEffVo.Value1 * 0.001f;

		FilterTargetFishes (mEffectArgs.FishList);
		if (mCastEffID > 0) {
			effGoPrefab = FishResManager.Instance.SkillEffectMap.TryGet(mCastEffID);
			AltFish ();
		} else {
			AltFish ();
		}
	}

	protected virtual void FilterTargetFishes(List<CatchFishData> catchDatas)
	{
		mTargetFishList.Clear ();
		foreach(var cah in catchDatas)
		{
			if (cah.FishObj != null)
				mTargetFishList.Add (cah.FishObj);
		}
	}

	public override bool Update (float delta)
	{
		if (delta >= 100f) {
			isDestroy = true;
			Destroy ();
			return false;
		}

		for (int i = 0; i < altSpeedFishList.Count;) {
			SpeedAltaData altData = altSpeedFishList [i];
			if (altData.fish.Transform != null && altData.fishOpt.isDestroy) {
				if (altData.dizzyTrans != null) {				
					altData.dizzyTrans.RefCount--;
					if (altData.dizzyTrans.RefCount <= 0) {
						GameObject.Destroy (altData.dizzyTrans.mTrans);
						altData.fish.mCacheEffObjRefMap.Remove (effGoPrefab.GetInstanceID ());
					}
					Utility.ListRemoveAt (altSpeedFishList, i);
					continue;
				}
			}
			++i;
		}

		if (altSpeedFishList.Count <= 0) {
			isDestroy = true;
			Destroy ();
			return false;
		}

		if (duration > 0) {
			duration -= delta;
			if (duration <= 0) 
			{
				Destroy ();
				return false;
			}
		}
		return duration > 0;
	}

	public override void Destroy ()
	{	
		while (altSpeedFishList.Count > 0) {
			SpeedAltaData altData = altSpeedFishList [0];
			if (altData.fish.Transform != null)
				altData.fish.RemoveOpt (altSpeedFishList [0].fishOpt);

			if (altData.dizzyTrans != null) {				
				altData.dizzyTrans.RefCount--;
				if (altData.dizzyTrans.RefCount <= 0) {
					GameObject.Destroy (altData.dizzyTrans.mTrans);
					altData.fish.mCacheEffObjRefMap.Remove (effGoPrefab.GetInstanceID ());
				}
			}
			altSpeedFishList.RemoveAt (0);
		}
		isDestroy = true;
		base.Destroy ();
	}

	protected virtual void AltFish()
	{
		for (int i = 0; i < mTargetFishList.Count; i++) 
		{
			Fish fish = mTargetFishList[i];
			Vector3[] corns = fish.GetWorldCorners ();
			if (corns == null || fish.Catched)
				continue;
			SpeedAltaData altData = new SpeedAltaData ();
			if (mCastEffID > 0) 
			{
				altData.dizzyTrans = AddFishEffect (fish, mCastEffID);
			}

			fish.ClearOpt ();
			var rd = new ReductionData (speedrate, 0, duration, 0f);
			FishOptReduction ff = new FishOptReduction (speedrate, rd);
			fish.AddOpt (ff);

			altData.fish = fish;
			altData.fishOpt = ff;

			altSpeedFishList.Add (altData);
		}
	}

	protected virtual GameObjRef AddFishEffect(Fish fish, uint effectid)
	{
		Vector3[] corns = fish.GetWorldCorners ();
		if (corns == null)
			return null;
		
		GameObjRef gameObjRef;
		int prefabid = effGoPrefab.GetInstanceID ();
		if (fish.mCacheEffObjRefMap.ContainsKey (prefabid)) {
			gameObjRef = fish.mCacheEffObjRefMap [prefabid];
			gameObjRef.RefCount++;
		} else {
			gameObjRef = new GameObjRef ();
			fish.mCacheEffObjRefMap [prefabid] = gameObjRef;
			gameObjRef.mTrans = GameUtils.CreateGo (effGoPrefab);
			gameObjRef.RefCount++;
			gameObjRef.mTrans.transform.SetParent (fish.Transform);
		}
		return gameObjRef;
	}
}