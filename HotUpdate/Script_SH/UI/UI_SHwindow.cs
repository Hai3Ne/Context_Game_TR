using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SHwindow : UILayer {
    public UILabel mLbInfo;

    public VoidDelegate mCallOK;
    public VoidDelegate mCallCancel;

    public void InitData(string msg, VoidDelegate call_ok, VoidDelegate call_cancel) {
        this.mLbInfo.text = msg;
        this.mCallOK = call_ok;
        this.mCallCancel = call_cancel;
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_ok":
                this.Close();
                if (this.mCallOK != null) {
                    this.mCallOK();
                }
                break;
            case "btn_cancel":
                this.Close();
                if (this.mCallCancel != null) {
                    this.mCallCancel();
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_msg":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
