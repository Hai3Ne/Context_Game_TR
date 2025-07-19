using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKGoldEff {
    public Vector3 mCenterPos;//圆心坐标
    public Vector3 mInitPos;
    public Vector3 mTargetPos;
    public float mTotalTime;
    public ushort mClientSeat;

    private LKAnimSprite mSprAnim;
    private float mScale;
    private int stage = -1;
    private float _fly_time = 0;//飞行时间
    private Vector3[] curePath = new Vector3[3];//飞行路径
    private Vector3 _init_scale;
    private Vector3 _target_scale;

    public Transform mTf;
    public SpriteRenderer mSR;
    private float mAnimSpeed = 1;//金币动画速度

    private float _time;

    public LKGoldEff(LKAnimSprite anim) {
        this.mSprAnim = anim;
        this.mTf = this.mSprAnim.transform;
        this.mSR = this.mSprAnim.GetComponent<SpriteRenderer>();
    }
    public void InitData(ushort seat, float scale, Vector3 pos) {
        this._time = 0;
        this.mClientSeat = seat;
        this.mScale = scale;
        this.mCenterPos = pos;
        this.mTf.localScale = Vector3.zero;
        this.mTf.position = this.mCenterPos;

        this.mSprAnim.ResetToBeginning();
        this.SetStage(-1);
    }

    public bool Update(float delta) {
        this._time += delta;
        if (stage == -1) {
            if (this._time > 0.1f) {
                this.mTf.localScale = Vector3.one * this.mScale;
                this.mTf.position = this.mInitPos;
                this._time -= 0.1f;
                this.SetStage(0);
            } else {
                this.mTf.localScale = Vector3.one * Mathf.Lerp(0, this.mScale, this._time / 0.1f);
                this.mTf.position = Vector3.Lerp(this.mCenterPos, this.mInitPos, this._time / 0.1f);
            }
        } else if (stage == 0) {//等待1S
            if (this._time > 1) {
                this._time -= 1;
                float len = Vector2.Distance(this.mInitPos, this.mTargetPos) / SceneObjMgr.Instance.UIRoot.transform.localScale.x;
                this.mTotalTime = Mathf.Max(0.3f, len / 300);//飞行总时间
                this.SetStage(1);
            }
        } else if (stage == 1) {//飞向目标点
            if (this._time >= this.mTotalTime) {
                this.mTf.position = this.mTargetPos;
                this.SetStage(2);
            } else {
                this.mTf.position = Vector3.Lerp(this.mInitPos, this.mTargetPos, this._time / this.mTotalTime);
            }
        }

        return stage != 2;
    }
    public void SetStage(int stage) {
        this.stage = stage;
        //if (this.stage == 2) {//完成
        //    //if (this.mClientSeat == RoleManager.Self.ChairSeat) {
        //    //    AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.GoldGet);
        //    //}
        //    LKEffManager.Reback(LKEffManager.Anim_Gold, this.gameObject);
        //    //LKGoldEffManager.GoldEffFinish(this);
        //}
    }

    public void SetGray(bool is_gray) {
        //this.mSR.IsGray = is_gray;
        if (is_gray) {
            this.mSR.color = Color.black;
        } else {
            this.mSR.color = Color.white;
        }
    }

}
