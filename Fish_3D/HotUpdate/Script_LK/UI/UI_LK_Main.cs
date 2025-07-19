using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_Main : UILayer {
    private int PageCount = 10;//每页显示个数

    public UITexture mTexturePlayer;
    public UILabel mLbGold;
    public UILabel mLbName;
    public UILabel mLbID;
    public UISprite mSprVIP;
    public GameObject mBtnPrePage;//上一页
    public GameObject mBtnNextPage;//下一页
    public UIGrid mGridRoom;
    public GameObject mItemRoom;

    public List<Item_LK_Main> mItemList = new List<Item_LK_Main>();
    public List<tagGameServer> mServerList = new List<tagGameServer>();
    public int mCurPage;
    public int mMaxPage;
    public void InitData() {
        this.mServerList = HallHandle.GetServerList(LKGameConfig.KindID);
        this.mMaxPage = (this.mServerList.Count - 1) / PageCount;
        this.SetPage(0);
        this.UpdateUserInfo();
    }
    private void SetFace(ushort face) {
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(face);
    }
    public void UpdateUserInfo() {
        this.mLbName.text = HallHandle.NickName;
        this.mLbID.text = string.Format("ID:{0}", HallHandle.GameID);
        this.mLbGold.text = HallHandle.UserGold.ToString();
        this.SetFace(HallHandle.FaceID);
        this.mSprVIP.gameObject.SetActive(HallHandle.MemberOrder > 0);
        this.mSprVIP.spriteName = string.Format("vip_{0}", HallHandle.MemberOrder);
        this.mSprVIP.MakePixelPerfect();
    }
    public void SetPage(int page) {
        foreach (var room in this.mItemList) {
            room.gameObject.SetActive(false);
        }
        this.mCurPage = page;
        this.mBtnPrePage.SetActive(this.mCurPage > 0);
        this.mBtnNextPage.SetActive(this.mCurPage < this.mMaxPage);
        int count = Mathf.Min(PageCount, this.mServerList.Count - this.mCurPage * PageCount);
        Item_LK_Main item;
        for (int i = 0; i < count; i++) {
            if (this.mItemList.Count > i) {
                item = this.mItemList[i];
                item.gameObject.SetActive(true);
            } else {
                item = this.AddItem<Item_LK_Main>(this.mItemRoom, this.mGridRoom.transform);
                this.mItemList.Add(item);
            }
            int index = i + page * PageCount;
            item.InitData(index, this.mServerList[index]);
        }
        this.mGridRoom.Reposition();
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

        if (LKDataManager.mAudio == null) {
            LKDataManager.InitData();
        }
        //TimeManager.DelayExec(UI.AnimTime, () => {
        //    AudioManager.PlayMusic(GameEnum.Fish_LK, LKDataManager.mAudio.RoomBgm[Random.Range(0, LKDataManager.mAudio.RoomBgm.Length)]);
        //});

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
            case "btn_pre_page":
                if (this.mCurPage > 0) {
                    this.SetPage(this.mCurPage - 1);
                }
                break;
            case "btn_next_page":
                if (this.mCurPage < this.mMaxPage) {
                    this.SetPage(this.mCurPage + 1);
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "texture_player":
                this.mTexturePlayer = tf.GetComponent<UITexture>();
                break;
            case "lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "lb_id":
                this.mLbID = tf.GetComponent<UILabel>();
                break;
            case "spr_vip":
                this.mSprVIP = tf.GetComponent<UISprite>();
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "btn_pre_page":
                this.mBtnPrePage = tf.gameObject;
                break;
            case "btn_next_page":
                this.mBtnNextPage = tf.gameObject;
                break;
            case "item_room":
                this.mItemRoom = tf.gameObject;
                this.mItemRoom.SetActive(false);
                break;
            case "grid_room":
                this.mGridRoom = tf.GetComponent<UIGrid>();
                break;
        }
    }
}
