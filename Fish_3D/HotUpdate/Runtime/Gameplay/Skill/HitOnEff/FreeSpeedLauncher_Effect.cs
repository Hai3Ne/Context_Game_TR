using System;
using UnityEngine;

public class FreeSpeedLauncher_Effect : BaseSkillEffect
{
	BulletBufferData mCachedBuff;
	float duration;
	GameObject mLcrGo, screenHoleGo;
	Renderer mSrcHoleRender;
	public Transform mlcrTrans;
    public GameObject mEffFree;//免费炮炮台特效

	protected override void Start ()
	{
        mLcrGo = GameUtils.CreateGo(FishResManager.Instance.mFreeLcrObj, SceneLogic.Instance.LogicUI.BattleUI);
        mEffFree = GameUtils.CreateGo(FishResManager.Instance.mEffFree, SceneLogic.Instance.LogicUI.BattleUI);
		mlcrTrans = mLcrGo.transform.GetChild (0);
		screenHoleGo = mLcrGo.transform.GetChild (1).gameObject;
		mSrcHoleRender = screenHoleGo.GetComponentInChildren<Renderer> ();
		this._init_pos();
		if (mEffectArgs.clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
			screenHoleGo.SetActive (true);
			ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (mEffectArgs.clientSeat);
			sp.Launcher.SetRateLCRActive(true);//拥有分叉跑时禁止更改炮台倍率以及等级
		} else {
			screenHoleGo.SetActive(false);
		}
		mCachedBuff = SceneLogic.Instance.BulletMgr.FindBBufferByID (mEffectArgs.bufferID);
        duration = mCachedBuff.duration;
        EffManager.Vibrate();
	}

	private bool is_init = false;
	private void _init_pos() {
		var curPlayer = SceneLogic.Instance.PlayerMgr.GetPlayer(mEffectArgs.clientSeat);
		if (curPlayer.Launcher.SeatTransform == null) {
			return;
		}
		this.is_init = true;
		mlcrTrans.position = curPlayer.Launcher.SeatTransform.TransformPoint (curPlayer.Launcher.CanonBaseLocalPos);
        this.mEffFree.transform.position = curPlayer.Launcher.SeatTransform.position;
		if (mEffectArgs.clientSeat >= 2) {
			mlcrTrans.localPosition = mlcrTrans.localPosition + new Vector3(0, -300, 0);
		} else {
			mlcrTrans.localPosition = mlcrTrans.localPosition + new Vector3(0, 50, 0);
		}
	}

	void SetColorAlpha(float alpha) {//设置能量跑特效颜色透明度
		if (this.screenHoleGo.activeSelf) {
			this.mSrcHoleRender.material.SetColor("_TintColor", new Color(1f, 0.8f, 0, alpha));
		}
	}

    private bool is_show = true;
    private void CheckShow() {
        if (SceneLogic.Instance.mIsXiuYuQi == is_show) {
            is_show = !is_show;
            this.mLcrGo.SetActive(is_show);
            this.mEffFree.SetActive(is_show);
        }
    }
    public override bool Update(float delta) {
        this.CheckShow();

		if (isDestroy)
			return false;
		if (this.is_init == false) {
			this._init_pos();
		}
		float span = duration - (Time.realtimeSinceStartup - mCachedBuff.startTime);
		//分叉炮持续时间少于3S 或者 子弹少于10个进行颜色渐变消失
		if (span < 3 || mCachedBuff.bulletNum < 10) {
			this.SetColorAlpha(Mathf.Min(span / 3f, mCachedBuff.bulletNum / 10f));
		}

		if (delta > 100f) {
			isDestroy = true;
			Destroy ();
			return false;
		}
		return true;
	}

	public override void Destroy ()
	{	
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (mEffectArgs.clientSeat);
		if (sp != null)
		{
			if (mEffectArgs.clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
				sp.Launcher.SetRateLCRActive(false);//拥有分叉跑时禁止更改炮台倍率以及等级
			}
        }
        if (mLcrGo != null) {
            GameObject.Destroy(mLcrGo.gameObject);
            mLcrGo = null;
        }
        if (this.mEffFree != null) {
            GameObject.Destroy(this.mEffFree.gameObject);
            this.mEffFree = null;
        }
		base.Destroy ();
	}
}