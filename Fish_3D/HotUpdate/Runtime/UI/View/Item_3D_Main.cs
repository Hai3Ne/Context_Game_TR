using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_3D_Main : UIItem {
    public UILabel mLbRate;
    public UISprite mSprLock;
    public Animator mAnim;

    private tagGameServer msg_server;

    public void InitData(tagGameServer server, uint min_mul, uint max_mul) {
        this.msg_server = server;
        this.mLbRate.text = string.Format("{0}-{1}倍", min_mul, max_mul);
        this.RefershLockInfo();
    }
    public void InitData(tagGameServer server, uint room_id) {
        TimeRoomVo vo = FishConfig.Instance.TimeRoomConf.TryGet(room_id);
        this.msg_server = server;
        this.mLbRate.text = string.Format("{0}-{1}倍", vo.Multiple[0], vo.RoomMultiple);
        this.RefershLockInfo();
    }

    public void RefershLockInfo() {//刷新进入限制
        if (this.msg_server == null || HallHandle.UserGold < this.msg_server.MinEnterScore || HallHandle.UserGold < this.msg_server.MinTableScore || HallHandle.MemberOrder < this.msg_server.MinEnterMember) {
            this.mSprLock.gameObject.SetActive(true);
        } else {
            this.mSprLock.gameObject.SetActive(false);
        }
    }

    public void Awake() {
        this.mAnim = this.gameObject.GetComponent<Animator>();
    }

    public override void OnButtonClick(GameObject obj) {
        if (obj == this.gameObject) {
            if (this.msg_server == null) {
                SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tips_RoomServerUnAvaible"), 1);
            }
            else if(this.mAnim == null || this.mAnim.enabled == true){
                MainEntrace.Instance.EnterGame(HallHandle.IsWXLogin, this.msg_server.ServerAddr, this.msg_server.ServerPort, HallHandle.UserID, HallHandle.Accounts, HallHandle.LoginPassword, HallHandle.LogonCode);
                //GameManager.EnterGame(GameEnum.Fish_3D, this.msg_server);
            }
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "beilv4":
                this.mLbRate = tf.GetComponent<UILabel>();
                break;
            case "spr_lock":
                this.mSprLock = tf.GetComponent<UISprite>();
                break;
        }
    }
}
