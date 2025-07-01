using UnityEngine;
using System.Collections;

/// <summary>
/// 休渔期倒计时动画脚本
/// </summary>
public class EffXiuYuQi : MonoBehaviour {
    public UISprite mSprNum1;
    public UISprite mSprNum2;
    public UISprite mSprNum3;

    private float mEndTime;//结束时间
    public void InitData(float time) {
        this.mEndTime = time;
        this.SetNum(this.mEndTime - Time.realtimeSinceStartup);
    }
    public void Awake() {
        this.mSprNum1 = this.transform.Find("num_1").GetComponent<UISprite>();
        this.mSprNum2 = this.transform.Find("num_2").GetComponent<UISprite>();
        this.mSprNum3 = this.transform.Find("num_3").GetComponent<UISprite>();
    }

    private int pre_num = -1;
    public void SetNum(float time) {
        int sec = (int)time;
        if (pre_num == sec) {
            return;
        }
        this.pre_num = sec;
        this.mSprNum1.spriteName = string.Format("num_000{0}_{0}", sec / 100 % 10);
        this.mSprNum2.spriteName = string.Format("num_000{0}_{0}", sec / 10 % 10);
        this.mSprNum3.spriteName = string.Format("num_000{0}_{0}", sec % 10);
    }

    public void Hide() {
        GameObject.Destroy(this.gameObject);
    }

    public void Update() {
        if (Time.realtimeSinceStartup > this.mEndTime) {
            SceneLogic.Instance.LogicUI.HideXiuYuQi();
        } else {
            this.SetNum(this.mEndTime - Time.realtimeSinceStartup);
        }
    }

	public void Shutdown()
	{
		this.Hide ();
	}

}
