using System;
using UnityEngine;

public class BranchesLauncher_Effect : BaseSkillEffect
{
	BranchLCRRef mBranchUIRef;
	BulletBufferData mCachedBuff;
	float duration;
	protected override void Start ()
	{
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (mEffectArgs.clientSeat);
        GameObject gameObj = GameUtils.CreateGo(FishResManager.Instance.mBranchLcrObj, SceneLogic.Instance.LogicUI.BattleUI);
		mBranchUIRef = gameObj.GetComponent<BranchLCRRef> ();

		int branchNum = mEffVo.Value0;
		int idx = Array.IndexOf(mBranchUIRef.numberIds, branchNum);
		mBranchUIRef.numLabel.spriteName = mBranchUIRef.numbersprites[idx];
		this._init_pos();

        sp.Launcher.ShowBranchEff(branchNum-1);
		if (mEffectArgs.clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
			this.mBranchUIRef.mRenderer.gameObject.SetActive(true);
            this.SetColorAlpha(1);
            sp.Launcher.SetRateLCRActive(true);//拥有分叉跑时禁止更改炮台倍率以及等级
		} else {
			this.mBranchUIRef.mRenderer.gameObject.SetActive(false);
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
		this.mBranchUIRef.mBranchTrans.position = curPlayer.Launcher.SeatTransform.TransformPoint (curPlayer.Launcher.CanonBaseLocalPos);
		if (mEffectArgs.clientSeat >= 2) {
			mBranchUIRef.mBranchTrans.localPosition = this.mBranchUIRef.mBranchTrans.localPosition + new Vector3(0, -300, 0);
		} else {
			mBranchUIRef.mBranchTrans.localPosition = this.mBranchUIRef.mBranchTrans.localPosition + new Vector3(0, 50, 0);
		}
	}

	void SetColorAlpha(float alpha) {//设置能量跑特效颜色透明度
		if (this.mBranchUIRef.mRenderer.gameObject.activeSelf) {
			this.mBranchUIRef.mRenderer.material.SetColor("_TintColor", new Color(1f, 0.8f, 0, alpha));
		}
	}

    private bool is_show = true;
    private void CheckShow() {
        if (SceneLogic.Instance.mIsXiuYuQi == is_show) {
            is_show = !is_show;
            this.mBranchUIRef.gameObject.SetActive(is_show);
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
			sp.Launcher.ResetGunAngleRange ();

            sp.Launcher.ShowBranchEff(0);
            if (mEffectArgs.clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                sp.Launcher.SetRateLCRActive(false);//拥有分叉跑时禁止更改炮台倍率以及等级
            }
		}
		if (mBranchUIRef != null) {
			GameObject.Destroy (mBranchUIRef.gameObject);
		}
		mBranchUIRef = null;
		base.Destroy ();
	}

	public static bool CheckHasBranchLCR(BulletBufferData bulletBuff,float angle, out short[] branchange){
		if (bulletBuff != null && bulletBuff.bulletNum > 0) {
			EffectVo branchVo = Array.Find (bulletBuff.effVo, x => x.Type == (byte)SkillCatchOnEffType.BranchLCR);
			if (branchVo == null) {
				branchange = new short[0];
				return false;
			}
            //int branchNum = branchVo.Value0;
            float[] angles = SceneLogic.Instance.BulletMgr.GetBreanchAngles(branchVo.Value0, angle);
            branchange = new short[angles.Length];
            for (int i = 0; i < angles.Length; i++) {
                branchange[i] = Utility.FloatToShort(angles[i]);
            }
			return true;

		} else {
			branchange = new short[0];
		}
		return false;
	}

}

