using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WZQGameManager {
    public static void HandleGameScene(NetCmdPack pack) {//游戏初始化
        switch (GameManager.CurGameState) {
            case GameState.GAME_STATUS_FREE://空闲状态
                CMD_S_StatusFree state_free = pack.ToObj<CMD_S_StatusFree>();
                WZQTableManager.FristHandSeat = state_free.BlackUser;
                WZQTableManager.MaxPayMoney = state_free.MaxMoney;
                WZQTableManager.SetCurRoleSeat(ushort.MaxValue);
                break;
            case GameState.GAME_STATUS_PLAY://游戏状态
                CMD_S_StatusPlay state_play = pack.ToObj<CMD_S_StatusPlay>();
                WZQTableManager.TotalTime = state_play.GameClock;//一局总时间
                WZQTableManager.FristHandSeat = state_play.BlackUser;//黑子座位号
                WZQTableManager.LeftTime = state_play.LeftClock;//剩余时间
                WZQTableManager.SetCurRoleSeat(state_play.CurrentUser);//当前执子玩家座位号
                //state_play.BegStatus;//请求状态
                WZQTableManager.MaxPayMoney = state_play.MaxMoney;
                EventManager.Notifiy(GameEvent.BegStatus, state_play.BegStatus);
                break;
            default:
                LogMgr.LogError("当前状态错误");
                break;
        }
    }
    public static void HandleGameStart(CMD_S_GameStart s_game_start) {//游戏开始
        WZQTableManager.TotalTime = s_game_start.GameClock;//一局总时间
        WZQTableManager.LeftTime = new ushort[2];//剩余时间
        WZQTableManager.LeftTime[0] = WZQTableManager.TotalTime;
        WZQTableManager.LeftTime[1] = WZQTableManager.TotalTime;
        WZQTableManager.FristHandSeat = s_game_start.BlackUser;//黑子座位号
        WZQTableManager.SetCurRoleSeat(WZQTableManager.FristHandSeat);
        GameManager.SetGameState(GameState.GAME_STATUS_PLAY);
    }
    public static void HandlePlaceChess(CMD_S_PlaceChess s_place_chess) {//放置旗子
        WZQTableManager.LeftTime = s_place_chess.LeftClock;//剩余时间
        WZQTableManager.SetCurRoleSeat(s_place_chess.CurrentUser);
    }

}
