using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    [NetHandler]
    public class FortyTwoGridCtrl : Singleton<FortyTwoGridCtrl>
    {
        #region 请求
        public void GameBetSend(int nBet)
        {
            CS_GAME15_BET_REQ data = new CS_GAME15_BET_REQ();

            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME15_BET_REQ, data);
        }
        #endregion

        #region 接收
        [NetResponse(NetMsgDef.SC_GAME15_BET_RET)]
        public void GameBetRecevied(MsgData msgData)
        {
            SC_GAME15_BET_RET data = (SC_GAME15_BET_RET)msgData;
            FortyTwoGridModel.Instance.setGameInfo(data);
        }

        [NetResponse(NetMsgDef.SC_GAME15_BROADCAST_JACKPOT)]
        public void BroadCastRecevied(MsgData msgData)
        {
            SC_GAME15_BROADCAST_JACKPOT data = (SC_GAME15_BROADCAST_JACKPOT)msgData;
            FortyTwoGridModel.Instance.JackpotNum = data.n64Jackpot;
            Message.Broadcast(MessageName.GAME_ZEUS_RELOADUI);
        }

        [NetResponse(NetMsgDef.SC_GAME15_BROADCAST_ADD_AWARD)]
        public void BroadCastAddRecevied(MsgData msgData)
        {
            SC_GAME15_BROADCAST_ADD_AWARD data = (SC_GAME15_BROADCAST_ADD_AWARD)msgData;
            FortyTwoGridModel.Instance.AddRoomInfoList(data.info);
            Message.Broadcast(MessageName.GAME_ZEUS_RELOADUI);
        }

        [NetResponse(NetMsgDef.SC_GAME15_ROOM_INFO)]
        public void RoomInfoRecevied(MsgData msgData)
        {
            SC_GAME15_ROOM_INFO data = (SC_GAME15_ROOM_INFO)msgData;
            FortyTwoGridModel.Instance.setRoomData(data);
            Message.Broadcast(MessageName.GAME_ZEUS_RELOADUI);
        }
        #endregion
    }
}


