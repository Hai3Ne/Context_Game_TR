using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimRotate : MonoBehaviour {
    public List<float> mList = new List<float>();
    public float mSpeed;//旋转速度
    public VoidCall<int, bool> mStepCall;//不同进度回调
    public VoidCall<float> mRotateCall;//旋转回调
    public static AnimRotate Begin(GameObject obj, float spd, VoidCall<int, bool> call, List<float> rotates, VoidCall<float> rotate_call) {
        if (rotates.Count == 0) {
            return null;
        }
        AnimRotate anim = obj.GetComponent<AnimRotate>();
        if (anim == null) {
            anim = obj.AddComponent<AnimRotate>();
        }
        anim.mList.Clear();
        anim.mList.AddRange(rotates);
        anim.mSpeed = spd;
        anim.mStepCall = call;
        anim.mRotateCall = rotate_call;
        anim.StartPlay();
        return anim;
    }
    private float _time;
    private float mInitVal;//当前初始化值
    private float mTargetVal;//目标值
    private int mCurIndex;
    private float mTotalTime;//总时间
    public void StartPlay() {
        this._time = 0;
        this.mCurIndex = 0;
        this.mInitVal = this.transform.localEulerAngles.z;
        this.mTargetVal = this.mList[this.mList.Count - 1];
        this.mTotalTime = (this.mTargetVal - this.mInitVal) / this.mSpeed;

        this.SetStep(0);
    }
    private float mCurTarget;//当前阶段目标
    public void SetStep(int step) {
        this.mCurIndex = step;
        this.mCurTarget = this.mList[this.mCurIndex];
    }
    public void FinishStep() {//完成当前进入，并进入下一进度
        if (this.mCurIndex == this.mList.Count - 1) {//结束
            this.mStepCall(this.mCurIndex, true);
            GameObject.Destroy(this);
            return;
        } else {
            this.mStepCall(this.mCurIndex, false);
        }
        this.SetStep(this.mCurIndex + 1);

        this.mWaitTime = 0.5f;//每完成一个进度，停顿0.5秒
    }

    private float mWaitTime;//连抽中间停顿时间
	void Update () {
        if (this.mWaitTime > 0) {
            this.mWaitTime -= Time.deltaTime;
            return;
        }
        //第一秒跟最后一秒都有一个缓变过程
        if (this._time < 0.5f) {
            this._time += Time.deltaTime * Mathf.Min(1, Mathf.Max(0.02f, this._time) * 1.5f) / 0.5f;
        //} else if (this._time > this.mTotalTime - 0.8f) {
        //    this._time += Time.deltaTime * Mathf.Max(0.03f, (this.mTotalTime - this._time)/0.8f);
        } else {
            this._time += Time.deltaTime;
        }

        float _z = Mathf.Lerp(this.mInitVal, this.mTargetVal, this._time / this.mTotalTime);
        if (_z >= this.mCurTarget) {
            _z = this.mCurTarget;
            this.transform.localEulerAngles = new Vector3(0, 0, -this.mCurTarget);
            this.FinishStep();
        } else {
            this.transform.localEulerAngles = new Vector3(0, 0, -_z);
        }
        if (this.mRotateCall != null) {
            this.mRotateCall(_z);
        }
	}
}
