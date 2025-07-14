using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_wzqmain : UILayer {
    public UITexture mTexturePlayer;
    public UISprite mSprVIP;
    public UILabel mLbName;
    public UILabel mLbGameID;
    public UILabel mLbGold;
    public UIGrid mGridRoom;
    public Item_wzqmain_Room[] mRooms = new Item_wzqmain_Room[4];
    public Item_wzqmain_Dialog item_dialog;//进入房间弹框
    public GameObject mRoomMain;//房间操作

    public void InitData()
    {
        List<tagGameServer> list = HallHandle.GetServerList(WZQGameConfig.KindID);

        for (int i = 0; i < mRooms.Length; i++) {
            if (list.Count > i) {
                mRooms[i].gameObject.SetActive(true);
                mRooms[i].InitData(this,list[i]);
            } else {
                mRooms[i].gameObject.SetActive(false);
            }
        }
        this.mGridRoom.Reposition();

        this.UpdateUserInfo();
        this.SetShowMode(0);
    }
    private void SetFace(ushort face) {
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(face);
    }

    public void RefreshInfo(int type)
    {
        UpdateUserInfo();
        SetShowMode(type);
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
    public void EnterGame(tagGameServer server) {//进入游戏服
        GameManager.EnterGame(GameEnum.WZQ, server);
    }
    public void SetShowMode(int type) {//0.房间列表  1.房间操作  2.输入房间密码
        this.mGridRoom.gameObject.SetActive(false);
        this.mRoomMain.SetActive(false);
        this.item_dialog.Hide();
        switch (type) {
            case 0://房间列表
                this.mGridRoom.gameObject.SetActive(true);
                break;
            case 1://房间操作
                this.mRoomMain.SetActive(true);
                break;
            case 2://输入房间密码
                this.mRoomMain.SetActive(true);
                this.item_dialog.Show();
                break;
        }
    }
    private void OnUserEneter(NetCmdType type, NetCmdPack pack) {//用户进入
        SC_UserEnter userEnter = pack.ToObj<SC_UserEnter>();
        if (RoleManager.Self.UserID == userEnter.UserInfoHead.UserID) {//直接进入游戏 说明游戏连接成功
            //如果用户是断线重连，则直接进入之前游戏房间，否则则进入下一界面
            if (userEnter.UserInfoHead.TableID == ushort.MaxValue) {//正常进入
                this.SetShowMode(1);
            } else {
                //断线重连操作
            }
        }
    }
    private void OnGameState(NetCmdType type,NetCmdPack pack) {//游戏状态变化，用来监听玩家是否进入房间
        //只要玩家游戏状态发生变化  说明进入房间成功 
        this.Close();
        UI.EnterUI<UI_WZQBattle>(GameEnum.WZQ).InitData();
    }
    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GR_REQUEST_FAILURE://请求错误
                if (this.item_dialog.mInputPwd.Length >= this.item_dialog.mPwdTick.Length) {//请求失败之后，清空密码
                    this.item_dialog.Clear();
                }
                break;
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
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_USER_ENTER, this.OnUserEneter);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_GAME_STATE, this.OnGameState);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_REQUEST_FAILURE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);

        TimeManager.DelayExec(this, UI.AnimTime, () => {
            AudioManager.PlayMusic(GameEnum.All, FishConfig.Instance.AudioConf.datingBgm.ToString());
        });
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_USER_ENTER, this.OnUserEneter);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_GAME_STATE, this.OnGameState);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_REQUEST_FAILURE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_back"://返回大厅
                NetClient.CloseConnect();
                this.Close();
                GameSceneManager.BackToHall(GameEnum.None);
                break;
            case "btn_create"://创建房间
                NetClient.Send(NetCmdType.SUB_GR_USER_CREATE_TABLE, new CMD_GR_UserCreateTableReq());
                break;
            case "btn_join"://加入房间
                this.SetShowMode(2);
                break;
            default:
                return;
        }

        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
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
            case "grid_room":
                this.mGridRoom = tf.GetComponent<UIGrid>();
                break;
            case "item_room_1":
                this.mRooms[0] = this.BindItem<Item_wzqmain_Room>(tf.gameObject);
                break;
            case "item_room_2":
                this.mRooms[1] = this.BindItem<Item_wzqmain_Room>(tf.gameObject);
                break;
            case "item_room_3":
                this.mRooms[2] = this.BindItem<Item_wzqmain_Room>(tf.gameObject);
                break;
            case "item_room_4":
                this.mRooms[3] = this.BindItem<Item_wzqmain_Room>(tf.gameObject);
                break;
            case "item_enter_room":
                this.item_dialog = this.BindItem<Item_wzqmain_Dialog>(tf.gameObject);
                this.item_dialog.Hide();
                break;
            case "room_main":
                this.mRoomMain = tf.gameObject;
                this.mRoomMain.SetActive(false);
                break;
        }
    }
}
