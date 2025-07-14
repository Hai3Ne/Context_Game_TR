using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_XiuYuQi : UILayer {
    public UILabel mLbNum;

    private int _pre_time = -1;
    private float _down_count;

    public void InitData(float sec) {
        this._down_count = Mathf.Min(200, sec);
        this.SetTime((int)this._down_count);
    }
    
    public void SetTime(int time) {
        this._pre_time = time;
        this.mLbNum.text = Mathf.Max(time, 0).ToString();
    }
    public void Update() {
        if (this._down_count > Time.deltaTime) {
            this._down_count -= Time.deltaTime;
            if (this._pre_time != (int)this._down_count) {
                this.SetTime((int)this._down_count);
            }
        } else {
            this.Close();
        }
    }

    private void OnHandle(NetCmdType type, NetCmdPack pack) {
        this.Close();
    }

    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_SWITCH_SCENE_LKPY, this.OnHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_RADIATION_LKPY, this.OnHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_FISH_TRACE_LKPY, this.OnHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_FISH_TRACE2_LKPY, this.OnHandle);
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_SWITCH_SCENE_LKPY, this.OnHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_RADIATION_LKPY, this.OnHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_FISH_TRACE_LKPY, this.OnHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_FISH_TRACE2_LKPY, this.OnHandle);

        LKGameManager.mSceneRemTime = 0;
    }
    public override void OnButtonClick(GameObject obj) { }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_num":
                this.mLbNum = tf.GetComponent<UILabel>();
                break;
        }
    }
}
