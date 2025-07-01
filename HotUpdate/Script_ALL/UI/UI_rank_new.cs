using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_rank_new : UILayer {
    public UIWidget mWebView;
    public override void OnNodeLoad() { }
    public override void OnEnter() { 
#if UNITY_EDITOR
	    string url = "https://www.789278.com/Active/Activity/HonerList.html?type=1";
#elif UNITY_IOS
	    string url = "https://www.789278.com/Active/Activity/HonerList.html?type=3";
#else
	    string url = "https://www.789278.com/Active/Activity/HonerList.html?type=2";
#endif

     /*   WebViewManager.ShowUrl(this.mWebView, url, 1.0f, () => {
            this.Close();
        });*/
    }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) { 
        switch(name){
            case "webview":
                this.mWebView = tf.GetComponent<UIWidget>();
                break;
        }
    }
}
