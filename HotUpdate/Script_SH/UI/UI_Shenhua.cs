using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Shenhua : UILayer {
    public Item_ShenHua_LeiZhu item_leizhu;
    public UILabel mLbName;//玩家昵称
    public UILabel mLbGold;//玩家金币
    public UILabel mLbResult;//玩家成绩
    public Item_ShenHua_History item_history;
    public Transform mTfMenus;
    public GameObject mBtnShowMenu;
    public GameObject mBtnHideMenu;
    public Item_ShenHua_Message item_message;
    public Item_ShenHua_BetList item_bet_list;
    public Item_ShenHua_ZhuanPan item_zhuanpan;

    private float EveryflyCoinTime = 0.3f;
    public GameObject mMenu;
    public GameObject mOpenArrow;
    public GameObject mCloseArrow;
    public UISlider mSliderMusic;
    public UISlider mSliderSound;

    /// <summary>
    /// 筹码飞行控制器
    /// </summary>
    public Item_ShenHua_Coin_Fly_Ctrl mCoinFlyCtrl;

    public void InitData(RoleInfo leizhu, List<RoleInfo> leizhu_list, long leizhu_result,int leizhu_count, uint chip_infor,SHEnumGameState state,float downcount,long[] all_gold,long[] self_gold) {
        this.gameObject.SetActive(true);
        UI.ExitOtherUI(this);
        this.item_leizhu.InitData(leizhu, leizhu_list, leizhu_result, leizhu_count);
        this.SetShowMenu(false);
        this.item_bet_list.InitData(SHGameConfig.GetChip(chip_infor));
        this.item_zhuanpan.InitData(this, state, downcount, all_gold, self_gold);
        this.item_bet_list.SetEnable(state == SHEnumGameState.Bet);
        this.RefershSelfInfo();
        item_zhuanpan.ClearFlyChouMa = mCoinFlyCtrl.ClearCoin;
        item_zhuanpan.mLbDownCount.text = string.Empty;
    }

    public void SetShowMenu(bool is_show) {
        //this.mBtnShowMenu.SetActive(is_show == false);
        //this.mBtnHideMenu.SetActive(is_show == true);
        //if (is_show) {
        //    TweenPosition.Begin(this.mTfMenus.gameObject, 0.25f, new Vector3(152, -433));
        //} else {
        //    TweenPosition.Begin(this.mTfMenus.gameObject, 0.25f, new Vector3(152, -560));
        //}
    }

    public void RefershSelfInfo() {//刷新自己信息
        this.mLbName.text = RoleManager.Self.NickName;
        if (this.item_zhuanpan.mGameState == SHEnumGameState.Result)
        {
            //结算的时候直接显示用户自身金币数量
            this.mLbGold.text = Tools.longToStr(RoleManager.Self.GoldNum, 3);
            item_leizhu.RefrshSelfGoldInScorllView(RoleManager.Self.GoldNum);
        }
        else
        {
            this.mLbGold.text = Tools.longToStr(SHGameManager.CurGold, 3);
            item_leizhu.RefrshSelfGoldInScorllView(SHGameManager.CurGold);
        }
        this.mLbResult.text = Tools.longToStr(SHGameManager.SelfTotalResult, 3);
    }
    private void NetEventHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GF_GAME_SCENE_PLAY://游戏数据同步
                SHGameManager.HandleGameScene(this,pack);
                break;
            case NetCmdType.SUB_S_GAME_FREE_SSZP://游戏进入空闲状态
                this.item_zhuanpan.SetState(SHEnumGameState.Wait, pack.ToObj<CMD_S_GameFree_sszp>().TimeLeave);
                this.item_bet_list.SetEnable(false);
                this.RefershSelfInfo();
                item_leizhu.ChangeWYSLBtnState(true);
                break;
            case NetCmdType.SUB_S_GAME_START_SSZP: //游戏开始
                item_leizhu.ChangeWYSLBtnState(false);
                item_zhuanpan.SetState(SHEnumGameState.Bet, pack.ToObj<CMD_S_GameStart_sszp>().TimeLeave);
                item_bet_list.SetEnable(true);
                item_leizhu.RefershLeiZhuInfo();
                RefershSelfInfo();
                AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_GAME_START);
                AudioManager.PlayAudio(GameEnum.SH,SHGameConfig.Audio_qingxiazhu);
                break;
            case NetCmdType.SUB_S_GAME_END_SSZP: {//游戏结束
                    CMD_S_GameEnd_sszp end = pack.ToObj<CMD_S_GameEnd_sszp>();
                    SHGameManager.PreCalcResult();
                    SHEnumOption resultOption = item_zhuanpan.SetResult(end.TimeLeave, end.TableCard - 1, end.BankerScore, end.UserScore);
                    item_history.RefreshHistroy();
                    item_history.mOptionList.Add(resultOption);
                    item_bet_list.SetEnable(false);
                    AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_SND_STARTSHOWCRAD);
                    SHGameManager.IsFlyChouMa = false;
                    break;
                }
            case NetCmdType.SUB_S_PLACE_JETTON_SSZP: //用户下注
                this.RefershSelfInfo();
                this.item_leizhu.RefershApplyState();
                this.item_bet_list.RefershEnable();
                this.item_zhuanpan.SetBetInfo(SHGameManager.CurAllBet, SHGameManager.CurSelfBet);
                if (SHGameManager.CurSelectBet >= 1000000) {
                    AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_ADD_GOLD_EX);
                } else {
                    AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_ADD_GOLD);
                }
                CMD_S_PlaceJetton_sszp placeJettonResp = pack.ToObj<CMD_S_PlaceJetton_sszp>();
                mCoinFlyCtrl.PlayAnima(placeJettonResp.JettonArea, placeJettonResp.JettonScore,true);
                break;
            case NetCmdType.SUB_S_PLACE_JETTON_BROAD_SSZP://广播下注
                this.item_bet_list.RefershEnable();
                this.item_zhuanpan.SetBetInfo(SHGameManager.CurAllBet, SHGameManager.CurSelfBet);
                CMD_S_PlaceJettonBroad_sszp resp = pack.ToObj<CMD_S_PlaceJettonBroad_sszp>();
         
                 for (int i = 0; i < resp.JettonScore.Length; i++)
                 {
                     if (resp.JettonScore[i] > 0)
                     {
                         mCoinFlyCtrl.PlayAnima(i + 1, resp.JettonScore[i]);
                     }
                 }

                if (!item_zhuanpan.mLbDownCount.text.Equals(string.Empty))
                {
                    int countCown = int.Parse(item_zhuanpan.mLbDownCount.text);

                    if (countCown > 2)
                    {
                        SHGameManager.IsFlyChouMa = true;
                    }
                    else
                    {
                        SHGameManager.IsFlyChouMa = false;
                    }
                }
                else
                {
                    SHGameManager.IsFlyChouMa = false;
                }

                AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_ADD_GOLD);
                break;
            case NetCmdType.SUB_S_APPLY_BANKER_SSZP: {//申请上庄
                    CMD_S_ApplyBanker_sszp cmd = pack.ToObj<CMD_S_ApplyBanker_sszp>();
                    RoleInfo applyLeizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, cmd.ApplyUser);
                    if (applyLeizhu != null)
                    {
                        this.item_leizhu.AddLeiZhu(applyLeizhu);
                    }
                    else
                    {
                        LogMgr.LogError("庄家位置信息错误 : " + cmd.ApplyUser);
                    }
                    break;
                }
            case NetCmdType.SUB_S_CHANGE_BANKER_SSZP: {//切换庄家
                    CMD_S_ChangeBanker_sszp cmd = pack.ToObj<CMD_S_ChangeBanker_sszp>();
                    RoleInfo changLeizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, cmd.BankerUser);
                    this.item_leizhu.SetLeiZhu(changLeizhu, 0, 0);
                    if (changLeizhu != null)
                    {
                        this.item_zhuanpan.ShowTips("轮换擂主", 1);
                    }
                    break;
                }
            case NetCmdType.SUB_S_SEND_RECORD_SSZP: {//游戏记录
                    CMD_S_GameRecord_sszp cmd = pack.ToObj<CMD_S_GameRecord_sszp>();
                    List<SHEnumOption> list = new List<SHEnumOption>();
                    foreach (var item in cmd.GameRecord) {
                        list.Add((SHEnumOption)SHGameConfig.Options[item.Animal - 1]);
                    }
                    this.item_history.InitData(list);
                    break;
                }
            case NetCmdType.SUB_S_CANCEL_BANKER_SSZP: {//取消上庄
                    CMD_S_CancelBanker_sszp cmd = pack.ToObj<CMD_S_CancelBanker_sszp>();
                    RoleInfo leizhu = RoleManager.FindRole(cmd.CancelUser);
                    if (leizhu != null) {
                        this.item_leizhu.RemoveLeiZhu(leizhu);
                    } else {
                        LogMgr.LogError("庄家昵称信息错误 : " + cmd.CancelUser);
                    }
                    break;
                }
            case NetCmdType.SUB_GF_SYSTEM_MESSAGE://桌子内消息
                this.item_message.AddMessage(Tools.MessageColor(pack.ToObj<SC_GR_GF_SystemMessage>().Message), Vector3.one);
                break;
        }
    }

    private void GameEventHandle(GameEvent type, object obj) {
        switch (type) {
            case GameEvent.UserInfoChange://用户信息变更
                if (this.item_zhuanpan.mGameState != SHEnumGameState.Result || obj != RoleManager.Self)
                {
                    if (obj == RoleManager.Self)
                    {
                        this.RefershSelfInfo();
                        this.item_leizhu.RefershApplyState();
                    }
                }
                break;
            case GameEvent.UserEneter://用户进入
                item_leizhu.AddOnLineRole(obj as RoleInfo);
                if (this.gameObject.activeSelf) {
                    this.item_message.AddMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]进来了[-][/c]", (obj as RoleInfo).NickName), Vector3.one,false);
                }
                break;
            case GameEvent.UserStateChange://用户状态变更
                break;
            case GameEvent.UserLeaveTable://用户离开桌子
                item_leizhu.RemoveOnLineRole(obj as RoleInfo);
                if (obj == RoleManager.Self) {
                    this.Exit();
                } else {
                    this.item_message.AddMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]离开了[-][/c]", (obj as RoleInfo).NickName), Vector3.one,false);
                }
                break;
        }
    }
    public void Exit() {
        this.Close();
        NetClient.CloseConnect();
        GameSceneManager.BackToHall(GameEnum.SH);
    }
    private void OnUserStateChange(GameEvent type, RoleInfo role, EnumUserStats state) {
        if (state == EnumUserStats.US_OFFLINE) {//离线
            this.item_message.AddMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]断线了,请耐心等候![-][/c]", role.NickName), Vector3.one,false);
        } else if (role.UserStatus == EnumUserStats.US_OFFLINE && role.TableID != ushort.MaxValue) {//断线重连
            this.item_message.AddMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]重连成功[-][/c]", role.NickName), Vector3.one,false);
        }
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_GAME_SCENE_PLAY, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_FREE_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_START_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_END_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PLACE_JETTON_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_APPLY_BANKER_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CHANGE_BANKER_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_SEND_RECORD_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CANCEL_BANKER_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PLACE_JETTON_BROAD_SSZP, this.NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_SYSTEM_MESSAGE, this.NetEventHandle);

        EventManager.RegisterEvent(GameEvent.UserInfoChange, this.GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserEneter, this.GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserStateChange, this.GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserLeaveTable, this.GameEventHandle);

        EventManager.RegisterEvent<RoleInfo, EnumUserStats>(GameEvent.UserStateChange, this.OnUserStateChange);
        this.gameObject.SetActive(false);

        // TimeManager.DelayExec(this, UI.AnimTime, () => {
        //     AudioManager.PlayMusic(GameEnum.SH,SHGameConfig.Audio_BG);
        // });
        AudioManager.PlayMusic(GameEnum.SH,SHGameConfig.Audio_BG);
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_GAME_SCENE_PLAY, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_FREE_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_START_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_END_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PLACE_JETTON_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_APPLY_BANKER_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CHANGE_BANKER_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_SEND_RECORD_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CANCEL_BANKER_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PLACE_JETTON_BROAD_SSZP, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_SYSTEM_MESSAGE, this.NetEventHandle);

        EventManager.UnRegisterEvent(GameEvent.UserInfoChange, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserEneter, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserStateChange, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserLeaveTable, this.GameEventHandle);

        EventManager.UnRegisterEvent<RoleInfo, EnumUserStats>(GameEvent.UserStateChange, this.OnUserStateChange);

        AudioManager.StopMusic();
    }

    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_hide_menu":
                this.SetShowMenu(false);
                break;
            case "btn_show_menu":
                this.SetShowMenu(true);
                break;
            case "btn_menu":
                mMenu.SetActive(!mMenu.activeSelf);
                mOpenArrow.SetActive(!mMenu.activeSelf);
                mCloseArrow.SetActive(mMenu.activeSelf);
                break;
            case "btn_help":
                //UI.EnterUI<UI_SHRule>(null);
                UI.EnterUI<UI_SHRule>(GameEnum.SH);
                break;
            case "btn_setting":
                UI.EnterUI<UI_Shenhua_setting>(GameEnum.SH).InitData();
                break;
            case "btn_exit":
                if (SHGameManager.CurTotalGold > 0 || SHGameManager.LeiZhuSeat == RoleManager.Self.ChairSeat) {
                    //UI.EnterUI<UI_SHwindow>(ui => 
                    //{
                    //    ui.InitData("你正在游戏中，强退将扣分，是否确认退出？", () => 
                    //    {
                    //        if (SHGameManager.LeiZhuSeat == RoleManager.Self.ChairSeat)
                    //        {//擂主强退发送强制起立消息
                    //            NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp {
                    //                ForceLeave = 1,
                    //                TableID = RoleManager.Self.TableID,
                    //                ChairID = RoleManager.Self.ChairSeat,
                    //            });
                    //        }
                    //        this.Exit();
                    //    }, null);
                    //});

                    UI.EnterUI<UI_SHwindow>(GameEnum.SH).InitData("你正在游戏中，强退将扣分，是否确认退出？",()=> 
                    {
                        if (SHGameManager.LeiZhuSeat == RoleManager.Self.ChairSeat)
                        {//擂主强退发送强制起立消息
                            NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp
                            {
                                ForceLeave = 1,
                                TableID = RoleManager.Self.TableID,
                                ChairID = RoleManager.Self.ChairSeat,
                            });
                        }
                        this.Exit();
                    }, null);

                } else {
                    NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp {
                        ForceLeave = 1,
                        TableID = RoleManager.Self.TableID,
                        ChairID = RoleManager.Self.ChairSeat,
                    });
                }
                break;
            case "btn_safebox"://保险箱
                //UI.EnterUI<UI_safebox_new>(ui => {
                //    ui.InitData();
                //});
                UI.EnterUI<UI_safebox_new>(GameEnum.All).InitData();
                break;
        }
    }

    private void Update()
    {
        if (SHGameManager.IsFlyChouMa)
        {
            EveryflyCoinTime -= Time.deltaTime;
            if (EveryflyCoinTime <= 0)
            {
                mCoinFlyCtrl.PlayAnimaRandom();
                //AudioManager.PlayAudio(SHGameConfig.Audio_ADD_GOLD);
                EveryflyCoinTime = 0.3f;
            }
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "info_leizhu":
                this.item_leizhu = this.BindItem<Item_ShenHua_LeiZhu>(tf.gameObject);
                break;
            case "lb_player_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "lb_player_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "lb_player_result":
                this.mLbResult = tf.GetComponent<UILabel>();
                break;
            case "item_history_info":
                this.item_history = this.BindItem<Item_ShenHua_History>(tf.gameObject);
                break;
            case "menu":
                this.mMenu = tf.gameObject;
                break;
            case "btn_show_menu":
                //this.mBtnShowMenu = tf.gameObject;
                break;
            case "btn_hide_menu":
                //this.mBtnHideMenu = tf.gameObject;
                break;
            case "info_msg":
                this.item_message = this.BindItem<Item_ShenHua_Message>(tf.gameObject);
                break;
            case "info_bet":
                this.item_bet_list = this.BindItem<Item_ShenHua_BetList>(tf.gameObject);
                break;
            case "zhuanpan_info":
                this.item_zhuanpan = this.BindItem<Item_ShenHua_ZhuanPan>(tf.gameObject);
                break;
            case "coin_fly_ctrl":
                mCoinFlyCtrl = BindItem<Item_ShenHua_Coin_Fly_Ctrl>(tf.gameObject);
                break;
            case "open":
                mOpenArrow = tf.gameObject;
                break;
            case "close":
                mCloseArrow = tf.gameObject;
                break;
            case "music_slider":
                mSliderMusic = tf.GetComponent<UISlider>();
                break;
            case "sound_slider":
                mSliderSound = tf.GetComponent<UISlider>();
                break;
        }
    }
}
