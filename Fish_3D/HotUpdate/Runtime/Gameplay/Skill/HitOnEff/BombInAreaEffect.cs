using System;
using UnityEngine;

public class BombInAreaEffect : BaseSkillEffect
{
	Vector3 bombScrPos;	
	uint mBombEffID;
	GameObject bombEffGoInst;
	bool isInited = false;
	protected override void Start ()
	{
		base.Start ();
		isInited = true;
		mBombEffID = (uint)mEffVo.EffID;
		var hitonPrefab = FishResManager.Instance.SkillEffectMap.TryGet (mBombEffID);
		bombEffGoInst = GameUtils.CreateGo (hitonPrefab);
		PSLife.Begin (bombEffGoInst, delegate {
			bombEffGoInst = null;
		});
		mEffectArgs.screenPos.z = ConstValue.NEAR_Z+0.1f;
		bombEffGoInst.transform.position = Utility.MainCam.ScreenToWorldPoint (mEffectArgs.screenPos);
	}

	public override bool Update (float delta)
	{
		if (delta > 100) {
			isDestroy = true;
			Destroy ();
			return false;
		}
		if (!isInited)
			return true;

		if (bombEffGoInst == null) {
			Destroy ();
			return false;
		}
		return true;
	}

	public override void Destroy() {
		if (isDestroy == false) {
			isDestroy = true;
			if (bombEffGoInst != null) {
				GameObject.Destroy(bombEffGoInst.gameObject);
				bombEffGoInst = null;
			}
			base.Destroy();
		}
	}
}


