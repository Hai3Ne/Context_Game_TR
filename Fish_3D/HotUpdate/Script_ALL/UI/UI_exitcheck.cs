using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_exitcheck : UILayer {
    public enum FunType {
        Login,
        Exit,
    }
    public UILabel mLbInfo;

    public FunType mCurType;
    public void InitData(FunType type) {
        this.mCurType = type;
        if (type == FunType.Login) {
            this.mLbInfo.text = "确定要返回登录吗？";
        } else {
            this.mLbInfo.text = "确定要退出游戏吗？";
        }
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_ok":
                if (this.mCurType == FunType.Login) {
                    UserManager.GoLogin();
                } else {
                    SDKMgr.ExitGame();
                }
                break;
            case "btn_cancel":
                this.Close();
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
