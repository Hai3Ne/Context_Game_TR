using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_poor : UILayer {
    public UILabel mLbInfo;
    public VoidDelegate mCallCancel;
    public void InitData(int type, VoidDelegate cancel_call = null) {//1.乐豆不足  2.VIP等级不足
        if (type == 1) {
            this.mLbInfo.text = "[c6e8ff]当前[fff602]乐豆不足[-],是否前往充值？";
        } else {
            this.mLbInfo.text = "[c6e8ff]当前[fff602]会员等级不足[-],是否前往充值？";
        }
        this.mCallCancel = cancel_call;
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                if (this.mCallCancel != null) {
                    this.mCallCancel();
                }
                break;
            case "btn_go":
                if (HallHandle.CheckPerfect(false)) {
                    this.Close();
                    //UI.EnterUI<UI_Shop>(null);
                    UI.EnterUI<UI_Shop>(GameEnum.All);
                } else {
                    SystemMessageMgr.Instance.ShowMessageBox("请返回大厅完善个人信息");
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_info":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
