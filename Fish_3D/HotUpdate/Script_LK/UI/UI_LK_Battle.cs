using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_Battle : UILayer {
    public static UI_LK_Battle ui;

    public GameObject mItemBattle;
    public GameObject mBtnAutoLock;//自动锁定
    public GameObject mBtnAutoFire;//自动射击
    //public UILabel mLbAutoLock;
    //public UILabel mLbAutoFire;
    public GameObject mEffAutoLock;
    public GameObject mEffAutoFire;
    public UIPanel mEffPanel;
    public UISprite mSprTop;//顶层节点
    public GameObject mItemMul;//特殊鱼倍率显示

    public float exit_time = 60;//退出倒计时
    public UILabel mLbExitDownCount;//退出倒计时
    public GameObject mMenu;
    public GameObject mOpenArrow;
    public GameObject mCloseArrow;

    public Item_LK_Battle[] mItems = new Item_LK_Battle[LKGameConfig.MAXSEAT];
    public void InitData() {
        for (int i = 0; i < LKGameConfig.MAXSEAT; i++) {
            this.mItems[i] = this.AddItem<Item_LK_Battle>(this.mItemBattle, this.transform);
            this.mItems[i].InitData(this, (ushort)i, LKRoleManager.mRoles[i]);
        }
        this.SetAutoFire(false);//取消自动射击
        this.SetAutoLock(false);//取消自动锁定
    }
    public void ResetData() {
        for (int i = 0; i < LKGameConfig.MAXSEAT; i++) {
            this.mItems[i].SetRole(LKRoleManager.mRoles[i]);
        }
        this.SetAutoFire(false);//取消自动射击
        this.SetAutoLock(false);//取消自动锁定
    }
    public void RefershExitTime() {
        this.exit_time = 60;//3分钟不操作踢出
        if (this.mLbExitDownCount != null) {
            GameObject.Destroy(this.mLbExitDownCount.transform.parent.gameObject);
            this.mLbExitDownCount = null;
        }
    }
    public void SetAutoFire(bool is_auto) {
        LKGameConfig.IsAutoFire = is_auto;
        this.mEffAutoFire.SetActive(LKGameConfig.IsAutoFire);
        //if (LKGameConfig.IsAutoFire) {
        //    this.mLbAutoFire.text = "取消射击";
        //} else {
        //    this.mLbAutoFire.text = "自动射击";
        //}
    }
    public void SetAutoLock(bool is_auto) {
        LKGameConfig.IsAutoLock = is_auto;
        this.mEffAutoLock.SetActive(LKGameConfig.IsAutoLock);
        //if (LKGameConfig.IsAutoLock) {
        //    this.mLbAutoLock.text = "取消锁定";
        //} else {
        //    this.mLbAutoLock.text = "自动锁定";
        //}
        if (LKGameConfig.IsAutoLock == false) {
            this.mItems[RoleManager.Self.ChairSeat].SetLockFish(null);
        }
    }
    public Item_LK_Battle_Mul BindFishMul(LKFish fish) {
        Item_LK_Battle_Mul item = this.AddItem<Item_LK_Battle_Mul>(this.mItemMul, this.mItemMul.transform.parent);
        item.InitData(fish);
        return item;
    }
    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_S_CLIENT_CFG_LKPY://客户端切换炮台操作
                this.mItems[pack.ToObj<CMD_S_ClientCfg_lkpy>().chair_id].UpdateLauncherInfo();
                break;
            case NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY://鱼币金币兑换
                this.mItems[pack.ToObj<CMD_S_ExchangeFishScore_lkpy>().chair_id].RefershGoldInfo();
                break;
            case NetCmdType.SUB_S_USER_FIRE_LKPY://玩家发炮
                this.mItems[pack.ToObj<CMD_S_UserFire_lkpy>().chair_id].OnFire(pack.ToObj<CMD_S_UserFire_lkpy>());
                break;
            case NetCmdType.SUB_S_BULLET_ION_TIMEOUT_LKPY://能量炮结束
                {
                    CMD_S_BulletIonTimeout_lkpy cmd = pack.ToObj<CMD_S_BulletIonTimeout_lkpy>();
                    this.mItems[cmd.chair_id].SetNLP(0);
                }
                break;
            case NetCmdType.SUB_S_CATCH_FISH_LKPY://捕获鱼类
                {
                    //处理能量炮逻辑
                    CMD_S_CatchFish_lkpy cmd = pack.ToObj<CMD_S_CatchFish_lkpy>();
                    if (cmd.bullet_ion > 0) {
                        this.mItems[cmd.chair_id].SetMul(cmd.bullet_mulriple);
                        this.mItems[cmd.chair_id].SetNLP(cmd.bullet_ion);
                    }
                }
                break;
        }
    }
    private void GameEventHandle(GameEvent type, object obj) {
        switch (type) {
            case GameEvent.UserLeaveTable://用户离开桌子
                LKRoleManager.OnUserLevelTable(obj as RoleInfo);
                if (LKGameManager.mIsChangeTable == false && obj == RoleManager.Self) {
                    this.Exit();
                } else {
                    RoleInfo role = obj as RoleInfo;
                    if (role.TableID == RoleManager.Self.TableID && role.ChairSeat < LKGameConfig.MAXSEAT) {
                        this.mItems[role.ChairSeat].SetRole(null);
                    }
                }
                break;
            case GameEvent.UserEnterTable://用户进入桌子
                {
                    RoleInfo role = obj as RoleInfo;
                    LKRoleManager.OnUserEnterTable(role);
                    if (role.TableID == RoleManager.Self.TableID && role.ChairSeat < LKGameConfig.MAXSEAT) {
                        this.mItems[role.ChairSeat].SetRole(LKRoleManager.mRoles[role.ChairSeat]);
                    }
                }
                break;
            case GameEvent.UserInfoChange://用户信息变化
                {
                    RoleInfo role = obj as RoleInfo;
                    if (role.ChairSeat < LKGameConfig.MAXSEAT) {
                        this.mItems[role.ChairSeat].RefershGoldInfo();
                    }
                }
                break;
        }
    }
    public void Exit() {
        this.Close();
        LKFishManager.ClearAllFish();
        LKBulletManager.ClearAllBullet();
        NetClient.CloseConnect();
        GameSceneManager.BackToHall(GameEnum.Fish_LK);
        //UI.EnterUI<UI_SHchooseroom>((ui) => {
        //    ui.InitData(HallHandle.GetServerList(SHGameConfig.KindID));
        //});
    }

    public void Update() {
        if (this.exit_time > 0 && NetClient.IsConnected) {
            if (this.exit_time > Time.deltaTime) {
                this.exit_time -= Time.deltaTime;
                if (this.exit_time < 11) {
                    if (this.mLbExitDownCount == null) {
                        GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_LeaveTip, this.transform);
                        obj.transform.localPosition = Vector2.zero;
                        obj.transform.localScale = Vector3.one;
                        this.mLbExitDownCount = obj.transform.Find("lb_downtime").GetComponent<UILabel>();
                    }
                    this.mLbExitDownCount.text = ((int)this.exit_time).ToString();
                }
            } else {
                this.exit_time = 0;//时间到了直接起立
                NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp {
                    ForceLeave = 1,
                    TableID = RoleManager.Self.TableID,
                    ChairID = RoleManager.Self.ChairSeat,
                });
            }
        }
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.Escape)) {
//            UI_LK_Setting ui = UI.GetUI<UI_LK_Setting>();
//            if (ui == null) {
//                UI.EnterUI<UI_LK_Setting>(GameEnum.Fish_LK).InitData();
//            } else {
//                ui.Close();
//            }
//        }
//#endif
    }

    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CLIENT_CFG_LKPY, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_USER_FIRE_LKPY, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_BULLET_ION_TIMEOUT_LKPY, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CATCH_FISH_LKPY, this.OnNetHandle);

        EventManager.RegisterEvent(GameEvent.UserLeaveTable, this.GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserEnterTable, this.GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserInfoChange, this.GameEventHandle);

        UI_LK_Battle.ui = this;
    }
    public override void OnEnter() {
        //首次进入 显示帮助界面
        if (PlayerPrefs.GetInt("first_enter_lk", 0) == 0) {
            UI.EnterUI<UI_LK_Help>(GameEnum.Fish_LK);
            PlayerPrefs.SetInt("first_enter_lk", 1);
            PlayerPrefs.Save();
        }
    }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CLIENT_CFG_LKPY, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_USER_FIRE_LKPY, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_BULLET_ION_TIMEOUT_LKPY, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CATCH_FISH_LKPY, this.OnNetHandle);

        EventManager.UnRegisterEvent(GameEvent.UserLeaveTable, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserEnterTable, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserInfoChange, this.GameEventHandle);

        UI_LK_Battle.ui = null;

        LKGoldEffManager.RemoveUserList();
        TimeManager.ClearAllCall();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_setting"://菜单
                UI.EnterUI<UI_LK_Setting>(GameEnum.Fish_LK).InitData();
                //Debug.LogError("暂时退出");
                //NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp {
                //    ForceLeave = 1,
                //    TableID = RoleManager.Self.TableID,
                //    ChairID = RoleManager.Self.ChairSeat,
                //});
                break;
            case "btn_menu":
                mMenu.SetActive(!mMenu.activeSelf);
                mOpenArrow.SetActive(!mMenu.activeSelf);
                mCloseArrow.SetActive(mMenu.activeSelf);
                break;
            case "btn_auto_lock"://自动锁定
                this.SetAutoLock(LKGameConfig.IsAutoLock == false);
                break;
            case "btn_auto_fire"://自动射击
                this.SetAutoFire(LKGameConfig.IsAutoFire == false);
                break;
            case "btn_help"://帮助界面
                UI.EnterUI<UI_LK_Help>(GameEnum.Fish_LK);
                break;
            case "btn_safebox"://保险箱
                UI.EnterUI<UI_LK_Bank>(GameEnum.Fish_LK).InitData();
                break;
            case "btn_exit"://离开房间
                UI.EnterUI<UI_LK_Quit>(GameEnum.Fish_LK);
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "btn_auto_lock":
                this.mBtnAutoLock = tf.gameObject;
                break;
            case "btn_auto_fire":
                this.mBtnAutoFire = tf.gameObject;
                break;
            case "eff_auto_fire":
                this.mEffAutoFire = tf.gameObject;
                break;
            case "eff_auto_lock":
                this.mEffAutoLock = tf.gameObject;
                break;
            //case "lb_auto_lock":
            //    this.mLbAutoLock = tf.GetComponent<UILabel>();
            //    break;
            //case "lb_auto_fire":
            //    this.mLbAutoFire = tf.GetComponent<UILabel>();
            //    break;
            case "item_role":
                this.mItemBattle = tf.gameObject;
                this.mItemBattle.SetActive(false);
                break;
            case "eff_panel":
                this.mEffPanel = tf.GetComponent<UIPanel>();
                break;
            case "spr_top":
                this.mSprTop = tf.GetComponent<UISprite>();
                break;
            case "item_mul":
                this.mItemMul = tf.gameObject;
                this.mItemMul.SetActive(false);
                break;
            case "spr_right":
                tf.localPosition = new Vector3(960 * Resolution.ViewAdaptAspect, 0);
                break;
            case "spr_left":
                tf.localPosition = new Vector3(-960 * Resolution.ViewAdaptAspect, 0);
                break;
            case "menu":
                mMenu = tf.gameObject;
                break;
            case "open_arrow":
                mOpenArrow = tf.gameObject;
                break;
            case "close_arrow":
                mCloseArrow = tf.gameObject;
                break;
        }
    }
}
