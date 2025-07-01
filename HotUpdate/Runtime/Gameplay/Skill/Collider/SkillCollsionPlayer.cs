using System;
using UnityEngine;
using System.Collections.Generic;

public class SkillCollsionPlayer
{
	List<SkilCollider> effectList = new List<SkilCollider> ();
	public SkillVo mSkillVo;
	float timeToggle = 0f, lifeTime = -1f;
	byte mCSeat;
	uint mSkillCfgId;
	ColliderTestInputData mInputArgs;
	public void Init (byte clientSeat, SkillVo skVo, ColliderTestInputData inputArgs)
	{
		mCSeat = clientSeat;
		mSkillCfgId = skVo.CfgID;
		mInputArgs = inputArgs;
		mSkillVo = skVo;
		timeToggle = 0f;
		effectList.Clear ();
	}

	public byte clientSeat
	{
		get { return mCSeat;}
	}

	public uint skillCfgId
	{
		get { return mSkillCfgId;}
	}

	public bool IsSustain
	{
		get 
		{
			return effectList.Count > 0 && effectList [0].isSustainEff;
		}
	}

	public void Start()
	{
		lifeTime = -1f;
		PlayCastEffect ();
	}

	void PlayCastEffect()
	{
		lifeTime = mSkillVo.CastDuation * 0.001f;
		lifeTime = lifeTime <= 0f ? 0.1f : lifeTime;
		BulletSkill pTracerBullet = null;
		if (mSkillVo.CastEffID > 0) {
			GameObject effPrefab = FishResManager.Instance.SkillEffectMap.TryGet (mSkillVo.CastEffID);
			GameObject effectGo = GameUtils.CreateGo (effPrefab);
			effectGo.transform.position = effPrefab.transform.position;
			effectGo.transform.rotation = effPrefab.transform.rotation;
			if (mSkillVo.NeedTarget) {
				var sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
				mInputArgs.inputScreenPos.z = effPrefab.transform.position.z;
				Vector3 fromWroldPos = Utility.ConvertWorldPos (sp.Launcher.CanonBaseWorldPos, SceneObjMgr.Instance.UICamera, Utility.MainCam, effPrefab.transform.position.z);
				Vector3 fromDir = sp.Launcher.GunDirection;
				CheckRobotSkill (ref mInputArgs.tarFishID, ref mInputArgs.inputScreenPos);
				pTracerBullet = SceneLogic.Instance.SkillMgr.AddBulletSkill (effectGo, clientSeat, mSkillVo, fromWroldPos, fromDir, mInputArgs.inputScreenPos, mInputArgs.tarFishID, mInputArgs.tarFishPart);
                //鱼雷攻击目标动画
                if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                    ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Other/Eff_YuLeiSelect", (ab_data, prefab) => {
                        GameObject obj = GameObject.Instantiate(prefab, SceneLogic.Instance.LogicUI.BattleUI) as GameObject;
                        obj.AddComponent<ResCount>().ab_info = ab_data;
                        EffFollowPos mEffYuLeiSelect = obj.AddComponent<EffFollowPos>();

                        mEffYuLeiSelect.gameObject.SetActive(true);
                        Transform fish_part = null;
                        Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(mInputArgs.tarFishID);
                        if (fish != null && fish.IsInView_Center()) {
                            fish_part = fish.GetBodyPartTrans(mInputArgs.tarFishPart);
                        }
                        if (fish_part == null) {
                            mEffYuLeiSelect.transform.position = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(new Vector3(mInputArgs.inputScreenPos.x, mInputArgs.inputScreenPos.y));
                            mEffYuLeiSelect.enabled = false;
                        } else {
                            mEffYuLeiSelect.InitData(fish_part);
                        }
                        mEffYuLeiSelect.transform.localScale = new Vector3(8, 8, 8);
                        TweenScale.Begin(mEffYuLeiSelect.gameObject, 0.35f, Vector3.one).SetOnFinished(() => {
                            if (mEffYuLeiSelect != null) {
                                GameObject.Destroy(mEffYuLeiSelect.gameObject);
                                mEffYuLeiSelect = null;
                            }
                        });
                    }, GameEnum.Fish_3D);
                }
			} else {
                if (mSkillVo.CastEffID == 210001u) {//1号炮台技能进行特殊处理
                    GameObject.Destroy(effectGo, lifeTime);
				    var sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
                    effectGo.transform.SetParent(sp.Launcher.GunBarrel.BaseTrans);
                    effectGo.transform.localPosition = Vector3.zero;
                    effectGo.transform.localRotation = Quaternion.identity;
                    effectGo.transform.localScale = Vector3.one;
                } else {
                    PSLife.Begin(effectGo, null);
                    GameUtils.PlayPS(effectGo);
                }
			}
		}
		SkilCollider skCollider = InitSkColliders ();
		if (pTracerBullet != null && skCollider != null) {
			pTracerBullet.bindCollder = skCollider;
			skCollider.isAvtive = false;
			skCollider.delaytime = float.MaxValue;
		}
	}

	bool CheckRobotSkill(ref ushort tarFishID, ref Vector3 screen){

		byte selfCSeat = SceneLogic.Instance.PlayerMgr.MyClientSeat;
		if (mInputArgs.Handle == SceneLogic.Instance.FModel.SelfServerSeat && selfCSeat != mCSeat) {
			Fish f = SceneLogic.Instance.FishMgr.GetAtkTargetFish (mCSeat);
			if (f != null)
				tarFishID = f.FishID;
			else
				tarFishID = 0xFFFF;
			screen = Utility.MainCam.WorldToScreenPoint(new Vector3(0f, 0f, ConstValue.NEAR_Z+1f));
		}
		return false;
	}

	SkilCollider InitSkColliders()
	{
		byte selfCSeat = SceneLogic.Instance.PlayerMgr.MyClientSeat;
		if (mCSeat != selfCSeat && mInputArgs.Handle != SceneLogic.Instance.FModel.SelfServerSeat)
			return null;
		
		SkilCollider skEff = null;
		if (mSkillVo.EffID0 > 0) {
			EffectVo colliderVo = FishConfig.Instance.EffectConf.TryGet (mSkillVo.EffID0);
			skEff = SkillFactory.Factory (colliderVo, mInputArgs);
			skEff.delaytime = skEff.isSustainEff ? 0 : lifeTime;
			skEff.isAvtive = skEff.delaytime <= 0f;
			if (skEff.isCollider)
				effectList.Add (skEff);
		}
		return skEff;
	}

	public List<SkilCollider> SkillEffects()
	{
		return effectList;
	}

	public bool Update(float delta)
	{
		if (lifeTime < 0f)
			return true;
		
		if (effectList.Count <= 0)
			return false;		
		for (int i = 0; i < effectList.Count; i++) {
			if (effectList [i].isAvtive) continue;
			if (effectList [i].delaytime > 0f) {
				effectList [i].delaytime -= delta;
			}
			if (effectList [i].delaytime <= 0f)
				effectList [i].isAvtive = true;
		}
		timeToggle += delta;
		if (effectList.Count > 0 && effectList[0].isSustainEff && timeToggle >= lifeTime) {
			return false;
		}
		return true;
	}

	public void Destroy()
	{
		foreach (var eff in effectList) {
			eff.Destory ();
		}
		effectList.Clear ();
	}
}