using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1400Ctrl : Singleton<Game1400Ctrl>
    {
        // Start is called before the first frame update


        public void ReqSendGAME12_BET_REQ(int nBet,int area)
        {
            CS_GAME12_BET_REQ data = new CS_GAME12_BET_REQ();
            data.nBet = nBet;
            data.ucArea = (sbyte)area;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME12_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME12_ROOM_INFO)]
        public void OnGAME12_ROOM_INFO(MsgData msgData)
        {
            SC_GAME12_ROOM_INFO data = (SC_GAME12_ROOM_INFO)msgData;
            Game1400Model.Instance.InitRoomInfo(data);
            Message.Broadcast(MessageName.GE_RoomInfo1400);
        }

        [NetResponse(NetMsgDef.SC_GAME12_BET_RET)]
        public void OnGAME12_BET_RET(MsgData msgData)
        {
           
            SC_GAME12_BET_RET data = (SC_GAME12_BET_RET)msgData;
            Game1400Model.Instance.SetGAME12_BET_RET(data);
            Message.Broadcast(MessageName.GE_BetGameRet1400);
        }

        [NetResponse(NetMsgDef.SC_GAME12_BROADCAST_BET)]
        public void OnGAME12_BROADCAST_BET(MsgData msgData)
        {
            SC_GAME12_BROADCAST_BET data = (SC_GAME12_BROADCAST_BET)msgData;
            Game1400Model.Instance.SetGAME12_BET_RET(data);
     
        }

        [NetResponse(NetMsgDef.SC_GAME12_BROADCAST_ADD_PLAYER)]
        public void OnGAME12_BROADCAST_ADD_PLAYER(MsgData msgData)
        {
            SC_GAME12_BROADCAST_ADD_PLAYER data = (SC_GAME12_BROADCAST_ADD_PLAYER)msgData;
            Game1400Model.Instance.BroadCastAddPlayer(data);
            Message.Broadcast(MessageName.GE_BroadCastAddPlayer1400);
        }

        [NetResponse(NetMsgDef.SC_GAME12_BROADCAST_DEL_PLAYER)]
        public void OnGAME12_BROADCAST_DEL_PLAYER(MsgData msgData)
        {
            SC_GAME12_BROADCAST_DEL_PLAYER data = (SC_GAME12_BROADCAST_DEL_PLAYER)msgData;
            Game1400Model.Instance.BroadCastDelPlayer(data);
            Message.Broadcast(MessageName.GE_BROADCAST_DEL_PLAYER1400);
        }

        [NetResponse(NetMsgDef.SC_GAME12_BROADCAST_BET_START)]
        public void OnGAME12_BROADCAST_BET_START(MsgData msgData)
        {
            SC_GAME12_BROADCAST_BET_START data = (SC_GAME12_BROADCAST_BET_START)msgData;
            Game1400Model.Instance.OnGAME12_BROADCAST_BET_START(data);
            Message.Broadcast(MessageName.GE_NoticeGameStart1400);
        }

        [NetResponse(NetMsgDef.SC_GAME12_BROADCAST_BET_END)]
        public void OnGAME12_BROADCAST_BET_END(MsgData msgData)
        {
            SC_GAME12_BROADCAST_BET_END data = (SC_GAME12_BROADCAST_BET_END)msgData;
            Game1400Model.Instance.OnGAME12_BROADCAST_BET_END(data);
            Message.Broadcast(MessageName.GE_NoticeBetEnd1400);
        }

        [NetResponse(NetMsgDef.SC_GAME12_CALCULATE)]
        public void OnGAME12_CALCULATE(MsgData msgData)
        {
            SC_GAME12_CALCULATE data = (SC_GAME12_CALCULATE)msgData;
            Game1400Model.Instance.OnGAME12_CALCULATE(data);
        }
        #endregion
    }
}
