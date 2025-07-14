using UnityEngine;
using System.Collections;

public class DelayHide : MonoBehaviour {
    public float mHideTime;//延迟删除时间

    public void OnEnable() {
        //如果没有指定隐藏时间 则默认加5秒
        if (this.mHideTime < Time.realtimeSinceStartup) {
            this.mHideTime = Time.realtimeSinceStartup + 5;
        }
    }


    public void SetDelayHideTime(float time) {
        this.mHideTime = Time.realtimeSinceStartup + time;
    }


	private void Update () {
        if (this.mHideTime < Time.realtimeSinceStartup) {
            this.gameObject.SetActive(false);
        }
	}
}
