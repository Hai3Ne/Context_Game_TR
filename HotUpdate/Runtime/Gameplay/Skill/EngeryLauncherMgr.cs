using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EngeryLauncherMgr
{
	public void Init()
	{
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.BulletBufferAdd, OnBulletBuffAddHandle);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.BulletBufferRemoved, OnBulletBuffRemoveHandle);
	}

	bool IsHasEngery(byte clientSeat) {//对应位置用户是否拥有能量炮BUFF
		IEffectProgroessItem rewardItem = EffectProgressMgr.FindEngeryLcr (clientSeat);
		return rewardItem != null;
    }

	void OnBulletBuffAddHandle(object args)
	{
		BulletBufferData buffdata = args as BulletBufferData;
		if (buffdata == null || Array.Exists(buffdata.effVo, x=>x.Type == (byte)SkillCatchOnEffType.AltaMulti) == false)
			return;

        ResManager.LoadAsyn<GameObject>(ResPath.NewUIPath + "UI_NLP", (ab_data, obj) => {
            GameObject eLcrGo = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
            eLcrGo.AddComponent<ResCount>().ab_info = ab_data;
            RateEngeryAwardItem item = new RateEngeryAwardItem(eLcrGo);
            item.SetBuff(buffdata);
            EffectProgressMgr.AddEfBufItem(buffdata.clientSeat, item);
            RefreshMyRateLCRStatus(buffdata.clientSeat);
            EffManager.Vibrate();
        }, GameEnum.Fish_3D);
	}

	Vector3 CalEnLcrEffPos(byte clientSeat)
	{
		Vector3 uiWpos = Vector3.zero;
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp == null)
			return uiWpos;
		uiWpos = sp.Launcher.LauncherPos;
		uiWpos += sp.Launcher.localToWorldMatx.MultiplyVector (new Vector3(300f, 300f, 0f));
		return uiWpos;
	}

	void OnBulletBuffRemoveHandle(object args)
	{
		BulletBufferData buffdata = args as BulletBufferData;
		if (buffdata == null || Array.Exists(buffdata.effVo, x=>x.Type == (byte)SkillCatchOnEffType.AltaMulti) == false)
			return;
        EffectProgressMgr.Remove(buffdata.uniqueID);
		RefreshMyRateLCRStatus (buffdata.clientSeat);
	}

	void RefreshMyRateLCRStatus(byte clientSeat){
		var sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp == null) {
			LogMgr.LogWarning ("clientseat="+clientSeat+" not exists.");	
			return;
		}

		if (this.IsHasEngery (clientSeat)) {
			sp.Launcher.SetRateLCRActive (true);
		} else {
			sp.Launcher.SetRateLCRActive (false);
		}
	}

	public bool Update(float dTime)
	{
		return false;
	}

	public void ShutDown()
	{
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.BulletBufferAdd, OnBulletBuffAddHandle);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.BulletBufferRemoved, OnBulletBuffRemoveHandle);
	}
}

/// <summary>
/// 能量炮奖励提示
/// </summary>
public class RateEngeryAwardItem: IEffectProgroessItem
{
	GameObject gameObj;
	EneryLCRRef uiRef;

	BulletBufferData mCachedBuff;
	int bulletTotalNum = 1;
	float duration;
	bool mIsDestroy = false;
    public ScenePlayer mCurPlayer;

    public RateEngeryAwardItem(GameObject goObj)
	{
		IsDestroy = false;
		gameObj = goObj;
		uiRef = gameObj.GetComponent<EneryLCRRef> ();

	}

	public uint buffUniqueID { get { return mCachedBuff.uniqueID;}}

	public BulletBufferData GetBuff()
	{
		return mCachedBuff;
	}

	public void SetBuff(BulletBufferData buffdata)
	{
		mCachedBuff = buffdata;
        this.mCurPlayer = SceneLogic.Instance.PlayerMgr.GetPlayer(this.mCachedBuff.clientSeat);
		EffectVo altRateEffVo = Array.Find (buffdata.effVo, x => x.Type == (byte)SkillCatchOnEffType.AltaMulti);
		uiRef.mSprNum.spriteName = (altRateEffVo.Value0).ToString();
        uiRef.mSprNum.MakePixelPerfect();
		bulletTotalNum = mCachedBuff.bulletNum;
        duration = mCachedBuff.duration;

        this._init_pos();

        if (this.mCachedBuff.clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
            this.uiRef.mRenderer.gameObject.SetActive(true);
            this.SetColorAlpha(1);
        } else {
            this.uiRef.mRenderer.gameObject.SetActive(false);
        }
	}

    private bool is_init = false;
    private void _init_pos() {
        if (this.mCurPlayer.Launcher.SeatTransform == null) {            
			return;
        }
		this.is_init = true;
		this.uiRef.mTfNLP.position = this.mCurPlayer.Launcher.SeatTransform.TransformPoint (this.mCurPlayer.Launcher.CanonBaseLocalPos);
        if (this.mCachedBuff.clientSeat >= 2) {
            this.uiRef.mTfNLP.localPosition = this.uiRef.mTfNLP.localPosition + new Vector3(0, -300, 0);
        } else {
            this.uiRef.mTfNLP.localPosition = this.uiRef.mTfNLP.localPosition + new Vector3(0, 50, 0);
        }
    }

    public void SetColorAlpha(float alpha) {//设置能量跑特效颜色透明度
        if (this.uiRef.mRenderer.gameObject.activeSelf) {
            this.uiRef.mRenderer.material.SetColor("_TintColor", new Color(1f, 0.8f, 0, alpha));
        }
    }

    private bool is_show = true;
    private void CheckShow() {
        if (SceneLogic.Instance.mIsXiuYuQi == is_show) {
            is_show = !is_show;
            this.gameObj.SetActive(is_show);
        }
    }
    public bool Update(float delta) {
        this.CheckShow();
		if (IsDestroy)
			return false;
        if (this.is_init == false) {
            this._init_pos();
        }
        //float per = (float)mCachedBuff.bulletNum / (float)bulletTotalNum;
        float span = duration - (Time.realtimeSinceStartup - mCachedBuff.startTime);
        //float percent = Mathf.Max(0f, span/duration);
        //uiRef.slider.value = Mathf.Min(per, percent);

        //能量跑持续时间少于3S 或者 子弹少于10个进行颜色渐变消失
        if (span < 3 || mCachedBuff.bulletNum < 10) {
            this.SetColorAlpha(Mathf.Min(span / 3f, mCachedBuff.bulletNum / 10f));
        }
        if (span <= 0f) {
            return false;
        }
		return true; 
	}

	public void SetPosition(Vector3 pos, bool isTween = false)
	{
	}


	public bool IsDestroy
	{
		get { return mIsDestroy;}
		set { mIsDestroy = value;}
	}

	public void Destroy()
	{
		if (gameObj != null) {
			GameObject.Destroy (gameObj.gameObject);
			gameObj = null;
			uiRef = null;
		}
		mCachedBuff = null;
	}
}