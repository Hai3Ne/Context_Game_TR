using System;
using UnityEngine;

public class ScreenOverlap_Effect : BaseSkillEffect
{
	uint mEffectID;
	GameObject effInst = null;
	float duration = 0f;
	protected override void Start ()
	{
		base.Start ();
		mEffectID = (uint)mEffVo.EffID;
		GameObject prefabgo = FishResManager.Instance.SkillEffectMap.TryGet(mEffectID);
		effInst = GameUtils.CreateGo(prefabgo);
		effInst.transform.position = prefabgo.transform.position;
		effInst.transform.localScale =prefabgo.transform.localScale;
		effInst.transform.localRotation = prefabgo.transform.localRotation;
		duration 	= mEffVo.Value0 * 0.001f;
	}

	public override bool Update (float delta)
	{
		if (delta > 100f) {
			isDestroy = true;
			if (effInst != null) {
				GameObject.Destroy (effInst);
				effInst = null;
			}
			return false;
		}
		if (effInst == null)
			return base.Update (delta);
		if (duration > 0) 
		{
			duration -= delta;
			if (duration <= 0) {
				GameObject.Destroy (effInst);
				effInst = null;
				return false;
			}
		}
		return true;
	}
}
