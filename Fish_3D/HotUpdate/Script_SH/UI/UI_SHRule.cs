using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SHRule : UILayer {
    public UIScrollView mScrollView;
    public UILabel mLbInfo;
    public override void OnNodeLoad() { }
    public override void OnEnter() {
        this.mScrollView.ResetPosition();
    }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_ok":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "scrollview":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "lb_info":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
