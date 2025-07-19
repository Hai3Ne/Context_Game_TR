using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillEffectData
{
	public byte clientSeat;
	public uint bufferID;

	public Vector3 screenPos  = ConstValue.UNVALIDE_POSITON;
	public Vector3 worldPos  = ConstValue.UNVALIDE_POSITON;
	public List<CatchFishData> FishList;
    public float WaitTime;//等待时间
}

public class BaseSkillEffect
{
	protected bool isDestroy = false;
	protected EffectVo mEffVo;
	protected SkillEffectData mEffectArgs;
	public SkillCatchOnEffType EffType;
	public uint EffectCfgID
	{
		get { return mEffVo.CfgID;}
	}

	public SkillEffectData ExtData
	{
		get { return mEffectArgs;}
	}
	public virtual void Init(EffectVo effVo, SkillEffectData effData)
	{
		isDestroy = false;
		mEffVo = effVo;
		mEffectArgs = effData;
		EffType = (SkillCatchOnEffType)mEffVo.Type;
		Start ();
	}

	protected virtual void Start() {
	}

	public virtual bool Update(float delta)	{
		return true;
	}

	public bool IsDestroy
	{
		get { return isDestroy;}
	}
	protected Fish[] GetFishesInScreen() {
		return null;
	}

	public virtual void Destroy () {
		isDestroy = true;
		mEffVo = null;
		mEffectArgs = null;
	}

}
