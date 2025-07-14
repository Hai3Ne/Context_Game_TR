using UnityEngine;
using System.Collections;

/// <summary>
/// 世界BOSS奖池数字跳动动画
/// </summary>
public class UI_BossChestNum : MonoBehaviour {
    public UILabel mLbNum;
    public UISlider mSliderHP;
    public UILabel mLbHpCount;
    public UILabel mLbKillerName;//击杀者名称
    public UILabel mLbCountdown;//活动剩余倒计时
    public UISpriteAnimation mAnimLight;//闪光灯特效

    private float mTotalTime;
    private long mInitNum;
    private long mTargetNum;
    private float mTime;
    public bool mIsKill;//BOSS是否已经被击杀
    public void SetKiller(string killer_name) {
        if (this.gameObject.activeSelf == false) {
            this.gameObject.SetActive(true);
            this.mAnimLight.Pause();
        }
        this.mLbCountdown.transform.parent.gameObject.SetActive(true);
        this.mLbNum.transform.parent.gameObject.SetActive(false);

        this.mIsKill = true;
        this.mLbKillerName.text = string.Format(StringTable.GetString("ItemNotice13"), killer_name);
    }

    public void StartPlay(float time, long target, uint multiple) {//根据倍率判断数字跳转时间
        this.mIsKill = false;

        this.mLbCountdown.transform.parent.gameObject.SetActive(false);
        this.mLbNum.transform.parent.gameObject.SetActive(true);

        if (this.gameObject.activeSelf == false) {
            this.gameObject.SetActive(true);
            this.mAnimLight.Pause();
        }

        if (long.TryParse(this.mLbNum.text, out this.mInitNum) == false) {
            this.mInitNum = 0;
        }

        if (this.mInitNum == 0) {//第一次设置金额不需要动画
            this.SetNum(target);
            this.enabled = false;
        } else {
            this.mTargetNum = target;

            this.mTime = 0;
            if (this.mTargetNum > this.mInitNum) {
                this.mTotalTime = Mathf.Min(time, Mathf.Sqrt((this.mTargetNum - this.mInitNum) / multiple * 1f) * 0.5f + 0.25f);
            } else {
                this.mTotalTime = Mathf.Min(time, Mathf.Sqrt((this.mInitNum - this.mTargetNum) / multiple * 1f) * 0.5f + 0.25f);
            }

            this.SetNum(this.mInitNum);
            this.enabled = true;
        }
    }

    public void SetNum(long num) {
        this.mLbNum.text = num.ToString();
    }

    private const float big_time = 0.2f;//放大时间
    private const float small_time = 0.3f;//缩小时间
	private void Update () {
        mTime += Time.deltaTime;

        if (this.mTotalTime > 3.5f) {
            if (this.mTime < big_time) {//放大
                float scale = this.mLbNum.transform.localScale.x;
                scale = Mathf.Lerp(scale, 1.5f, this.mTime / big_time);
                this.mLbNum.transform.localScale = new Vector3(scale, scale, scale);
            } else if (this.mTime + small_time > this.mTotalTime) {//缩小
                float t = (this.mTotalTime - this.mTime) / small_time;
                //float t = (this.mTotalTime - this.mTime) / (this.mTotalTime - big_time);
                float scale = Mathf.Lerp(1, 1.5f, Mathf.Clamp01(t));
                this.mLbNum.transform.localScale = new Vector3(scale, scale, scale);
            }
        } else if(this.mTime < small_time){
            float scale = this.mLbNum.transform.localScale.x;
            scale = Mathf.Lerp(scale, 1f, this.mTime / small_time);
            this.mLbNum.transform.localScale = new Vector3(scale, scale, scale);
        }

        if (this.mTime < this.mTotalTime) {
            this.SetNum(this.Lerp(this.mInitNum, this.mTargetNum, Mathf.Sqrt(this.mTime / this.mTotalTime)));
        } else {
            this.SetNum(this.mTargetNum);
            this.enabled = false;
        }
	}

    public long Lerp(long from, long to, double p) {
        if (p < 0) {
            p = 0;
        } else if (p > 1) {
            p = 1;
        }
        return (long)(from * (1 - p) + to * p);
    }
    
    public void OnDestroy() {
        if (this.mIsKill == false) {
            MtaManager.EndWorldBox();
        }
    }


}
