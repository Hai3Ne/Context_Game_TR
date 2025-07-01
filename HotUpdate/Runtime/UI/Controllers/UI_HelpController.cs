using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_HelpController : IUIControllerImp {
    public GameObject mBtnClose;
    public UIScrollView mScrollView;
    public UILabel mLbInfo;

    public override void Init(object data) {
        base.Init(data);
    }
    public override void Show() {
        WndManager.LoadUIGameObject("UI_Help",
			SceneObjMgr.Instance.UIPanelTransform,
			delegate(GameObject obj) {
				uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
				TweenShow();
                GameUtils.Traversal(obj.transform, this.OnNodeAsset);
                UIEventListener.Get(this.mBtnClose).onClick = this.OnButtonClick;

                //Kubility.KAssetBundleManger.Instance.ResourceLoad<TextAsset>(ResPath.RootPath + "Config/Help", (asset) => {
                //    this.mLbInfo.text = (asset.MainObject as TextAsset).text;
                //    TimeManager.DelayExec(0, () => {
                //        this.mScrollView.ResetPosition();
                //    });
                //});
            }
        );
        base.Show();
    }

    public override void Close() {
        base.Close();
    }

    public void OnButtonClick(GameObject obj) {
        if (this.mBtnClose == obj) {//
            this.Close();
        }
    }
    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "btn_close":
                this.mBtnClose = tf.gameObject;
                break;
            case "scrollview_info":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "lb_info":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
