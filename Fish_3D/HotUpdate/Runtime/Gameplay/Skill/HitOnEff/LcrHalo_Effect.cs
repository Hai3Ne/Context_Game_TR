using System;
using System.Collections.Generic;
using UnityEngine;


public class LcrHalo_Effect : BaseSkillEffect
{
	byte clientSeat = 0xFF;
	GameObject mEffectGo;
	float duration = 0f;
	protected override void Start ()
	{
		clientSeat = mEffectArgs.clientSeat;
		duration = mEffVo.Value0 * 0.001f;
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp != null) {
			GameObject handleObj = GameUtils.CreateGo (FishResManager.Instance.SkillEffectMap.TryGet (mEffVo.EffID), sp.Launcher.SeatTransform);
			setupEffObject (sp.ClientSeat, handleObj, sp);
		}
	}

	void setupEffObject(byte clientSeat, GameObject obj, ScenePlayer sp)
	{
		if (isDestroy)
			return;
		mEffectGo = obj;
		mEffectGo.transform.localPosition = sp.Launcher.LauncherUIPos - sp.Launcher.SeatTransform.localPosition;
		mEffectGo.transform.localScale = Vector3.one;
		mEffectGo.transform.localRotation = clientSeat > 1 ? Quaternion.Euler(Vector3.forward*180f) : Quaternion.identity;
		EffectArgs eArgs = mEffectGo.GetComponent<EffectArgs> ();
		if (eArgs == null)
			eArgs = mEffectGo.AddComponent<EffectArgs> ();
		eArgs.duration = duration;
	}

	public override bool Update (float delta)
	{
		if (delta > 100f) {
			isDestroy = true;
			if (mEffectGo != null) {
				Destroy ();
			}
			return false;
		}
		if (duration > 0f) {
			duration -= delta;
			if (duration <= 0f) {
				Destroy ();
				return false;
			}
		} else {
			Destroy ();
			return false;
		}
		return true;
	}

	public override void Destroy ()
	{
		if (mEffectGo != null) {
			GameObject.Destroy (mEffectGo);
			mEffectGo = null;
		}
		base.Destroy ();
	}
}

