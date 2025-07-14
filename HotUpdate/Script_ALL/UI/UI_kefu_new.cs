using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_kefu_new : UILayer {
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                break;
            case "btn_copy":
                SDKMgr.Instance.copyTextToClipBoard(ConstValue.WXNo);
                SystemMessageMgr.Instance.ShowMessageBox("复制成功", 1);
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) { }
}
