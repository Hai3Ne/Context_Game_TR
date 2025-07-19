using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 金币同步  只用来同步自己的游戏数据
/// </summary>
public class SygnGold : MonoBehaviour {
    public UILabel mLbGold;

    private MsgNotificator notify; 

    public void Awake() {
        this.mLbGold = this.GetComponent<UILabel>();

        RoleItemModel.Instance.RegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        if (SceneLogic.Instance.FModel != null) {
            this.notify = SceneLogic.Instance.FModel.Notify;
        }
        if (this.notify != null) {
            this.notify.RegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        }
    }

    public void OnDestroy() {
        RoleInfoModel.Instance.UnRegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        if (this.notify != null) {
            this.notify.UnRegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        }
    }
    void OnPlayGoldNumChange(object o) {
        if (SceneLogic.Instance.IsGameOver == false) {
            byte clientSeat = SceneLogic.Instance.FModel.GetTableRolerSeat(RoleInfoModel.Instance.Self.UserID);
            this.mLbGold.text = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(clientSeat).ToString();
        }
    }
}
