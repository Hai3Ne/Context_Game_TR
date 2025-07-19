using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SHchooseroom : UILayer {
    private int ItemWidth = 436;

    public UITexture mTexturePlayer;
    public UISprite mSprVIP;
    public UILabel mLbName;
    public UILabel mLbGameID;
    public UILabel mLbGold;
    public GameObject mItemRoom;
    public UIScrollView mScrollView;

    public List<Item_SHchooseroom> mItemList = new List<Item_SHchooseroom>();
    public void InitData()
    {
        List<tagGameServer> list = HallHandle.GetServerList(SHGameConfig.KindID);
        Item_SHchooseroom item;
        float start_x = -ItemWidth * Mathf.Min(3, list.Count - 1) * 0.5f;
        for (int i = 0; i < list.Count; i++) {
            item = this.AddItem<Item_SHchooseroom>(this.mItemRoom, this.mScrollView.transform);
            item.InitData(i,list[i]);
            item.transform.localPosition = new Vector3(start_x + ItemWidth * i, 0);
            this.mItemList.Add(item);
        }
        if (list.Count > 4) {
            this.mScrollView.enabled = true;
            this.mScrollView.ResetPosition();
        } else {
            this.mScrollView.enabled = false;
        }
        this.UpdateUserInfo();
    }
    private void SetFace(ushort face) {
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(face);
    }
    public void UpdateUserInfo() {
        this.mLbName.text = HallHandle.NickName;
        this.mLbGameID.text = string.Format("ID:{0}", HallHandle.GameID);
        this.mLbGold.text = HallHandle.UserGold.ToString();
        this.SetFace(HallHandle.FaceID);
        this.mSprVIP.gameObject.SetActive(HallHandle.MemberOrder > 0);
        this.mSprVIP.spriteName = string.Format("vip_{0}", HallHandle.MemberOrder);
        this.mSprVIP.MakePixelPerfect();
    }
    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GR_PROPERTY_EFFECT://VIP通知
                this.UpdateUserInfo();
                break;
            case NetCmdType.SUB_GP_USER_FACE_INFO://更改头像
                this.SetFace(HallHandle.FaceID);
                break;
        }
    }
    private void OnEventHandle(GameEvent event_type, object obj) {
        switch (event_type) {
            case GameEvent.Hall_UserInfoChange://用户信息变更
                this.UpdateUserInfo();
                break;
        }
    }

    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);

        TimeManager.DelayExec(this, UI.AnimTime, () => {
            AudioManager.PlayMusic(GameEnum.All, FishConfig.Instance.AudioConf.datingBgm.ToString());
        });
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_back":
                this.Close();
                GameSceneManager.BackToHall(GameEnum.None);
                break;
        }
    }

    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "texture_player":
                this.mTexturePlayer = tf.GetComponent<UITexture>();
                break;
            case "spr_vip":
                this.mSprVIP = tf.GetComponent<UISprite>();
                break;
            case "lb_game_id":
                this.mLbGameID = tf.GetComponent<UILabel>();
                break;
            case "lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_room":
                this.mItemRoom = tf.gameObject;
                this.mItemRoom.SetActive(false);
                break;
            case "scrollview":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
        }
    }
}
