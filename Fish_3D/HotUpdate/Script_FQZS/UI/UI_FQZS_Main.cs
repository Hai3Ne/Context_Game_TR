using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UI_FQZS_Main : UILayer
{
    /// <summary>
    /// 上擂UI
    /// </summary>
    public Item_FQZS_ShangLei mShangLei;

    /// <summary>
    /// 游戏状态UI
    /// </summary>
    public Item_FQZS_GameState mGameState;

    /// <summary>
    /// 擂主UI
    /// </summary>
    public Item_FQZS_LeiZhu mLeiZhu;

    /// <summary>
    /// 更多信息UI
    /// </summary>
    public Item_FQZS_MoreInfo mMoreWindow;

    /// <summary>
    /// 下注动物的UI
    /// </summary>
    public Item_FQZS_AnimaSelect mAnimSelect;

    /// <summary>
    /// 选择筹码UI
    /// </summary>
    public Item_FQZS_Bet mSelectChouMa;

    /// <summary>
    /// 自己的信息
    /// </summary>
    public Item_FQZS_SelfInfo mSelfInfo;

    /// <summary>
    /// 中间转盘
    /// </summary>
    public Item_FQZS_ZhuanPan mZhuanPan;

    /// <summary>
    /// 中奖历史
    /// </summary>
    public Item_FQZS_Histroy mHistroy;

    /// <summary>
    /// 押注动画控制
    /// </summary>
    public Item_FQZS_CoinAnimaCtrl mCoinAnimaCtrl;

    private int AnimaStartIndex = 0;
    private int AnimaEndIndex = 0;

    private float betSoundTime = 0.1f;

    private List<string> mSpriteNames = new List<string>();

    private GameObject mTrans;

    /// <summary>
    /// 当前申请擂主的人数
    /// </summary>
    private int CurrApplyLeiZhuNum = 0;

    private byte mCurSelectAnima;

    private long mLizhuWinSocre;

    public override void OnButtonClick(GameObject obj)
    {
        base.OnButtonClick(obj);
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    public void InitData(RoleInfo leizhu, List<RoleInfo> leizhu_list, int leizhu_count, float downcount, long[] all_gold, long[] self_gold)
    {
        this.gameObject.SetActive(true);
        UI.ExitOtherUI(this);
        mLeiZhu.InitData(leizhu, leizhu_count, mMoreWindow);
        mSelectChouMa.InitData();
        mSelfInfo.RefreshSelfInfo();
        mZhuanPan.InitData(false, OpenBigShow);
        mAnimSelect.RefreshBetInfo(all_gold, self_gold);
        mGameState.RefreshGameState(downcount, mAnimSelect);
        FQZSGameManager.ClearXuTouArr();
        mSelectChouMa.SetState(mAnimSelect);
        mGameState.OnGameInWait = mSelectChouMa.SetState;
        mHistroy.InitData();
        mMoreWindow.InitData(QuitGame);
        mShangLei.InitData(leizhu_list);
        FQZSGameManager.CurrSelectChouMa = null;
    }

    /// <summary>
    /// 打开中奖动物的序列帧展示
    /// </summary>
    /// <param name="index"></param>
    private void OpenBigShow(int index)
    {
        string currSelect = FQZSGameManager.GetAnimaByCode(FQZSGameManager.SelectAni, true);

        if (currSelect == "shizi" || currSelect == "tongchi" || currSelect == "tongpei" || currSelect == "yanzi")
            currSelect = "common";
        GameObject go = ResManager.LoadAndCreate(GameEnum.FQZS, string.Format("Prefabs/SpriteAnima/fqzs_{0}", currSelect), mTrans.transform);
        UISpriteAnimation animation = go.GetComponent<UISpriteAnimation>();

        SetSpritesList(animation);

        StartCoroutine(HideAnima(go));
    }

    IEnumerator HideAnima(GameObject anima)
    {
        yield return new WaitForSeconds(3);

        Destroy(anima);
    }

    private void SetSpritesList(UISpriteAnimation spriteAnima)
    {
        string currSelectAnima = FQZSGameManager.GetAnimaByCode(FQZSGameManager.SelectAni, true);
        mSpriteNames.Clear();
        switch (currSelectAnima)
        {
            case "shayu":
                SetSprites("shayu_", 1, 6);
                break;
            case "yanzi":
                SetSprites("yanzi_", 0, 5);
                break;
            case "tuzi":
                SetSprites("tuzi_", 0, 5);
                break;
            case "gezi":
                SetSprites("gezi_", 0, 5);
                break;
            case "xiongmao":
                SetSprites("xiongmao_", 0, 5);
                break;
            case "kongque":
                SetSprites("kongque_", 0, 8);
                break;
            case "houzi":
                SetSprites("houzi_", 0, 5);
                break;
            case "laoying":
                SetSprites("laoying_", 0, 5);
                break;
            case "shizi":
                SetSprites("shizi_", 0, 5);
                break;
            case "tongchi":
                SetSprites("tongchi_", 0, 8);
                break;
            case "tongpei":
                SetSprites("tongpei_", 0, 7);
                break;
            case "jinsha":
                SetSprites("jinsha_", 1, 8);
                break;
        }

        spriteAnima.SetSpriteList(mSpriteNames);
        spriteAnima.framesPerSecond = 10;
        spriteAnima.ResetToBeginning();
        spriteAnima.Play();
    }

    private void SetSprites(string animaName,int startIndex,int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            string curAnimaName = animaName + i;
            mSpriteNames.Add(curAnimaName);
        }
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void QuitGame()
    {
        if (FQZSGameManager.CurSelfTotalGold > 0 || FQZSGameManager.LeiZhuSeat == RoleManager.Self.ChairSeat)
        {
            UI.EnterUI<UI_FQZS_NoticeWin>(GameEnum.FQZS).InitData(() =>
            {
                NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp
                {
                    ForceLeave = 1,
                    TableID = RoleManager.Self.TableID,
                    ChairID = RoleManager.Self.ChairSeat,
                });
               Exit();
            },"你正在游戏中，强退将扣分，是否确认退出？");
        }
        else
        {
            NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp
            {
                ForceLeave = 1,
                TableID = RoleManager.Self.TableID,
                ChairID = RoleManager.Self.ChairSeat,
            });
        }
    }

    private void Exit()
    {
        Close();
        NetClient.CloseConnect();
        GameSceneManager.BackToHall(GameEnum.FQZS);
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch(name)
        {
            case "bg_leizhu":
                mLeiZhu = BindItem<Item_FQZS_LeiZhu>(tf.gameObject);
                break;
            case "gamestate":
                mGameState = BindItem<Item_FQZS_GameState>(tf.gameObject);
                break;
            case "shanglei":
                mShangLei = BindItem<Item_FQZS_ShangLei>(tf.gameObject);
                break;
            case "more_win":
                mMoreWindow = BindItem<Item_FQZS_MoreInfo>(tf.gameObject);
                break;
            case "animal_select":
                mAnimSelect = BindItem<Item_FQZS_AnimaSelect>(tf.gameObject);
                break;
            case "Bet":
                mSelectChouMa = BindItem<Item_FQZS_Bet>(tf.gameObject);
                break;
            case "info_self":
                mSelfInfo = BindItem<Item_FQZS_SelfInfo>(tf.gameObject);
                break;
            case "anima_zhuan_pan":
                mZhuanPan = BindItem<Item_FQZS_ZhuanPan>(tf.gameObject);
                break;
            case "histroy":
                mHistroy = BindItem<Item_FQZS_Histroy>(tf.gameObject);
                break;
            case "fly_coin_ctrl":
                mCoinAnimaCtrl = BindItem<Item_FQZS_CoinAnimaCtrl>(tf.gameObject);
                break;
            case "anima_show":
                mTrans = tf.gameObject;
                break;
        }
    }

    /// <summary>
    /// 加载飞禽走兽主界面完成回调
    /// </summary>
    public override void OnNodeLoad()
    {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_GAME_SCENE_PLAY,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_FREE_FQZS,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_START_FQZS,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_GAME_END_FQZS,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_PLACE_JETTON_FQZS,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_APPLY_BANKER,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CHANGE_BANKER, NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_SCORE_HISTORY_FQZS,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_CANCEL_BANKER,NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GF_SYSTEM_MESSAGE, NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_AGAIN_JETTON_FQZS, NetEventHandle);

        EventManager.RegisterEvent(GameEvent.UserEneter, GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserLeaveTable, GameEventHandle);
        EventManager.RegisterEvent(GameEvent.UserInfoChange, GameEventHandle);

        EventManager.RegisterEvent<RoleInfo, EnumUserStats>(GameEvent.UserStateChange, this.OnUserStateChange);

        TimeManager.DelayExec(this, UI.AnimTime, () => {
            AudioManager.PlayMusic(GameEnum.FQZS,FQZSGameConfig.SOUND_BGM);
        });

        this.transform.localScale = new Vector3(Resolution.ViewAdaptAspect, 1, 1);//特殊分辨率自适应
    }

    private void OnUserStateChange(GameEvent type, RoleInfo role, EnumUserStats state)
    {
        if (state == EnumUserStats.US_OFFLINE)
        {
            //离线
            mMoreWindow.AddSysMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]断线了,请耐心等候![-][/c]", role.NickName));
        }
        else if (role.UserStatus == EnumUserStats.US_OFFLINE && role.TableID != ushort.MaxValue)
        {
            //断线重连
            mMoreWindow.AddSysMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]重连成功[-][/c]", role.NickName));
        }
    }


    private void GameEventHandle(GameEvent type, object obj)
    {
        switch (type)
        {
            case GameEvent.UserEneter:
                mMoreWindow.AddRole(obj as RoleInfo);
                mMoreWindow.AddSysMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]进来了[-][/c]", (obj as RoleInfo).NickName));
                break;
            case GameEvent.UserLeaveTable:
                mMoreWindow.RemoveRole(obj as RoleInfo);
                mMoreWindow.AddSysMessage(string.Format("[c][3590FF][{0}]:[-][/c][c][AF955F]离开了[-][/c]", (obj as RoleInfo).NickName));
                if ((obj as RoleInfo).UserID == RoleManager.Self.UserID)
                    Exit();
                break;
            case GameEvent.UserInfoChange:
                if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Reslut)
                {
                    long curGold = RoleManager.Self.GoldNum + FQZSGameManager.CurSelfResult - FQZSGameManager.CurRevenue;
                    mShangLei.RefreshSelfGoldInScorllView(curGold);
                }
                else
                {
                    mShangLei.RefreshSelfGoldInScorllView(FQZSGameManager.CurGold);
                    mSelfInfo.RefreshSelfInfo();
                }
                break;
        }
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    public override void OnExit()
    {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_GAME_SCENE_PLAY, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_FREE_FQZS, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_START_FQZS, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_GAME_END_FQZS, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_PLACE_JETTON_FQZS, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_APPLY_BANKER, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CHANGE_BANKER, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_SCORE_HISTORY_FQZS, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_CANCEL_BANKER, this.NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GF_SYSTEM_MESSAGE, NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_AGAIN_JETTON_FQZS, NetEventHandle);

        EventManager.UnRegisterEvent(GameEvent.UserEneter, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserLeaveTable, this.GameEventHandle);
        EventManager.UnRegisterEvent(GameEvent.UserInfoChange, GameEventHandle);

        EventManager.UnRegisterEvent<RoleInfo, EnumUserStats>(GameEvent.UserStateChange, this.OnUserStateChange);

        CurrApplyLeiZhuNum = 0;
    }

    /// <summary>
    /// 消息监听
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pack"></param>
    private void NetEventHandle(NetCmdType type, NetCmdPack pack)
    {
        switch (type)
        {
            case NetCmdType.SUB_GF_GAME_SCENE_PLAY:
                FQZSGameManager.HandleGameScene(this, pack);
                break;
            case NetCmdType.SUB_S_GAME_FREE_FQZS:
                //准备开始(倒计时5秒)
                CMD_S_GameFree_fqzs respFree = pack.ToObj<CMD_S_GameFree_fqzs>();
                FQZSGameManager.CurrGameState = FQZSEnumGameState.Free;
                mGameState.RefreshGameState(respFree.TimeLeave);
                mSelectChouMa.SetState(mAnimSelect);
                mAnimSelect.ClearBetInfo();
                mCoinAnimaCtrl.ResetCoin();
                mZhuanPan.RefreshData(true);
                FQZSGameManager.IsFlyCoin = false;
                mShangLei.ChangeWYSLBtnState(true);
                break;
            case NetCmdType.SUB_S_GAME_START_FQZS:
                //开始押注前的短暂时间
                mShangLei.ChangeWYSLBtnState(false);
                CMD_S_GameStart_fqzs respStart = pack.ToObj<CMD_S_GameStart_fqzs>();
                FQZSGameManager.CurrGameState = FQZSEnumGameState.Bet;
                mGameState.RefreshGameState(respStart.TimeLeave);
                FQZSGameManager.UserMaxGold = respStart.UserMaxScore;
                FQZSGameManager.LeiZhuSeat = respStart.BankerUser;
                FQZSGameManager.LeiZhuLeDou = respStart.BankerScore;
                mZhuanPan.ShowTip("开始游戏");
                mSelectChouMa.SetState(mAnimSelect);
                mLeiZhu.RefrshLsiZhuInfo(mMoreWindow);
                mSelfInfo.RefreshSelfInfo();
                FQZSGameManager.IsFlyCoin = false;
                AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_START);
                mSelectChouMa.IsAreadyXuTou = false;
                break;
            case NetCmdType.SUB_S_GAME_END_FQZS:
                //开始转盘(有20秒倒计时)
                CMD_S_GameEnd_fqzs respEnd = pack.ToObj<CMD_S_GameEnd_fqzs>();
                FQZSGameManager.CurrGameState = FQZSEnumGameState.End;
                mGameState.RefreshGameState(respEnd.TimeLeave);
                FQZSGameManager.LastEndIndex = respEnd.LastEndIndex;
                FQZSGameManager.CurResultIndex = respEnd.ResultIndex;
                FQZSGameManager.CurStopIndex = respEnd.EndIndex;
                FQZSGameManager.SelectAni = respEnd.SelectAni;
                FQZSGameManager.LeiZhuLeDou = respEnd.BankerTotallScore;
                FQZSGameManager.CurLeiZhuResult = respEnd.BankerScore;
                FQZSGameManager.LeiZhuTimes = respEnd.BankerTime;
                FQZSGameManager.CurSelfResult = respEnd.UserScore;
                FQZSGameManager.UserName = respEnd.NamePlayer;
                mSelectChouMa.SetState(mAnimSelect);
                mZhuanPan.StartRotate();
                mMoreWindow.AddSelfBetMessage(GetSelfBet());
                break;
            case NetCmdType.SUB_S_PLACE_JETTON_FQZS:
                if (mGameState.CountTime > 0)
                {
                    //押注时刻(有20秒倒计时)
                    FQZSGameManager.IsFlyCoin = true;
                    FQZSGameManager.CurrGameState = FQZSEnumGameState.Bet;
                }
                else
                {
                    //倒计时结束的时候就不能再下注了
                    FQZSGameManager.CurrGameState = FQZSEnumGameState.End;
                }
                CMD_S_PlaceJetton_fqzs respJetton = pack.ToObj<CMD_S_PlaceJetton_fqzs>();

                //只要自己下注了就不能再续投了
                if (respJetton.ChairID == RoleManager.Self.ChairSeat)
                {
                    if (!mSelectChouMa.IsAreadyXuTou)
                        mSelectChouMa.IsAreadyXuTou = true;
                }

                mGameState.RefreshGameState();
                mSelectChouMa.SetState(mAnimSelect);
                mSelfInfo.RefreshSelfInfo();
                mAnimSelect.RefreshBetInfo(FQZSGameManager.CurAllBet, FQZSGameManager.CurSelfBet);
                mCoinAnimaCtrl.PlayAnima(respJetton.JettonArea, respJetton.JettonScore);
                break;
            case NetCmdType.SUB_S_APPLY_BANKER:
                CMD_S_ApplyBanker_fqzs applyLeiZhu = pack.ToObj<CMD_S_ApplyBanker_fqzs>();
                CurrApplyLeiZhuNum++;
                //有人申请擂主
                mShangLei.ApplyLeiZhuCallBack(CurrApplyLeiZhuNum, applyLeiZhu.ApplyUser);
                break;
            case NetCmdType.SUB_S_CHANGE_BANKER:  //擂主被更换
                CMD_S_ChangeBanker leiZhuChange = pack.ToObj<CMD_S_ChangeBanker>();
                FQZSGameManager.LeiZhuLeDou = leiZhuChange.BankerScore;
                FQZSGameManager.LeiZhuTimes = 0;
                mZhuanPan.ShowTip("轮换擂主");
                CurrApplyLeiZhuNum--;
                if (CurrApplyLeiZhuNum <= 0)
                    CurrApplyLeiZhuNum = 0;
                FQZSGameManager.LeiZhuSeat = leiZhuChange.BankerUser;
                mShangLei.ChangeLeiZhuCallBack(CurrApplyLeiZhuNum, leiZhuChange.BankerUser);
                mLeiZhu.RefrshLsiZhuInfo(mMoreWindow, true);
                AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_CHANGE_BANKER);
                break;
            case NetCmdType.SUB_S_SCORE_HISTORY_FQZS: //结算
                FQZSGameManager.CurrGameState = FQZSEnumGameState.Reslut;
                CMD_S_ScoreHistory_fqzs historyResp = pack.ToObj<CMD_S_ScoreHistory_fqzs>();
                UI.EnterUI<UI_FQZS_ResultWin>(GameEnum.FQZS).InitData(historyResp.ScoreInfoZhuang.NickName, historyResp.ScoreInfoZhuang.WinScore, RoleManager.Self.NickName, FQZSGameManager.CurSelfResult, historyResp.SelectAni, historyResp.ScoreRankInfo);
                mZhuanPan.RefreshData(true);
                mHistroy.UpdateHistroy(1, historyResp.ScoreHistroy);
                mCurSelectAnima = historyResp.SelectAni;
                mLizhuWinSocre = historyResp.ScoreInfoZhuang.WinScore;
                mMoreWindow.AddResultMessage(FQZSGameManager.GetAnimaByCode(mCurSelectAnima), mLizhuWinSocre.ToString(), FQZSGameManager.CurSelfResult.ToString(), FQZSGameManager.CurSelfTotalGold.ToString(), FQZSGameManager.CurRevenue.ToString());
                mSelfInfo.RefreshSelfInfo();
                mLeiZhu.RefrshLsiZhuInfo(mMoreWindow);
                break;
            case NetCmdType.SUB_S_CANCEL_BANKER:
                //取消申请擂主返回
                CMD_S_CancelBanker cancelApply = pack.ToObj<CMD_S_CancelBanker>();
                CurrApplyLeiZhuNum--;
                if (CurrApplyLeiZhuNum <= 0)
                    CurrApplyLeiZhuNum = 0;
                mShangLei.CancelApplyLeiZhu(CurrApplyLeiZhuNum, cancelApply.CancelUser);
                break;
            case NetCmdType.SUB_GF_SYSTEM_MESSAGE:

                SC_GR_GF_SystemMessage msg = pack.ToObj<SC_GR_GF_SystemMessage>();
                string sysGFMessage = msg.Message;

                if ((msg.Type & (ushort)SysMessageType.SMT_EJECT) != 0)
                    return;
                mMoreWindow.AddSysMessage(sysGFMessage,true);
                break;
            case NetCmdType.SUB_S_AGAIN_JETTON_FQZS:
                mAnimSelect.RefreshBetInfo(FQZSGameManager.CurAllBet, FQZSGameManager.CurSelfBet);
                break;
        }
    }

    private void Update()
    {
        if (FQZSGameManager.IsFlyCoin && FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
        {
            betSoundTime -= Time.deltaTime;
            if (betSoundTime <= 0)
            {
                betSoundTime = 0.1f;
                AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_ADDCHIP);
            }
            mCoinAnimaCtrl.PlayAnimaRandom();
        }
    }

    private string GetSelfBet()
    {
        bool isTouZhu = false;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < FQZSGameManager.CurSelfBet.Length; i++)
        {
            if (FQZSGameManager.CurSelfBet[i] > 0)
            {
                isTouZhu = true;

                sb.Append(FQZSGameManager.GetAnimaByCode((byte)(i + 1)) +"," + FQZSGameManager.CurSelfBet[i] + "#");
            }
        }

        if (!isTouZhu)
        {
            return string.Empty;
        }

        return sb.ToString();
    }
}
