using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FQZSGameManager
{
    /// <summary>
    /// 所有区域所有玩家下注的金额
    /// </summary>
    public static long[] CurAllBet = new long[FQZSGameConfig.MaxAreaCount];

    /// <summary>
    /// 本家所下注的区域的金额
    /// </summary>
    public static long[] CurSelfBet = new long[FQZSGameConfig.MaxAreaCount];

    /// <summary>
    /// 当局已下注金额
    /// </summary>
    public static long CurSelfTotalGold;
    /// <summary>
    /// 当局游戏税收
    /// </summary>
    public static long CurRevenue;
    /// <summary>
    /// 擂主座位号
    /// </summary>
    public static ushort LeiZhuSeat;
    /// <summary>
    /// 擂主当前乐豆
    /// </summary>
    public static long LeiZhuLeDou;
    /// <summary>
    /// 用户下注最大额度
    /// </summary>
    public static long UserMaxGold;

    /// <summary>
    /// 擂主本局输赢
    /// </summary>
    public static long CurLeiZhuResult;
    /// <summary>
    /// 本家本局的输赢
    /// </summary>
    public static long CurSelfResult;
    /// <summary>
    /// 本家总输赢(战绩)
    /// </summary>
    public static long SelfTotalResult;
    /// <summary>
    /// 结束时选择的动物
    /// </summary>
    public static byte SelectAni;

    /// <summary>
    /// 擂主连擂次数
    /// </summary>
    public static int LeiZhuTimes;

    /// <summary>
    /// 押注区域中奖索引
    /// </summary>
    public static int CurResultIndex;

    /// <summary>
    /// 转盘最终停止的索引
    /// </summary>
    public static int CurStopIndex;

    /// <summary>
    /// 上次停止的索引
    /// </summary>
    public static int LastEndIndex;

    /// <summary>
    /// 上擂必须大于这个数值才能上擂
    /// </summary>
    public static long ApplyBankerCondition;

    /// <summary>
    /// 系统是否坐庄
    /// </summary>
    public static bool IsSysLeiZhu;

    /// <summary>
    /// 区域限制
    /// </summary>
    public static long AreaLimitScore;

    /// <summary>
    /// 该局结束时本家
    /// </summary>
    public static long CurEndUserScore;

    /// <summary>
    /// 每局游戏结束时本家的名称,结算界面用到
    /// </summary>
    public static string UserName;

    /// <summary>
    /// 游戏状态
    /// </summary>
    public static FQZSEnumGameState CurrGameState;

    /// <summary>
    /// 当前选择的筹码
    /// </summary>
    public static Item_FQZS_ChouMa CurrSelectChouMa;

    /// <summary>
    /// 开始下注
    /// </summary>
    public static bool IsFlyCoin = false;

    /// <summary>
    /// 续押注字典
    /// </summary>
    public static long[] mXuTouArr = new long[11];

    /// <summary>
    /// 本家剩余的乐豆
    /// </summary>
    public static long CurGold
    {
        get
        {
            return RoleManager.Self.GoldNum - CurSelfTotalGold;
        }
    }

    public static void ClearXuTouArr()
    {
        for (int i = 0; i < mXuTouArr.Length; i++)
        {
            mXuTouArr[i] = 0;
        }
    }

    /// <summary>
    /// 获取目前还能够下注的金额
    /// </summary>
    /// <returns></returns>
    public static long GetCurBetMax()
    {
        long gold = 0;
        foreach (var item in CurAllBet)
        {
            if (gold < AreaLimitScore - item)
            {
                gold = AreaLimitScore - item;
            }
        }
        if (gold > UserMaxGold - CurSelfTotalGold)
        {
            gold = UserMaxGold - CurSelfTotalGold;
        }
        return gold;
    }

    /// <summary>
    /// 用户下注
    /// </summary>
    /// <param name="cmd"></param>
    public static void HandlePlaceJetton(CMD_S_PlaceJetton_fqzs cmd)
    {
        if (cmd.ChairID == RoleManager.Self.ChairSeat)
        {
            CurSelfBet[cmd.JettonArea - 1] += cmd.JettonScore;
            CurSelfTotalGold += cmd.JettonScore;
        }
        CurAllBet[cmd.JettonArea - 1] += cmd.JettonScore;
    }

    /// <summary>
    /// 用户续押
    /// </summary>
    /// <param name="cmd"></param>
    public static void HandleAgainJetton(CMD_S_Again_Jetton_fqzs cmd)
    {
        if (cmd.ChairID == RoleManager.Self.ChairSeat)
        {
            for (int i = 0; i < cmd.AreaJetton.Length; i++)
            {
                CurSelfBet[i] += cmd.AreaJetton[i];
                CurSelfTotalGold += cmd.AreaJetton[i];

                CurAllBet[i] += cmd.AreaJetton[i];
            }
        }
        else
        {
            for (int i = 0; i < cmd.AreaJetton.Length; i++)
            {
                CurAllBet[i] += cmd.AreaJetton[i];
            }
        }
    }

    /// <summary>
    /// 擂主切换
    /// </summary>
    /// <param name="cmd"></param>
    public static void HandleChangeLeiZhu(CMD_S_ChangeBanker cmd)
    {

    }

    /// <summary>
    /// 进入游戏开始状态
    /// </summary>
    /// <param name="game_start"></param>
    public static void HandleGameStart(CMD_S_GameStart_fqzs game_start)
    {
        CurSelfTotalGold = 0;
        CurRevenue = 0;
        for (int i = 0; i < FQZSGameConfig.MaxAreaCount; i++)
        {
            CurAllBet[i] = 0;
            CurSelfBet[i] = 0;
        }
        LeiZhuSeat = game_start.BankerUser;
        LeiZhuLeDou = game_start.BankerScore;
        UserMaxGold = game_start.UserMaxScore;
    }

    /// <summary>
    /// 游戏结束状态
    /// </summary>
    /// <param name="game_end"></param>
    public static void HandleGameEnd(CMD_S_GameEnd_fqzs game_end)
    {
        LeiZhuLeDou = game_end.BankerTotallScore;
        CurResultIndex = game_end.ResultIndex;
        CurStopIndex = game_end.EndIndex;
        CurLeiZhuResult = game_end.BankerScore;
        CurSelfResult = game_end.UserScore;
        SelfTotalResult += CurSelfResult;
        LeiZhuTimes = game_end.BankerTime;
        CurRevenue = game_end.Revenue;
        SelectAni = game_end.SelectAni;
        LastEndIndex = game_end.LastEndIndex;
    }

    /// <summary>
    /// 同步游戏状态(进入游戏房间的时候同步一次)
    /// </summary>
    /// <param name="ui"></param>
    /// <param name="pack"></param>
    public static void HandleGameScene(UI_FQZS_Main ui, NetCmdPack pack)
    {
        RoleInfo leizhu = null;

        float downcount = 0;
        CurSelfTotalGold = 0;
        SelfTotalResult = 0;

        switch (GameManager.CurGameState)
        {
            case GameState.FQZS_GAME_STATE_FREE:
                CMD_S_StatusFree_fqzs cmd = pack.ToObj<CMD_S_StatusFree_fqzs>();
                leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, cmd.BankerUser);
                CurrGameState = FQZSEnumGameState.Free;
                downcount = cmd.TimeLeave;
                SetFreeData(cmd);
                break;
            case GameState.FQZS_GAME_STATE_PLAY:
            case GameState.FQZS_GAME_STATE_END:
            case GameState.FQZS_GAME_STATE_RESULT:
                if (GameManager.CurGameState == GameState.FQZS_GAME_STATE_PLAY)
                    CurrGameState = FQZSEnumGameState.Bet;
                else if (GameManager.CurGameState == GameState.FQZS_GAME_STATE_END)
                    CurrGameState = FQZSEnumGameState.End;
                else if (GameManager.CurGameState == GameState.FQZS_GAME_STATE_RESULT)
                    CurrGameState = FQZSEnumGameState.Reslut;
                CMD_S_StatusPlay_fqzs resp = pack.ToObj<CMD_S_StatusPlay_fqzs>();
                leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, resp.BankerUser);
                downcount = resp.TimeLeave;
                SetPlayData(resp);
                break;
        }

        ui.InitData(leizhu, null, LeiZhuTimes, downcount, CurAllBet, CurSelfBet);
    }

    /// <summary>
    /// 设置游戏中或者游戏结束时的数据
    /// </summary>
    /// <param name="resp"></param>
    private static void SetPlayData(CMD_S_StatusPlay_fqzs resp)
    {
        for (int i = 0; i < FQZSGameConfig.MaxAreaCount; i++)
        {
            CurAllBet[i] = resp.ALLUserScore[i + 1];
            CurSelfBet[i] = resp.UserScore[i + 1];
            CurSelfTotalGold += CurSelfBet[i];
        }

        AreaLimitScore = resp.AreaLimitScore;
        ApplyBankerCondition = resp.ApplyBankerCondition;
        CurResultIndex = resp.ResultIndex;
        CurStopIndex = resp.EndIndex;
        LastEndIndex = resp.LastEndIndex;
        SelectAni = resp.SelectAni;
        LeiZhuSeat = resp.BankerUser;
        LeiZhuTimes = resp.BankerTime;
        LeiZhuLeDou = resp.BankerScore;
        IsSysLeiZhu = resp.EnableSysBanker;

        if (CurSelfTotalGold > 0)
        {
            if (resp.EndRevenue != 0)
            {
                CurRevenue = resp.EndRevenue;
            }
        }
        else
            CurRevenue = resp.EndRevenue;

        UserMaxGold = resp.UserMaxScore;
    }

    /// <summary>
    /// 设置空闲状态的数据
    /// </summary>
    /// <param name="cmd"></param>
    private static void SetFreeData(CMD_S_StatusFree_fqzs cmd)
    {
        UserMaxGold = cmd.UserMaxScore;
        ApplyBankerCondition = cmd.ApplyBankerCondition;
        LastEndIndex = cmd.LastEndIndex;
        AreaLimitScore = cmd.AreaLimitScore;
        LeiZhuSeat = cmd.BankerUser;
        LeiZhuTimes = cmd.BankerTime;
        LeiZhuLeDou = cmd.BankerScore;
        IsSysLeiZhu = cmd.EnableSysBanker;
        CurRevenue = 0;
        CurSelfTotalGold = 0;
    }

    public static string GetAnimaByCode(byte code, bool isPingYin = false)
    {
        switch (code)
        {
            case 1:
                return isPingYin ? "shayu" : "鲨鱼";
            case 4:
                return isPingYin ? "yanzi" : "燕子";
            case 5:
                return isPingYin ? "tuzi" : "兔子";
            case 6:
                return isPingYin ? "gezi" : "鸽子";
            case 7:
                return isPingYin ? "xiongmao" : "熊猫";
            case 8:
                return isPingYin ? "kongque" : "孔雀";
            case 9:
                return isPingYin ? "houzi" : "猴子";
            case 10:
                return isPingYin ? "laoying" : "老鹰";
            case 11:
                return isPingYin ? "shizi" : "狮子";
            case 0:
                return isPingYin ? "tongchi" : "通吃";
            case 13:
                return isPingYin ? "jinsha" : "金鲨";
            case 12:
                return isPingYin ? "tongpei" : "通赔";
            case 2:
                return isPingYin ? "feiqin" : "飞禽";
            case 3:
                return isPingYin ? "zoushou" : "走兽";
        }

        return null;
    }
}
