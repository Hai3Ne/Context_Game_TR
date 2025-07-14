using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchOneByOneEffect : BaseSkillEffect
{
	uint mLightingEffID;
	List<CatchFishData> mFishList;
	byte clientSeat = 0;
	GameObject hitonPrefab;
	EffectLighting lightingGo;

	protected override void Start ()
	{
		base.Start ();
		clientSeat = mEffectArgs.clientSeat;
		mFishList = mEffectArgs.FishList;
		mLightingEffID = (uint)mEffVo.EffID;
		hitonPrefab = FishResManager.Instance.SkillEffectMap.TryGet (mLightingEffID);
		lightingGo = hitonPrefab.GetComponent<EffectLighting> ();
		if (lightingGo != null) {
			lightingGo = GameUtils.CreateGo (hitonPrefab).GetComponent<EffectLighting> ();
            lightingGo.SetWaitTime(mEffectArgs.WaitTime);
			Vector3 v3 = mEffectArgs.screenPos;
			if (v3 == ConstValue.UNVALIDE_POSITON)
				v3 = new Vector3 (Resolution.ScreenWidth * 0.5f, Resolution.ScreenHeight * 0.5f, 0f);

			Vector3[] toPosList = new Vector3[mFishList.Count];
			for (int i = 0; i < mFishList.Count; i++) {
				toPosList [i] = mFishList [i].FishObj.ScreenPos;
			}
			lightingGo.fromPos = v3;
			lightingGo.targetList = toPosList;
		}
		isInited = true;
	}

	bool isInited = false;
	public override bool Update (float delta)
	{
		if (delta > 100) {
			if (lightingGo != null) {
				Destroy ();
			}
			return false;
		}
		if (!isInited)
			return true;
		
		if (lightingGo != null && lightingGo.isOver) {
			Destroy ();
			return false;
		}
		return true;
	}

    public override void Destroy() {
        if (isDestroy == false) {
            isDestroy = true;
            if (lightingGo != null) {
                GameObject.Destroy(lightingGo.gameObject);
                lightingGo = null;
            }
            base.Destroy();
        }
    }
}

