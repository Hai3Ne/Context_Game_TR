using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_3D_Main : UILayer {
    public UITexture mTexturePlayer;
    public UILabel mLbName;
    public UILabel mLbGameID;
    public UISprite mSprVIP;
    public UILabel mLbGold;
    public GameObject mObjJingDian;//经典模式
    public GameObject mObjJiFen;//积分模式
    public GameObject mModelList;//模式列表
    public UIScrollView mScrollRoom;
    public UIGrid mGridRoom;
    public GameObject mBtnSC;//首充
    public GameObject mBtnQuickGo;//快速开始

    public Item_3D_Main item_jd_2000;
    public Item_3D_Main item_jd_20000;
    public Item_3D_Main item_jf_daoju;
    public Item_3D_Main item_jf_jingdian;
    public Item_3D_Main[] item_daoju = new Item_3D_Main[4];//道具房间

    public int mCurMode;//10.正常模式  12.经典模式  13.PK模式  0.积分模式

    public void InitData()
    {
        List<tagGameServer> server = HallHandle.GetServerList(1, 0);//0.积分模式   1.正常模式
        List<TimeRoomVo> room_list = new List<TimeRoomVo>(FishConfig.Instance.TimeRoomConf.Values);
        tagGameServer gs;
        uint min_mul;
        for (int i = 0; i < item_daoju.Length; i++) {
            if(server.Count > i){
                gs = server[i];
            }else{
                gs = null;
            }
            if(i > 0){
                min_mul = room_list[i-1].RoomMultiple;
            }else{
                min_mul = 1;
            }
            item_daoju[i].InitData(gs, min_mul, room_list[i].RoomMultiple);
        }

        this.item_jd_2000.InitData(HallHandle.GetServerBySortID(121), 908);
        this.item_jd_20000.InitData(HallHandle.GetServerBySortID(122), 905);
        this.item_jf_daoju.InitData(HallHandle.GetServerBySortID(01), 906);
        this.item_jf_jingdian.InitData(HallHandle.GetServerBySortID(21), 907);

        this.SetMode(-1);
        this.UpdateUserInfo();
        this.RefershShouChong();
    }
    public void RefershShouChong() {//刷新首充状态
        this.mBtnSC.SetActive(ShopManager.mIsShowFrist);
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
    public void SetMode(int mode) {////10.正常模式  12.经典模式  13.PK模式  0.积分模式
        this.mCurMode = mode;

        this.mModelList.SetActive(false);
        this.mObjJiFen.SetActive(false);
        this.mObjJingDian.SetActive(false);
        this.mScrollRoom.gameObject.SetActive(false);
        this.mBtnQuickGo.SetActive(false);
        switch (this.mCurMode) {
            case 10://正常模式
                this.mScrollRoom.gameObject.SetActive(true);
                this.mBtnQuickGo.SetActive(true);
                break;
            case 12://经典模式
                this.mObjJingDian.SetActive(true);
                this.mBtnQuickGo.SetActive(true);
                break;
            case 13://PK模式
                this.mBtnQuickGo.SetActive(true);
                break;
            case 0://积分模式
                this.mObjJiFen.SetActive(true);
                this.mBtnQuickGo.SetActive(true);
                break;
            case -1:
            default:
                this.mModelList.SetActive(true);
                break;
        }
    }
    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GR_PROPERTY_EFFECT://VIP通知
                this.UpdateUserInfo();
                break;
            case NetCmdType.SUB_GP_USER_FACE_INFO://更改头像
                this.SetFace(HallHandle.FaceID);
                break;
            case NetCmdType.SUB_GP_LIST_SERVER://服务器列表
                this.InitData();
                break;
        }
    }
    private void OnEventHandle(GameEvent event_type, object obj) {
        switch (event_type) {
            case GameEvent.Hall_UserInfoChange://用户信息变更
                this.UpdateUserInfo();
                break;
            case GameEvent.Hall_SCUpdate://首充状态更新
                this.RefershShouChong();
                break;
        }
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);
        
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_LIST_SERVER, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
        EventManager.RegisterEvent(GameEvent.Hall_SCUpdate, this.OnEventHandle);
        this.InitData();
    }
    public override void OnEnter() {
        this.mScrollRoom.ResetPosition();
        this.UpdateUserInfo();
        TimeManager.DelayExec(this, UI.AnimTime, () => {
            if (MainEntrace.Instance.is_show_frist_pay == false && ShopManager.mIsShowFrist == true) {
                MainEntrace.Instance.ShowSCTick();
            }
        });
        TimeManager.DelayExec(this, 1, () => {
            GlobalAudioMgr.Instance.PlayBGMusic(FishConfig.Instance.AudioConf.MainBgm);
        });
    }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
        EventManager.UnRegisterEvent(GameEvent.Hall_SCUpdate, this.OnEventHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "Btn_DJF"://道具房
                this.SetMode(10);
                break;
            case "Btn_JDF"://经典房
                this.SetMode(12);
                break;
            case "Btn_TYF"://体验房
                this.SetMode(0);
                break;
            case "btn_buy"://商城界面
                //UI.EnterUI<UI_Shop>(null);
                UI.EnterUI<UI_Shop>(GameEnum.All);
                break;
            case "btn_quickgo"://快速开始
                MainEntrace.Instance.QuickStart(this.mCurMode, HallHandle.IsWXLogin, HallHandle.UserID, HallHandle.Accounts, HallHandle.LoginPassword, HallHandle.LogonCode);
                break;
            case "btn_shouchong"://首充
                if (UserManager.IsBingWX) {
                    MainEntrace.Instance.ShowSCTick();
                } else {
                    //UI.EnterUI<UI_BindNotice>(ui => ui.InitData(false));
                    UI.EnterUI<UI_BindNotice>(GameEnum.All).InitData(false);
                }
                break;
            case "btn_back":
                if (this.mCurMode >= 0) {
                    this.SetMode(-1);
                } else {
                    this.Close();
                    GameSceneManager.BackToHall(GameEnum.None);
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
            case "lb_game_id":
                this.mLbGameID = tf.GetComponent<UILabel>();
                break;
            case "spr_vip":
                this.mSprVIP = tf.GetComponent<UISprite>();
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "mode_jingdian":
                this.mObjJingDian = tf.gameObject;
                break;
            case "mode_jifen":
                this.mObjJiFen = tf.gameObject;
                break;
            case "model_list":
                this.mModelList = tf.gameObject;
                break;
            case "scrollview_room":
                this.mScrollRoom = tf.GetComponent<UIScrollView>();
                break;
            case "room_grid":
                this.mGridRoom = tf.GetComponent<UIGrid>();
                break;
            case "btn_shouchong":
                this.mBtnSC = tf.gameObject;
                break;
            case "btn_quickgo":
                this.mBtnQuickGo = tf.gameObject;
                break;
            case "Btn_JJC":
                GameUtils.SetGray(tf.gameObject, true);
                break;
            case "jqqd":
                tf.GetComponent<UISprite>().IsGray = false;
                break;

            case "room_jingdian_2000":
                this.item_jd_2000 = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "room_jingdian_20000":
                this.item_jd_20000 = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "room_jifen_daoju":
                this.item_jf_daoju = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "room_jifen_jingdian":
                this.item_jf_jingdian = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "Icon_XSF":
                this.item_daoju[0] = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "Icon_CJF":
                this.item_daoju[1] = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "Icon_ZJF":
                this.item_daoju[2] = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
            case "Icon_GJF":
                this.item_daoju[3] = this.BindItem<Item_3D_Main>(tf.gameObject);
                break;
        }
    }
}
