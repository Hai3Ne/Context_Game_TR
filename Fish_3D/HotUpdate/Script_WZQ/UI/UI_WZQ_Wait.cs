using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WZQ_Wait : UILayer {
    public UILabel mLbTick;
    public UILabel mLbDownTick;

    public int mFuncType;//操作类型 1.悔棋  2.和棋
    //public float mWaitTime;//等待时间
    public void InitData(int type, int wait) {//1.悔棋  2.和棋
        this.mFuncType = type;
        //this.mWaitTime = wait - 0.01f;

        //this.SetWaitTime((int)this.mWaitTime);
        this._count = 0;
        this.SetWaitTime(this._count);
    }
    public void SetWaitTime(int time) {
        switch (time) {
            case 0:
                this.mLbDownTick.text = ".";
                break;
            case 1:
                this.mLbDownTick.text = "..";
                break;
            case 2:
            default:
                this.mLbDownTick.text = "...";
                break;
        }
    }

    private int _count = 0;
    private float _time;
    public void Update() {
        this._time += Time.deltaTime*1.25f;
        if (this._time >= 1) {
            this._time -= 1;
            this._count = (this._count + 1) % 3;
            this.SetWaitTime(this._count);
        }
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_S_REGRET_FAILE://悔棋失败
            case NetCmdType.SUB_S_REGRET_RESULT://悔棋结果
            case NetCmdType.SUB_S_PEACE_ANSWER://和棋应答
            case NetCmdType.SUB_S_REGRET_REQ://悔棋请求
            case NetCmdType.SUB_S_PEACE_REQ://和棋请求
            case NetCmdType.SUB_S_GAME_END://游戏结束
                this.Close();
                break;
        }
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_REGRET_FAILE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_REGRET_RESULT, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PEACE_ANSWER, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_REGRET_REQ, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PEACE_REQ, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnNetHandle);
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_REGRET_FAILE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_REGRET_RESULT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PEACE_ANSWER, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_REGRET_REQ, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PEACE_REQ, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_END, this.OnNetHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "item_btn_cancel"://取消等待
                if (this.mFuncType == 1) {//1.悔棋
                    NetClient.Send(NetCmdType.SUB_C_CANCEL_REGERT, new CMD_C_CANCEL_REGERT());
                } else if (this.mFuncType == 2) {//2.和棋
                    NetClient.Send(NetCmdType.SUB_C_CANCEL_PEACE, new CMD_C_CANCEL_PEACE());
                } else {
                    this.Close();
                }
                break;
            default:
                return;
        }
        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_tick":
                this.mLbTick = tf.GetComponent<UILabel>();
                break;
            case "item_lb_downcount":
                this.mLbDownTick = tf.GetComponent<UILabel>();
                break;
        }
    }
}
