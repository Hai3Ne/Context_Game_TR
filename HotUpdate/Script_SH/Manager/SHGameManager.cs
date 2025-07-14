using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHGameManager {
    public static long UserMaxGold;//用户下注最大下注额度
    public static long AreaMaxGold;//每个区域最大下注额度
    public static long ApplyMinGold;//申请擂主最小乐豆
    public static bool IsSysLeiZhu;//系统是否坐庄

    public static ushort LeiZhuSeat;//擂主座位号
    public static long LeiZhuGold;//擂主当前乐豆
    public static long LeiZhuTotalResult;//擂主总输赢
    public static int LeiZhuTimes;//擂主守擂次数
    public static long SelfTotalResult;//当前玩家总输赢

    public static long CurSelectBet;//当前选择筹码
    public static int CurResultIndex;//当前选择索引
    public static long CurLeiZhuResult;//擂主当局输赢
    public static long CurSelfResult;//玩家当前输赢
    public static long CurTotalGold;//当局已下注金额
    public static long CurRevenue;//当局游戏税收
    public static long[] CurAllBet = new long[SHGameConfig.MaxAreaCount];//所有区域下注
    public static long[] CurSelfBet = new long[SHGameConfig.MaxAreaCount];//用户自己下注

    public static string RoomeName;//房间简称
    public static string RoomTotalName;//房间全称

    public static bool IsFlyChouMa = false;

    /// <summary>
    /// 当前鼠标点击的位置
    /// </summary>
    public static Vector3 curMousePos = Vector3.zero;
    
    public static long CurGold {//用户当前金币，扣除已经下注的
        get {
            return RoleManager.Self.GoldNum - SHGameManager.CurTotalGold;
        }
    }

    public static void PreCalcResult() {//本地预先计算结果
        RoleManager.Self.AddGold(SHGameManager.CurSelfResult - SHGameManager.CurRevenue);
    }

    public static long GetCurBetMax() {//获取当前可下注最大金额
        long gold = 0;
        foreach (var item in SHGameManager.CurAllBet) {
            if (gold < SHGameManager.AreaMaxGold - item) {
                gold = SHGameManager.AreaMaxGold - item;
            }
        }
        if (gold > SHGameManager.UserMaxGold - SHGameManager.CurTotalGold) {
            gold = SHGameManager.UserMaxGold - SHGameManager.CurTotalGold;
        }
        return gold;
    }

    public static void HandleGameScene(UI_Shenhua ui, NetCmdPack pack) {//游戏初始化
        RoleInfo leizhu = null;
        uint chip = 0;
        SHEnumGameState state = SHEnumGameState.Wait;
        float downcount = 0;
        switch (GameManager.CurGameState) {
            case GameState.GAME_STATUS_FREE://空闲状态
                CMD_S_StatusFree_sszp free = pack.ToObj<CMD_S_StatusFree_sszp>();
                leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID,free.BankerUser);
                chip = free.ChipInfor;
                state = SHEnumGameState.Wait;
                downcount = free.TimeLeave;
                SHGameManager.CurTotalGold = 0;
                for (int i = 0; i < SHGameConfig.MaxAreaCount; i++) {
                    SHGameManager.CurAllBet[i] = 0;
                    SHGameManager.CurSelfBet[i] = 0;
                }

                SHGameManager.IsSysLeiZhu = free.EnableSysBanker;
                SHGameManager.LeiZhuSeat = free.BankerUser;
                SHGameManager.LeiZhuGold = free.BankerScore;
                SHGameManager.LeiZhuTotalResult = free.BankerWinScore;
                SHGameManager.LeiZhuTimes = free.BankerTime;
                SHGameManager.CurResultIndex = 0;
                SHGameManager.SelfTotalResult = 0;
                SHGameManager.UserMaxGold = free.UserMaxScore;
                SHGameManager.AreaMaxGold = free.AreaLimitScore;
                SHGameManager.ApplyMinGold = free.ApplyBankerCondition;
                SHGameManager.RoomeName = free.GameRoomName;
                SHGameManager.RoomTotalName = free.RoomTotalName;
                SHGameManager.CurLeiZhuResult = 0;
                SHGameManager.CurSelfResult = 0;
                SHGameManager.CurRevenue = 0;
                break;
            case GameState.SG_GAME_STATE_PLAY://下注状态
            case GameState.SH_GAME_STATE_GAME_END://结束状态
            case GameState.SH_GAME_STATE_MOVECARD_END://转盘结束
                CMD_S_StatusPlay_sszp play = pack.ToObj<CMD_S_StatusPlay_sszp>();
                leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, play.BankerUser);
                chip = play.ChipInfor;
                if (play.GameStatus == 100) {
                    state = SHEnumGameState.Bet;
                } else if (play.GameStatus == 101) {
                    state = SHEnumGameState.Result;
                }
                downcount = play.TimeLeave;
                SHGameManager.CurTotalGold = 0;
                for (int i = 0; i < SHGameConfig.MaxAreaCount; i++) {
                    SHGameManager.CurAllBet[i] = play.AllJettonScore[i + 1];
                    SHGameManager.CurSelfBet[i] = play.UserJettonScore[i + 1];
                    SHGameManager.CurTotalGold += SHGameManager.CurSelfBet[i];
                }

                SHGameManager.IsSysLeiZhu = play.EnableSysBanker;
                SHGameManager.LeiZhuSeat = play.BankerUser;
                SHGameManager.LeiZhuGold = play.BankerScore;
                SHGameManager.LeiZhuTotalResult = play.BankerWinScore;
                SHGameManager.LeiZhuTimes = play.BankerTime;
                SHGameManager.CurResultIndex = play.TableCard - 1;
                SHGameManager.UserMaxGold = play.UserMaxScore;
                SHGameManager.AreaMaxGold = play.AreaLimitScore;
                SHGameManager.ApplyMinGold = play.ApplyBankerCondition;
                SHGameManager.RoomeName = play.GameRoomName;
                SHGameManager.RoomTotalName = play.RoomTotalName;
                SHGameManager.CurLeiZhuResult = play.EndBankerScore;
                SHGameManager.CurSelfResult = play.EndUserScore;
                SHGameManager.SelfTotalResult = 0;
                SHGameManager.CurRevenue = play.EndRevenue;
                break;
            default:
                LogMgr.LogError("当前状态错误");
                break;
        }
        ui.InitData(leizhu, null, SHGameManager.LeiZhuTotalResult, SHGameManager.LeiZhuTimes, chip, state, downcount, SHGameManager.CurAllBet, SHGameManager.CurSelfBet);
        if (state == SHEnumGameState.Result)
        {
            ui.item_zhuanpan.SetResult(downcount, SHGameManager.CurResultIndex, SHGameManager.CurLeiZhuResult, SHGameManager.CurSelfResult);
            if (ui.item_zhuanpan.mRotateStep <= 3)
            {
                ui.item_history.ShowLast();
            }
        }
    }
    public static void HandleGameFree(CMD_S_GameFree_sszp game_free) {//游戏进入等待状态
        SHGameManager.CurTotalGold = 0;
        SHGameManager.CurRevenue = 0;
        for (int i = 0; i < SHGameConfig.MaxAreaCount; i++) {
            SHGameManager.CurAllBet[i] = 0;
            SHGameManager.CurSelfBet[i] = 0;
        }
        GameManager.SetGameState(GameState.GAME_STATUS_FREE);
    }
    public static void HandleGameStart(CMD_S_GameStart_sszp game_start) {//游戏开始
        SHGameManager.CurTotalGold = 0;
        SHGameManager.CurRevenue = 0;
        for (int i = 0; i < SHGameConfig.MaxAreaCount; i++) {
            SHGameManager.CurAllBet[i] = 0;
            SHGameManager.CurSelfBet[i] = 0;
        }
        SHGameManager.LeiZhuSeat = game_start.BankerUser;
        SHGameManager.LeiZhuGold = game_start.BankerScore;
        SHGameManager.UserMaxGold = game_start.UserMaxScore;
        GameManager.SetGameState(GameState.SG_GAME_STATE_PLAY);
    }
    public static void HandleGameEnd(CMD_S_GameEnd_sszp game_end) {//游戏结束
        SHGameManager.LeiZhuGold = game_end.BankerCurScore;
        SHGameManager.CurResultIndex = game_end.TableCard - 1;
        SHGameManager.CurLeiZhuResult = game_end.BankerScore;
        SHGameManager.LeiZhuTotalResult = game_end.BankerTotallScore;
        SHGameManager.CurSelfResult = game_end.UserScore;
        SHGameManager.SelfTotalResult += SHGameManager.CurSelfResult;
        SHGameManager.LeiZhuTimes = game_end.BankerTime;
        SHGameManager.CurRevenue = game_end.Revenue;
        GameManager.SetGameState(GameState.SH_GAME_STATE_GAME_END);
    }
    public static void HandlePlaceJetton(CMD_S_PlaceJetton_sszp cmd) {//用户下注  区域下标从1开始
        if (cmd.ChairID == RoleManager.Self.ChairSeat) {
            SHGameManager.CurSelfBet[cmd.JettonArea-1] += cmd.JettonScore;
        }
        SHGameManager.CurAllBet[cmd.JettonArea-1] += cmd.JettonScore;
        SHGameManager.CurTotalGold += cmd.JettonScore;
    }
    public static void HandleChangeLeiZhu(CMD_S_ChangeBanker_sszp cmd) {//更改擂主
        SHGameManager.LeiZhuSeat = cmd.BankerUser;
        SHGameManager.LeiZhuGold = cmd.BankerScore;
        SHGameManager.LeiZhuTotalResult = 0;
        SHGameManager.LeiZhuTimes = 0;
    }
    public static void HandlePlaceJettonBroad(CMD_S_PlaceJettonBroad_sszp cmd) {//广播所有下注
        for (int i = 0; i < SHGameConfig.MaxAreaCount; i++) {
            SHGameManager.CurAllBet[i] += cmd.JettonScore[i];
        }
    }
}
