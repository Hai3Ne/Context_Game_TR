using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保险箱
/// </summary>
public class UI_safebox_new : UILayer {
    public GameObject mObjLogin;//登录相关
    public UIInput mInputPassworld;
    public GameObject mObjSelect;//自动注销标识
    public UILabel mLbPwdTick;

    public GameObject mObjBankInfo;//银行信息
    public UILabel mLbGold;
    public UILabel mLbBank;
    public UIInput mInputFunc;

    private MsgNotificator notify;
    private string mPwd;

    public void InitData() {
        this.RefershUserInfo();
        this.SetMode(0);
        this.SetAutoLogin(GameConfig.OP_AutoLoginBank, false);
        if (GameConfig.OP_AutoLoginBank && string.IsNullOrEmpty(GameConfig.InsurePassword) == false) {
            this.Login(GameConfig.InsurePassword);
        }
	    if(HallHandle.IsPerfect()){//用户信息是否完善
		    this.mLbPwdTick.text = StringTable.GetString("Tip_41");
	    }else{
		    this.mLbPwdTick.text = StringTable.GetString("Tip_40");
	    }
    }
    public long GetUserGold() {
        if (UI_LK_Battle.ui != null) {
            return RoleManager.Self.GoldNum;//LKRoleManager.GetBaseGold(RoleManager.Self.ChairSeat);
        }else if (SceneLogic.Instance.IsGameOver == false) {
            byte clientSeat = SceneLogic.Instance.FModel.GetTableRolerSeat(RoleInfoModel.Instance.Self.UserID);
            return SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(clientSeat);
        } else {
            return HallHandle.UserGold;
        }
    }
    public long GetFuncMaxGold() {
        if (this.GetUserGold() > HallHandle.UserInsure) {
            return this.GetUserGold();
        } else {
            return HallHandle.UserInsure;
        }
    }
    public void RefershUserInfo() {
        this.mLbGold.text = this.GetUserGold().ToString();
        this.mLbBank.text = HallHandle.UserInsure.ToString();
    }
    public void SetMode(int type) {//0:登录  1:保险箱
        this.mObjLogin.SetActive(type == 0);
        this.mObjBankInfo.SetActive(type == 1);
    }
    public void SetAutoLogin(bool is_auto, bool is_save) {
        this.mObjSelect.SetActive(is_auto == false);
        if (is_save) {
            GameConfig.OP_AutoLoginBank = is_auto;
        }
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GP_USER_INSURE_INFO://保险箱资料
            case NetCmdType.SUB_GR_USER_INSURE_INFO://保险箱资料
                if (string.IsNullOrEmpty(this.mPwd) == false) {
                    GameConfig.InsurePassword = this.mPwd;
                }
                this.SetMode(1);
                break;
            case NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY://鱼币兑换
                this.RefershUserInfo();
                break;

            case NetCmdType.SUB_GP_USER_INSURE_SUCCESS://取钱成功//CMD_GP_UserInsureSuccess
            case NetCmdType.SUB_GR_USER_INSURE_SUCCESS://取钱成功//CMD_GR_S_UserInsureSuccess
                this.RefershUserInfo();
                break;
        }
    }
    private void GameEventHandle(GameEvent type, object obj) {
        switch (type) {
            case GameEvent.Hall_UserInfoChange://用户信息变更
                this.RefershUserInfo();
                break;
        }
    }
    private void OnPlayGoldNumChange(object o) {
        if (SceneLogic.Instance.IsGameOver == false) {
            byte clientSeat = SceneLogic.Instance.FModel.GetTableRolerSeat(RoleInfoModel.Instance.Self.UserID);
            this.mLbGold.text = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(clientSeat).ToString();
        }
    }
    private void onFuncGoldChange() {//金额操作
        long gold;
        if (long.TryParse(this.mInputFunc.value, out gold)) {
            long max_gold = this.GetFuncMaxGold();
            if (gold > max_gold) {
                this.mInputFunc.value = max_gold.ToString();
            }
        }
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_INSURE_INFO, this.OnNetHandle);//保险箱资料
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_USER_INSURE_INFO, this.OnNetHandle);//保险箱资料
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_INSURE_SUCCESS, this.OnNetHandle);//取钱成功
        NetEventManager.RegisterEvent(NetCmdType.SUB_GR_USER_INSURE_SUCCESS, this.OnNetHandle);//取钱成功
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY, this.OnNetHandle);//鱼币金币兑换

        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, this.GameEventHandle);

        RoleItemModel.Instance.RegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        if (SceneLogic.Instance.FModel != null) {
            this.notify = SceneLogic.Instance.FModel.Notify;
        }
        if (this.notify != null) {
            this.notify.RegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        }
    }
    public override void OnEnter() {
        EventDelegate.Add(this.mInputFunc.onChange, onFuncGoldChange);
    }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_INSURE_INFO, this.OnNetHandle);//保险箱资料
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_USER_INSURE_INFO, this.OnNetHandle);//保险箱资料
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_INSURE_SUCCESS, this.OnNetHandle);//取钱成功
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GR_USER_INSURE_SUCCESS, this.OnNetHandle);//取钱成功
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY, this.OnNetHandle);//鱼币金币兑换

        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, this.GameEventHandle);

        RoleInfoModel.Instance.UnRegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        if (this.notify != null) {
            this.notify.UnRegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
        }
    }
    public void Login(string pwd) {
        this.mPwd = pwd;
        FishNetAPI.Instance.SendMainQueryInsureInfo(this.mPwd);
    }
    public void SaveGold(long gold) {//存钱
        if (gold < 2000) {
            SystemMessageMgr.Instance.ShowMessageBox("单次存款金额不能少于2000", 1);
        } else {
            FishNetAPI.Instance.SendMainSaveInsure(HallHandle.LogonCode, GameConfig.InsurePassword, gold);
        }
    }
    public void GetGold(long gold) {//取钱
        if (gold < 2000) {
            SystemMessageMgr.Instance.ShowMessageBox("单次取出金额不能少于2000", 1);
        } else {
            FishNetAPI.Instance.SendMainTakeInsure(HallHandle.LogonCode, GameConfig.InsurePassword, gold);
        }
    }
    public void SetFuncGold(long gold) {//设置金额
        long max_gold = this.GetFuncMaxGold();
        if (gold > max_gold) {
            gold = max_gold;
        }
        this.mInputFunc.value = gold.ToString();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "bg":
            case "btn_close":
                this.Close();
                break;
            case "btn_login_op":
                this.SetAutoLogin(GameConfig.OP_AutoLoginBank == false, true);
                break;
            case "btn_login":
                this.Login(this.mInputPassworld.value);
                break;
            case "btn_all_save"://全存
                this.SaveGold(this.GetUserGold());
                break;
            case "btn_all_get"://全取
                this.GetGold(HallHandle.UserInsure);
                break;
            case "btn_10w"://
                this.SetFuncGold(100000);
                break;
            case "btn_100w":
                this.SetFuncGold(1000000);
                break;
            case "btn_1000w":
                this.SetFuncGold(10000000);
                break;
            case "btn_1e":
                this.SetFuncGold(100000000);
                break;
            case "btn_get": {//取款
                    long val;
                    if (long.TryParse(this.mInputFunc.value, out val)) {
                        this.GetGold(val);
                    }
                }
                break;
            case "btn_save": {//存款
                    long val;
                    if (long.TryParse(this.mInputFunc.value, out val)) {
                        this.SaveGold(val);
                    }
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "login_info":
                this.mObjLogin = tf.gameObject;
                break;
            case "input_pwd":
                this.mInputPassworld = tf.GetComponent<UIInput>();
                break;
            case "spr_select":
                this.mObjSelect = tf.gameObject;
                break;
            case "bank_info":
                this.mObjBankInfo = tf.gameObject;
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "lb_bank":
                this.mLbBank = tf.GetComponent<UILabel>();
                break;
            case "input_func":
                this.mInputFunc = tf.GetComponent<UIInput>();
                break;
            case "lb_pwd_tips":
                this.mLbPwdTick = tf.GetComponent<UILabel>();
                break;
        }
    }
}
