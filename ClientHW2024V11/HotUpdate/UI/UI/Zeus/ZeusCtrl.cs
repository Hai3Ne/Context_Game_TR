using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate {
    [NetHandler]
    public class ZeusCtrl : Singleton<ZeusCtrl>
    {
        #region 请求
        public void GameBetSend(int nBet)
        {
            CS_GAME4_BET_REQ data = new CS_GAME4_BET_REQ();
            
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME4_BET_REQ, data);
        }
        #endregion

        #region 接收
        [NetResponse(NetMsgDef.SC_GAME4_BET_RET)]
        public void GameBetRecevied(MsgData msgData)
        {
            SC_GAME4_BET_RET data =  (SC_GAME4_BET_RET)msgData;
            ZeusModel.Instance.setGameInfo(data);
        }

        [NetResponse(NetMsgDef.SC_GAME4_BROADCAST_JACKPOT)]
        public void BroadCastRecevied(MsgData msgData)
        {
            SC_GAME4_BROADCAST_JACKPOT data = (SC_GAME4_BROADCAST_JACKPOT)msgData;
            ZeusModel.Instance.JackpotNum = data.n64Jackpot;
            Message.Broadcast(MessageName.GAME_ZEUS_RELOADUI);
        }

        [NetResponse(NetMsgDef.SC_GAME4_BROADCAST_ADD_AWARD)]
        public void BroadCastAddRecevied(MsgData msgData)
        {
            SC_GAME4_BROADCAST_ADD_AWARD data = (SC_GAME4_BROADCAST_ADD_AWARD)msgData;
            ZeusModel.Instance.AddRoomInfoList(data.info);
            Message.Broadcast(MessageName.GAME_ZEUS_UPDATEJACKPOT);
        }

        [NetResponse(NetMsgDef.SC_GAME4_ROOM_INFO)]
        public void RoomInfoRecevied(MsgData msgData)
        {
            SC_GAME4_ROOM_INFO data = (SC_GAME4_ROOM_INFO)msgData;
            ZeusModel.Instance.setRoomData(data);
            Message.Broadcast(MessageName.GAME_ZEUS_UPDATEJACKPOT);
            Message.Broadcast(MessageName.GAME_ZEUS_RELOADUI);
        }
        #endregion
    }
}


