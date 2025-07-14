using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WZQNetHandle {
    public static void Handle(NetCmdPack pack) {
        NetCmdType cmdType = (NetCmdType)pack.cmdTypeId;
        //Debug.LogError("resp <-------- cmdType:" + cmdType);
        switch (cmdType) {
            case NetCmdType.SUB_GR_USER_ENTER://用户进入
                WZQTableManager.HandleUserEnter(pack.ToObj<SC_UserEnter>());
                break;
            case NetCmdType.SUB_GR_USER_STATUS://用户当前状态更新
                WZQTableManager.HandleUserState(pack.ToObj<SC_GR_UserStatus>());
                break;

            //case NetCmdType.SUB_GF_GAME_SCENE_FREE://空闲状态下同步数据
            case NetCmdType.SUB_GF_GAME_SCENE_PLAY://游戏状态下同步数据
                WZQGameManager.HandleGameScene(pack);
                break;
                
            //case NetCmdType.SUB_GR_TABLE_STATUS://桌子状态
            //    //CMD_GR_TableStatus table_state = pack.ToObj<CMD_GR_TableStatus>();
            //    //Debug.LogError(LitJson.JsonMapper.ToJson(table_state));
            //    break;
            //case NetCmdType.SUB_S_TABLE_PASS://桌子密码
            //    //CMD_S_Pass table_pass = pack.ToObj<CMD_S_Pass>();
            //    //Debug.LogError(LitJson.JsonMapper.ToJson(table_pass));
            //    break;


            case NetCmdType.SUB_S_GAME_START://游戏开始
                WZQGameManager.HandleGameStart(pack.ToObj<CMD_S_GameStart>());
                break;
            case NetCmdType.SUB_S_PLACE_CHESS://CMD_S_PlaceChess" Main="MDM_GF_GAME" Sub="SUB_S_PLACE_CHESS" ID="101" desc="放置棋子">
                WZQGameManager.HandlePlaceChess(pack.ToObj<CMD_S_PlaceChess>());
                break;
            case NetCmdType.SUB_S_GAME_END://CMD_S_GameEnd" Main="MDM_GF_GAME" Sub="SUB_S_GAME_END" ID="108" desc="游戏结束">
                GameManager.SetGameState(GameState.GAME_STATUS_FREE);
                break;
            case NetCmdType.SUB_S_REGRET_RESULT://CMD_S_RegretResult" Main="MDM_GF_GAME" Sub="SUB_S_REGRET_RESULT" ID="104" desc="悔棋结果">
                WZQTableManager.SetCurRoleSeat(pack.ToObj<CMD_S_RegretResult>().RegretUser);
                break;
            case NetCmdType.SUB_S_BLACK_TRADE://CMD_S_BLACK_TRADE" Main="MDM_GF_GAME" Sub="SUB_S_BLACK_TRADE" ID="107" desc="交换对家">
                if (WZQTableManager.FristHandSeat == 0) {
                    WZQTableManager.FristHandSeat = 1;
                } else {
                    WZQTableManager.FristHandSeat = 0;
                }
                break;
            //case NetCmdType.SUB_S_REGRET_REQ://CMD_S_REGRET" Main="MDM_GF_GAME" Sub="SUB_S_REGRET_REQ" ID="102" desc="悔棋请求">
            //case NetCmdType.SUB_S_REGRET_FAILE://CMD_S_RegretFaile" Main="MDM_GF_GAME" Sub="SUB_S_REGRET_FAILE" ID="103" desc="悔棋失败">
            //case NetCmdType.SUB_S_PEACE_REQ://CMD_S_PEACE_REQ" Main="MDM_GF_GAME" Sub="SUB_S_PEACE_REQ" ID="105" desc="和棋请求">
            //case NetCmdType.SUB_S_PEACE_ANSWER://CMD_S_PEACE_ANSWER" Main="MDM_GF_GAME" Sub="SUB_S_PEACE_ANSWER" ID="106" desc="和棋应答">
            //case NetCmdType.SUB_S_KICK_FLAG://CMD_S_GAME_KICK_FLAG" Main="MDM_GF_GAME" Sub="SUB_S_KICK_FLAG" ID="120" desc="踢人按钮点亮">
            //case NetCmdType.SUB_S_CHESS_MANUAL://CMD_S_CHESS_MANUAL" Main="MDM_GF_GAME" Sub="SUB_S_CHESS_MANUAL" ID="109" desc="棋谱信息">
            //    break;
        }
    }
}
