using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActivityNotice : UILayer {
    public class NoticeInfo {
        public string ImgPath;//图片地址
        public string URL;//跳转链接
    }

    public UITexture mTextureMain;

    public List<NoticeInfo> mInfoList;
    public void InitData(List<NoticeInfo> list) {
        this.mInfoList = list;
        GameUtils.DownLoadTexture(this, this.mTextureMain, ConstValue.ActivityNoticeURL + this.mInfoList[0].ImgPath);
    }


    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() {
        MainEntrace.Instance.EnterNextTick();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "texture_main":
                if (string.IsNullOrEmpty(this.mInfoList[0].URL) == false) {
                    Application.OpenURL(this.mInfoList[0].URL);
                }
                break;
            case "btn_ok":
                if (this.mInfoList.Count > 1) {
                    this.mInfoList.RemoveAt(0);
                    this.InitData(this.mInfoList);
                    this.TweenShow();
                } else {
                    this.Close();
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "texture_main":
                this.mTextureMain = tf.GetComponent<UITexture>();
                break;
        }
    }
}
