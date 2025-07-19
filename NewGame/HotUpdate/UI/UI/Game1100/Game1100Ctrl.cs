using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1100Ctrl : Singleton<Game1100Ctrl>
    {


        public void Send_CS_GAME7_BET_REQ(int nBet)
        {
            CS_GAME7_BET_REQ data = new CS_GAME7_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME7_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME7_ROOM_INFO)]
        public void OnEnterRoomRet_1100(MsgData msgData)
        {
            SC_GAME7_ROOM_INFO data = (SC_GAME7_ROOM_INFO)msgData;
            Game1100Model.Instance.Init(data);
            //Debug.LogError("房间信息===============");
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME7_BET_RET)]
        public void OnGAME7_BET_RET(MsgData msgData)
        {
            SC_GAME7_BET_RET data = (SC_GAME7_BET_RET)msgData;
            Game1100Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet1100);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }


        //[NetResponse(NetMsgDef.SC_GAME_BROADCAST_JACKPOT)]
        //public void GAME1_BROADCAST_JACKPOT(MsgData msgData)
        //{
        //    SC_GAME2_BROADCAST_JACKPOT data = (SC_GAME2_BROADCAST_JACKPOT)msgData;
        //    Game1100Model.Instance.n64Jackpot = data.n64Jackpot;
        //    Message.Broadcast(MessageName.GE_BroadCast_Jackpot1100);
        //}

        [NetResponse(NetMsgDef.SC_GAME7_BROADCAST_ADD_AWARD)]
        public void GAME7_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME7_BROADCAST_ADD_AWARD data = (SC_GAME7_BROADCAST_ADD_AWARD)msgData;
            SGame7AwardInfo jackpotList = data.info[0];
            if (Game1100Model.Instance.arrayAward.Count >= 10)
                Game1100Model.Instance.arrayAward.RemoveAt(9);

            SGame7AwardInfo temp = new SGame7AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.nRate = jackpotList.nRate;
            temp.szName = jackpotList.szName;
            Game1100Model.Instance.arrayAward.Insert(0, temp);

            Message.Broadcast(MessageName.GE_BroadCast_JackpotList1100);
        }

        #endregion

    }
}
