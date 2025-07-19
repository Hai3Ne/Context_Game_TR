using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_VersionInfo : UILayer {
    public UIScrollView mScrollView;
    public UILabel mLbInfo;
    public override void OnNodeLoad() {
        //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<TextAsset>(ResPath.RootPath + "Config/NewContent");
        //ResManager.LoadAsset<TextAsset>(data,(ta)=>{
        //    this.mLbInfo.text = ta.text;
        //    TimeManager.DelayExec(0, () => {
        //        this.mScrollView.ResetPosition();
        //    });
        //    ResManager.UnloadAB(data);
        //});

        string content = string.Empty;
        ResManager.LoadText(GameEnum.All, "Config/NewContent", out content);
        this.mLbInfo.text = content;
        TimeManager.DelayExec(0, () =>
        {
            this.mScrollView.ResetPosition();
        });
    }
    public override void OnEnter() { 
    }
    public override void OnExit() {
		MainEntrace.Instance.EnterNextTick();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "scrollview_info":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "lb_info":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
