using System;
using UnityEngine;
using System.Collections.Generic;

public class AltLauncherSpeed_Effect : BaseSkillEffect
{
	float speedRate = 1f;
	float duration = 0f;
	GameObject mEffectGo;
    private LCRReduce reduce;
	protected override void Start ()
	{
		isDestroy = true;
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (mEffectArgs.clientSeat);
		if (sp != null) {
			isDestroy = false;
			duration = mEffVo.Value0 * 0.001f;
			speedRate = mEffVo.Value1 * 0.001f;
            this.reduce = new LCRReduce(speedRate, duration);
            sp.Launcher.lcrReduceData.Add(this.reduce);
			if (mEffVo.EffID > 0) {
				setupEffObject (GameUtils.CreateGo (FishResManager.Instance.SkillEffectMap.TryGet (mEffVo.EffID), sp.Launcher.SeatTransform), sp);
			}
		}
	}

	void setupEffObject(GameObject obj, ScenePlayer sp)
	{
		mEffectGo = obj;
		mEffectGo.transform.localPosition = sp.Launcher.CanonBaseLocalPos;
		mEffectGo.transform.localScale = Vector3.one;
	}

	public override bool Update (float delta)
	{
		if (isDestroy)
			return false;
		
		if (delta > 100f) {
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
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (mEffectArgs.clientSeat);
		if (sp != null)
		{
            if (this.reduce != null) {
                sp.Launcher.lcrReduceData.Remove(this.reduce);
                this.reduce = null;
            }
			if (mEffectGo != null)
				GameObject.Destroy (mEffectGo);
		}
		base.Destroy ();
	}


}
