using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_mainhall_new : UILayer {
    public UITexture mTexturePlayer;
    public UILabel mLbName;
    public UISprite mSprVIP;
    public UILabel mLbGameID;
    public UILabel mLbGold;
    public GameObject mBtnSign;//签到
    public GameObject mEffQianDao;//签到特效
    public GameObject mEffShare;//分享特效
    public GameObject mBtnRank;//排行榜
    public UIScrollView mScrollView;
    public GameObject mObjNoFinishTicks;//用户未完善信息提示
    public UIGrid mGridRoom;//
    public UIGrid mGridDown;//按钮

    private bool _pre_sign;
    public List<Item_mainhall_new> mItemList = new List<Item_mainhall_new>();

    public void InitData() {
        this.UpdateUserInfo();
        //this.mBtnSign.SetActive(HallHandle.IsWXLogin);//只有微信登录状态下才能签到
        this.mEffQianDao.SetActive(SignManager.IsSign);
        this._pre_sign = SignManager.IsSign;
        this.mEffShare.SetActive(UserManager.IsShare == false);
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
        this.mObjNoFinishTicks.SetActive(HallHandle.IsPerfect() == false);
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
                foreach (var item in this.mItemList) {
                    item.CheckShow();
                }
                this.RefershPos();
                break;
        }
    }
    private void OnEventHandle(GameEvent event_type, object obj) {
        switch (event_type) {
            case GameEvent.Hall_UserInfoChange://用户信息变更
                this.UpdateUserInfo();
                break;
            case GameEvent.Hall_Share://分享状态
                this.mEffShare.SetActive(UserManager.IsShare == false);
                break;
        }
    }

    public void Update() {
        if (this._pre_sign != SignManager.IsSign) {//检测签到效果显示
            this.mEffQianDao.SetActive(SignManager.IsSign);
            this._pre_sign = SignManager.IsSign;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (UI.GetUI<UI_exitcheck>() == null)
            {
                //UI.EnterUI<UI_exitcheck>(ui=>ui.InitData(UI_exitcheck.FunType.Login));
                UI.EnterUI<UI_exitcheck>(GameEnum.All).InitData(UI_exitcheck.FunType.Login);
            } else {
                UI.ExitUI<UI_exitcheck>();
            }
        }
    }

    private void RefershPos() {//刷新位置
        int count = 0;
        bool is_show_rank = false;
        foreach (var item in this.mItemList) {
            if (item.mIsShow) {
                count++;
                if (item.mGameEnum == GameEnum.WZQ) {
                    is_show_rank = true;
                }
            }
        }
        // this.mBtnRank.SetActive(is_show_rank);
        this.mBtnRank.SetActive(false);
        this.mGridDown.Reposition();
        float w;
        if (count <= 2) {
            w = 700;
        } else if (count <= 3) {
            w = 600;
        } else {
            w = 450;
        }
        this.mGridRoom.cellWidth = w;
        if (w * count > UI.Width) {
            float width = (w * count - UI.Width) * 0.5f;
            width -= this.mScrollView.transform.localPosition.x;
            this.mScrollView.Move(new Vector3(width, 0));
        }
        this.mGridRoom.Reposition();
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_LIST_SERVER, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
        EventManager.RegisterEvent(GameEvent.Hall_Share, this.OnEventHandle);

        this.InitData();
    }
    public override void OnEnter() {
        this.mGridRoom.hideInactive = true;
        this.mGridDown.hideInactive = true;
        this.mScrollView.disableDragIfFits = true;
        // HallHandle.QueryIndividualInfo();
        this.RefershPos();

        MainEntrace.Instance.ShowLoginTick();
        TimeManager.DelayExec(this, UI.AnimTime, () =>
        {
            AudioManager.PlayMusic(GameEnum.All, FishConfig.Instance.AudioConf.datingBgm.ToString());
        });
    }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_PROPERTY_EFFECT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_LIST_SERVER, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
        EventManager.UnRegisterEvent(GameEvent.Hall_Share, this.OnEventHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_shop"://充值
            case "btn_cz"://充值
                //UI.EnterUI<UI_Shop>(null);
                // UI.EnterUI<UI_Shop>(GameEnum.All);
                break;
            case "btn_fx":
                //UI.EnterUI<UI_Share>(ui => ui.InitData(false));
                // UI.EnterUI<UI_Share>(GameEnum.All).InitData(false);
                break;
            case "btn_qd"://签到
                // MainEntrace.Instance.is_show_sign = true;
                //UI.ExitUI<UI_Qiandao>();
                // UI.EnterUI<UI_Qiandao>(GameEnum.All);
                break;
            case "btn_kf"://客服
                //UI.EnterUI<UI_kefu_new>(null);
                // UI.EnterUI<UI_kefu_new>(GameEnum.All);
                string baseUrl = "http://47.111.80.94:82/index/index/build?id=1";
                string roleid = HallHandle.GameID.ToString();
                string visiterName = WWW.EscapeURL(HallHandle.NickName);
                string fullUrl = string.Format("{0}&roleid={1}&visiter_name={2}", baseUrl, roleid, visiterName);
                Application.OpenURL(fullUrl);
                // Debug.Log("link profile and contact support: " + fullUrl);
                break;
            case "btn_phb"://排行榜
                //UI.EnterUI<UI_rank_new>(null);
                UI.EnterUI<UI_rank_new>(GameEnum.All);
                break;
            case "btn_bxx"://保险箱
                //UI.EnterUI<UI_safebox_new>(ui=>ui.InitData());
                UI.EnterUI<UI_safebox_new>(GameEnum.All).InitData();
                break;
            case "btn_sz"://设置
                //UI.EnterUI<UI_setting_new>(null);
                UI.EnterUI<UI_setting_new>(GameEnum.All);
                break;
            case "texture_player"://用户信息
                //UI.EnterUI<UI_userinfo_new>(ui => ui.InitData(1));
                UI.EnterUI<UI_userinfo_new>(GameEnum.All).InitData(1);
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
            case "spr_vip":
                this.mSprVIP = tf.GetComponent<UISprite>();
                break;
            case "lb_game_id":
                this.mLbGameID = tf.GetComponent<UILabel>();
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "btn_qd":
                this.mBtnSign = tf.gameObject;
                break;
            case "scrollview_game":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "eff_qiandao":
                this.mEffQianDao = tf.gameObject;
                this.mEffQianDao.SetActive(false);
                break;
            case "eff_fenxiang":
                this.mEffShare = tf.gameObject;
                break;
            case "info_finish_ticks":
                this.mObjNoFinishTicks = tf.gameObject;
                break;
            case "room_by3d"://3D捕鱼
                {
                    Item_mainhall_new item = this.BindItem<Item_mainhall_new>(tf.gameObject);
                    item.InitData(this, GameEnum.Fish_3D, GameConfig.KindID);
                    this.mItemList.Add(item);
                    break;
                }
            case "room_lk"://李逵劈鱼
                {
                    Item_mainhall_new item = this.BindItem<Item_mainhall_new>(tf.gameObject);
                    item.InitData(this, GameEnum.Fish_LK, LKGameConfig.KindID);
                    this.mItemList.Add(item);
                    break;
                }
            case "room_sh"://神话
                {
                    Item_mainhall_new item = this.BindItem<Item_mainhall_new>(tf.gameObject);
                    item.InitData(this, GameEnum.SH, SHGameConfig.KindID);
                    this.mItemList.Add(item);
                    break;
                }
            case "room_fqzs"://飞禽走兽
                {
                    Item_mainhall_new item = this.BindItem<Item_mainhall_new>(tf.gameObject);
                    item.InitData(this, GameEnum.FQZS, FQZSGameConfig.KindID);
                    this.mItemList.Add(item);
                    break;
                }
            case "room_wzq"://五子棋
                {
                    Item_mainhall_new item = this.BindItem<Item_mainhall_new>(tf.gameObject);
                    item.InitData(this, GameEnum.WZQ, WZQGameConfig.KindID);
                    this.mItemList.Add(item);
                    break;
                }
            case "grid_gamse":
                this.mGridRoom = tf.GetComponent<UIGrid>();
                break;
            case "down_grid":
                this.mGridDown = tf.GetComponent<UIGrid>();
                break;
            case "btn_phb":
                this.mBtnRank = tf.gameObject;
                break;
        }
     }
}
