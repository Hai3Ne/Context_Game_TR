using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_WZQBattle_Message : UIItem {
    public UIScrollView mScrollInfo;
    public GameObject mItemLbInfo;//单条信息
    public GameObject mItemResult;//结算信息

    private float _y;//y轴坐标
    public void AddMessage(string msg) {
        GameObject obj = GameObject.Instantiate(this.mItemLbInfo, this.mScrollInfo.transform);
        obj.SetActive(true);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = new Vector3(this.mItemLbInfo.transform.localPosition.x, _y);
        UILabel lb = obj.GetComponent<UILabel>();
        lb.text = msg;
        _y -= lb.height + 10;
        this.mScrollInfo.ResetPosition();
    }

    public void AddMessage(CMD_S_GameEnd s_game_end) {
        GameObject obj = GameObject.Instantiate(this.mItemResult, this.mScrollInfo.transform);
        obj.SetActive(true);
        Transform tf = obj.transform;
        tf.localScale = Vector3.one;
        tf.localPosition = new Vector3(this.mItemResult.transform.localPosition.x, _y);

        UILabel lb_p1_name = tf.Find("item_lb_user_1").GetComponent<UILabel>();
        UILabel lb_p1_gold = tf.Find("item_lb_user_1/item_lb_gold_1").GetComponent<UILabel>();
        UILabel lb_p2_name = tf.Find("item_lb_user_2").GetComponent<UILabel>();
        UILabel lb_p2_gold = tf.Find("item_lb_user_2/item_lb_gold_2").GetComponent<UILabel>();
        UILabel lb_win = tf.Find("item_lb_win").GetComponent<UILabel>();

        if (s_game_end.WinUser == WZQTableManager.FristHandSeat) {
            lb_win.text = "[黑子胜]";
        } else if (s_game_end.WinUser == ushort.MaxValue) {
            lb_win.text = "[平局]";
        } else {
            lb_win.text = "[白子胜]";
        }
        RoleInfo[] roles = new RoleInfo[2];
        roles[RoleManager.Self.ChairSeat] = RoleManager.Self;
        roles[WZQTableManager.mOtherRole.ChairSeat] = WZQTableManager.mOtherRole;

        lb_p1_name.text = GameUtils.SubStringByWidth(lb_p1_name, roles[0].NickName, 180);
        if (s_game_end.UserScore[0] >= 0) {
            lb_p1_gold.text = string.Format("+{0}", s_game_end.UserScore[0]);
        } else {
            lb_p1_gold.text = s_game_end.UserScore[0].ToString();
        }
        lb_p2_name.text = GameUtils.SubStringByWidth(lb_p2_name, roles[1].NickName, 180);
        if (s_game_end.UserScore[1] >= 0) {
            lb_p2_gold.text = string.Format("+{0}", s_game_end.UserScore[1]);
        } else {
            lb_p2_gold.text = s_game_end.UserScore[1].ToString();
        }
        _y -= 146;
        this.mScrollInfo.ResetPosition();
    }

    public void OnSystemMessage(NetCmdType type, NetCmdPack pack) {//房间内消息
        this.AddMessage(Tools.MessageColor(pack.ToObj<SC_GR_GF_SystemMessage>().Message));
    }
    public void OnGameResult(NetCmdType type, NetCmdPack pack) {//游戏结算
        this.AddMessage(pack.ToObj<CMD_S_GameEnd>());
    }
    public void Awake() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_SYSTEM_MESSAGE, this.OnSystemMessage);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnGameResult);
    }

    public void OnDestroy() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_SYSTEM_MESSAGE, this.OnSystemMessage);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnGameResult);
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "panel_msg":
                this.mScrollInfo = tf.GetComponent<UIScrollView>();
                TimeManager.DelayExec(0, this.mScrollInfo.ResetPosition);
                break;
            case "item_lb_info":
                this.mItemLbInfo = tf.gameObject;
                this.mItemLbInfo.SetActive(false);
                break;
            case "item_result":
                this.mItemResult = tf.gameObject;
                this.mItemResult.SetActive(false);
                break;
        }
    }
}
