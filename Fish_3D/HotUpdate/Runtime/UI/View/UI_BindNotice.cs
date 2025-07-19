using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BindNotice : UILayer {
    public UILabel mLbBind;//帐号金额数量过大，强制绑定
    public UILabel mLbNoFunc;//没有绑定微信无法操作

    public bool mIsGoLogin;//是否强制跳转到登录

    public void InitData(bool is_go_login) {//是否强制跳转到登录界面
        this.mIsGoLogin = is_go_login;
        if (is_go_login) {
            this.mLbNoFunc.gameObject.SetActive(false);
        } else {
            this.mLbBind.gameObject.SetActive(false);
        }
    }

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                if (this.mIsGoLogin) {
                    UserManager.GoLogin();
                }
                break;
            case "btn_copy":
                SDKMgr.Instance.copyTextToClipBoard(ConstValue.WXNo);
                SystemMessageMgr.Instance.ShowMessageBox("复制成功", 1);
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_bind":
                this.mLbBind = tf.GetComponent<UILabel>();
                break;
            case "lb_nofunc":
                this.mLbNoFunc = tf.GetComponent<UILabel>();
                break;
        }
    }
}
