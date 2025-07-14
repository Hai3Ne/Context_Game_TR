using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WZQ_Dialog : UILayer {
    public UILabel mLbInfo;
    public UILabel mLbDownCount;
    public UILabel mLbCancel;//取消文本
    public UILabel mLbOK;//确认文本

    private VoidDelegate mCallOK;//确认回调
    private VoidDelegate mCallCancel;//取消回调
    private float mDownCount = 15;//拒绝倒计时
    /// <summary>
    /// 1.求和求请
    /// 2.悔棋请求
    /// 3.认输确认
    /// 4.悔棋确认
    /// 5.求和确认
    /// </summary>
    public void InitData(int type, VoidDelegate call_ok, VoidDelegate call_cancel) {
        this.mCallOK = call_ok;
        this.mCallCancel = call_cancel;
        switch (type) {
            case 1://求和
                this.mLbInfo.text = "对方请求[ffff00]和棋[-]是否同意?";
                this.mLbInfo.alignment = NGUIText.Alignment.Automatic;
                this.mLbOK.text = "同意";
                this.mLbCancel.text = "拒绝";
                this.mDownCount = 15;//15秒倒计时  不作处理直接当拒接处理
                break;
            case 2://悔棋
                this.mLbInfo.text = "对方请求[ff0000]悔棋[-]是否同意?";
                this.mLbInfo.alignment = NGUIText.Alignment.Automatic;
                this.mLbOK.text = "同意";
                this.mLbCancel.text = "拒绝";
                this.mDownCount = 15;//15秒倒计时  不作处理直接当拒接处理
                break;
            case 3://认输确认
                //long gold = Tools.Min(RoleManager.Self.GoldNum,WZQTableManager.mOtherRole.GoldNum,(long)WZQTableManager.MaxPayMoney);
                this.mLbInfo.text = "确定要认输吗？";// string.Format("认输将从当前携带游戏币中扣除\n[ffff00]{0}[-]乐豆，确定要认输吗？", gold);
                this.mLbInfo.alignment = NGUIText.Alignment.Automatic;
                this.mLbOK.text = "确定";
                this.mLbCancel.text = "取消";
                this.mDownCount = -1;
                break;
            case 4://悔棋确认
                this.mLbInfo.text = "是否确定发起悔棋？";
                this.mLbInfo.alignment = NGUIText.Alignment.Automatic;
                this.mLbOK.text = "确定";
                this.mLbCancel.text = "取消";
                this.mDownCount = -1;
                break;
            case 5://求和确认
                this.mLbInfo.text = "是否确定发起求和？";
                this.mLbInfo.alignment = NGUIText.Alignment.Automatic;
                this.mLbOK.text = "确定";
                this.mLbCancel.text = "取消";
                this.mDownCount = -1;
                break;
        }
        this.SetDownCount((int)this.mDownCount);
    }

    public void SetDownCount(int time) {
        if (time >= 0) {
            this.mLbDownCount.text = string.Format("({0})", time);
        } else {
            this.mLbDownCount.text = string.Empty;
        }
    }
    public void Update() {
        if (this.mDownCount > 0) {
            this.mDownCount -= Time.deltaTime;
            if (this.mDownCount < 0) {
                if (this.mCallCancel != null) {
                    this.mCallCancel();
                }
                this.Close();
            } else {
                this.SetDownCount((int)this.mDownCount);
            }
        }
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        this.Close();
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PEACE_ANSWER, this.OnNetHandle);
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PEACE_ANSWER, this.OnNetHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "item_btn_cancel"://取消
                if (this.mCallCancel != null) {
                    this.mCallCancel();
                }
                this.Close();
                break;
            case "item_btn_ok"://确定
                if (this.mCallOK != null) {
                    this.mCallOK();
                }
                this.Close();
                break;
            default:
                return;
        }
        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_info":
                this.mLbInfo = tf.GetComponent<UILabel>();
                break;
            case "item_lb_downcount":
                this.mLbDownCount = tf.GetComponent<UILabel>();
                break;
            case "item_lb_cancel":
                this.mLbCancel = tf.GetComponent<UILabel>();
                break;
            case "item_lb_ok":
                this.mLbOK = tf.GetComponent<UILabel>();
                break;
        }
    }
}
