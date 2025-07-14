using UnityEngine;
using System.Collections;

/// <summary>
/// 数字跳动动画
/// </summary>
public class ActionNumAnim : MonoBehaviour {
    public UILabel mLbNum;
    public float mTotalTime;
    public int mCurNum;
    public int mTargetNum;

    public float mTime;

    public void StartPlay(UILabel lb_num, float time, int cur, int target) {
        this.mLbNum = lb_num;
        this.mTotalTime = time;
        this.mCurNum = cur;
        this.mTargetNum = target;
        this.mTime = 0;

        this.SetNum(cur);
    }

    public void ResetTarget(float time, int target) {
        this.mCurNum = (int)Mathf.Lerp(this.mCurNum, this.mTargetNum, this.mTime / this.mTotalTime);
        this.mTotalTime = time;
        this.mTargetNum = target;
        this.mTime = 0;

        this.SetNum(this.mCurNum);
    }

    public void SetNum(int num) {
        this.mLbNum.text = num.ToString();
    }

	private void Update () {
        if (this.mTime < this.mTotalTime) {
            mTime += Time.deltaTime;
            this.SetNum((int)Mathf.Lerp(this.mCurNum, this.mTargetNum, this.mTime / this.mTotalTime));
        }
	}

}
