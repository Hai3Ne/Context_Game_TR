using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Share : UILayer {
    public GameObject mObjEveryDay;
    public GameObject mObjJiuJiJin;

    public bool mIsGame;//是否游戏中
    public void InitData(bool is_game) {
        this.mIsGame = is_game;
        this.mObjEveryDay.SetActive(is_game == false);
        this.mObjJiuJiJin.SetActive(is_game == true);
    }

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                break;
            case "btn_wx"://分享给微信好友
                if (UserManager.IsBingWX) {
                    this.Close();
                    SDKMgr.Instance.WXWebShare(
                        this.mIsGame,
                        ConstValue.ShareURL,
                        "首创英雄召唤玩法 2018新一代3D捕鱼",
                        "免费领更多乐豆，体验全新道具，挑战神秘海底首领，玩法多样，尽在《1378捕鱼3D》",
                        false
                    );
                } else {
                    //UI.EnterUI<UI_BindNotice>(ui => ui.InitData(false));
                    UI.EnterUI<UI_BindNotice>(GameEnum.All).InitData(false);
                }
                break;
            case "btn_pyq"://分享到朋友圈
                if (UserManager.IsBingWX) {
                    this.Close();
                    SDKMgr.Instance.WXWebShare(
                        this.mIsGame,
                        ConstValue.ShareURL,
                        string.Format("{0}推荐您加入《1378捕鱼3D》，免费领取乐豆！", HallHandle.NickName),
                        "免费领更多乐豆，体验全新道具，挑战神秘海底首领，玩法多样，尽在《1378捕鱼3D》",
                        true
                    );
                } else {
                    //UI.EnterUI<UI_BindNotice>(ui => ui.InitData(false));
                    UI.EnterUI<UI_BindNotice>(GameEnum.All).InitData(false);
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "text_everyday":
                this.mObjEveryDay = tf.gameObject;
                break;
            case "text_jiujijin":
                this.mObjJiuJiJin = tf.gameObject;
                break;
        }
    }
}
