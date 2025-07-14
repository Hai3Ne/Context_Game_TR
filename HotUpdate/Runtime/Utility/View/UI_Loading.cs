using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Loading : UILayer {
    public UILabel mLbInfo;

    public float mHideTime = 0;
    public void InitData(string msg, float time) {
        this.mLbInfo.text = msg;
        this.mHideTime = time;
    }

    public void Update() {
        if (this.mHideTime > Time.deltaTime) {
            this.mHideTime -= Time.deltaTime;
        } else {
            this.Close();
        }
    }


    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) { }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_info":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
