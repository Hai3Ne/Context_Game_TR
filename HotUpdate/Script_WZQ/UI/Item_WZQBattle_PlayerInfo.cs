using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_WZQBattle_PlayerInfo : UIItem {
    public UISprite mSprNono;//没人的时候显示头像信息
    public UITexture mTexturePlayer;//人物头像
    public UISprite mSprVIP;//会员标识
    public UILabel mLbGold;//
    public UILabel mLbName;
    public UILabel mLbID;//游戏ID
    public UISprite mSprSeat;//黑白位置标识
    public UISprite mSprDownCount;//倒计时标记
    public UILabel mLbDownCount;
    public UILabel mLbWait;//等待中提示
    public GameObject mBtnTiRen;//踢人
    public UISprite mSprReady;//准备标记

    public byte mColor;//1.黑子  2.白字
    public byte mSeat;//当前位置
    public RoleInfo mRoleInfo;//当前用户信息
    public float mDownCount;//当前游戏倒计时
    public bool mShowTiRen = false;//是否显示踢人按钮

    public void InitData(RoleInfo info,byte color) {
        this.SetColor(color);
        this.SetRole(info);
    }
    public void SetColor(byte color) {
        this.mColor = color;
        if (color == 1) {//黑棋
            this.mSprSeat.spriteName = "heiqi-min";
        } else {//if(color == 2){//白旗
            this.mSprSeat.spriteName = "baise-min";
        }
    }
    public void SetGold(long gold) {
        this.mLbGold.text = gold.ToString();
    }
    public void SetRole(RoleInfo info) {
        this.mRoleInfo = info;
        if (this.mRoleInfo == null) {//没有玩家
            this.mShowTiRen = false;//玩家变更的时候  踢人按钮隐藏
            this.mSprNono.gameObject.SetActive(true);
            this.mTexturePlayer.gameObject.SetActive(false);
            this.mSprVIP.gameObject.SetActive(false);
            this.mLbGold.gameObject.SetActive(false);
            this.mLbName.gameObject.SetActive(false);
            this.mLbID.text = string.Empty;
            this.SetShowDownCount(false);
            this.mLbWait.gameObject.SetActive(true);
            this.mBtnTiRen.SetActive(false);
            this.mSprReady.gameObject.SetActive(false);
        } else {
            this.mSprNono.gameObject.SetActive(false);
            this.mTexturePlayer.gameObject.SetActive(true);
            this.mSprVIP.gameObject.SetActive(true);
            this.mLbGold.gameObject.SetActive(true);
            this.mLbName.gameObject.SetActive(true);
            this.SetShowDownCount(true);
            this.mLbWait.gameObject.SetActive(false);
            //踢人逻辑需要特殊处理
            this.RefershTiRenShow();
            this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(this.mRoleInfo.FaceID);
            this.mSprVIP.spriteName = string.Format("vip_{0}",this.mRoleInfo.MemberOrder);
            this.mSprVIP.MakePixelPerfect();
            this.SetGold(this.mRoleInfo.GoldNum);
            this.mLbName.text = this.mRoleInfo.NickName;
            this.mLbID.text = string.Format("ID:{0}", this.mRoleInfo.GameID);
            this.SetShowDownCount(false);

            if (GameManager.CurGameState == GameState.GAME_STATUS_PLAY) {//游戏状态下获取用户剩余时间
                this.StartGame();
                this.mSprReady.gameObject.SetActive(false);
            } else {
                this.StopGame();
                if (this.mRoleInfo.UserStatus == EnumUserStats.US_READY) {
                    this.mSprReady.gameObject.SetActive(true);
                } else {
                    this.mSprReady.gameObject.SetActive(false);
                }
            }
        }
    }
    public void SetSeat(byte seat) {
        this.mSeat = seat;
    }
    private int _pre_downcount = int.MinValue;
    public void SetDownCount(int time) {
        if (_pre_downcount == time) {
            return;
        }
        this._pre_downcount = time;
        this.mLbDownCount.text = time.ToString();
        if (this.mDownCount < 60) {//最后一分钟进行红色
            if (this.mSprDownCount.spriteName != "timered-min") {
                this.mSprDownCount.spriteName = "timered-min";
                this.mLbDownCount.color = new Color32(255, 0, 0, 255);
            }
        } else {
            if (this.mSprDownCount.spriteName == "timegreen-min") {
                this.mSprDownCount.spriteName = "timegreen-min";
                this.mLbDownCount.color = new Color32(0, 254, 66, 255);
            }
        }
        if (time <= 10) {
            AudioManager.PlayAudio(GameEnum.WZQ,WZQGameConfig.Audio_ClickBtn);
        }
    }
    public void ResetDownCount(float time) {//重设倒计时
        this.mDownCount = time;
        if (this.mDownCount > 0) {
            this._pre_downcount = int.MinValue;
            this.SetShowDownCount(true);
            this.SetDownCount((int)this.mDownCount);
        } else {
            this.SetShowDownCount(false);
        }
    }
    public void StartGame() {//游戏开始
        this.mBtnTiRen.gameObject.SetActive(false);
    }
    public void StopGame() {//游戏结束
        this.SetShowDownCount(false);
        this.mBtnTiRen.gameObject.SetActive(this.mShowTiRen);
    }
    public void SetShowDownCount(bool is_show) {
        this.mSprDownCount.gameObject.SetActive(is_show);
    }

    public void Update() {
        if (this.mRoleInfo == null) {
            return;
        }
        if (WZQTableManager.mCurRoleSeat == this.mRoleInfo.ChairSeat) {//当前正在执子
            if(this.mDownCount > 0){
                this.mDownCount -= Time.deltaTime;
                this.SetDownCount((int)this.mDownCount);
            }
        }
    }
    private void OnCurRoleSeatChange(GameEvent type, object obj) {//该谁下子通知
        if (this.mSeat == System.Convert.ToUInt16(obj)) {
            this.ResetDownCount(WZQTableManager.LeftTime[this.mSeat]);
        } else {
            this.SetShowDownCount(false);
        }
    }
    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_S_KICK_FLAG://踢人按钮点亮
                if (this.mRoleInfo != RoleManager.Self) {
                    this.mShowTiRen = true;
                } else {
                    this.mShowTiRen = false;
                }
                this.RefershTiRenShow();
                break;
        }
    }
    public void Awake() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_KICK_FLAG, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.CurPlaySeatChange, this.OnCurRoleSeatChange);
    }
    public void OnDestroy() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_KICK_FLAG, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.CurPlaySeatChange, this.OnCurRoleSeatChange);
    }

    public void RefershTiRenShow() {//刷新踢人按钮显示
        this.mBtnTiRen.SetActive(this.mShowTiRen);
    }

    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "item_btn_tiren"://踢人
                NetClient.Send(NetCmdType.SUB_C_KICK_USER, new CMD_C_KICK_USER());
                break;
            default:
                return;
        }

        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_none":
                this.mSprNono = tf.GetComponent<UISprite>();
                break;
            case "item_img_player":
                this.mTexturePlayer = tf.GetComponent<UITexture>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "item_lb_id":
                this.mLbID = tf.GetComponent<UILabel>();
                break;
            case "item_spr_seat":
                this.mSprSeat = tf.GetComponent<UISprite>();
                break;
            case "item_spr_downcount":
                this.mSprDownCount = tf.GetComponent<UISprite>();
                break;
            case "item_lb_downcount":
                this.mLbDownCount = tf.GetComponent<UILabel>();
                break;
            case "item_lb_waiting":
                this.mLbWait = tf.GetComponent<UILabel>();
                break;
            case "item_btn_tiren":
                this.mBtnTiRen = tf.gameObject;
                break;
            case "item_spr_ready":
                this.mSprReady = tf.GetComponent<UISprite>();
                break;
            case "item_spr_vip":
                this.mSprVIP = tf.GetComponent<UISprite>();
                break;
        }
    }
}
