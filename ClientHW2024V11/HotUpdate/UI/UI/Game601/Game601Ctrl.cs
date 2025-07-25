using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game601Ctrl : Singleton<Game601Ctrl>
    {
        public void Send_CS_GAME6_BET_REQ(int nBet)
        {
            CS_GAME6_BET_REQ data = new CS_GAME6_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME6_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME6_ROOM_INFO)]
        public void OnEnterRoomRet_601(MsgData msgData)
        {
            SC_GAME6_ROOM_INFO data = (SC_GAME6_ROOM_INFO)msgData;
            Game601Model.Instance.Init(data);
            //Debug.LogError("房间信息===============");
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME6_BET_RET)]
        public void OnGAME2_BET_RET(MsgData msgData)
        {
            SC_GAME6_BET_RET data = (SC_GAME6_BET_RET)msgData;
            Game601Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet600);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }


        [NetResponse(NetMsgDef.SC_GAME6_BROADCAST_JACKPOT)]
        public void GAME6_BROADCAST_JACKPOT(MsgData msgData)
        {
            SC_GAME6_BROADCAST_JACKPOT data = (SC_GAME6_BROADCAST_JACKPOT)msgData;
            Game601Model.Instance.n64Jackpot = data.n64Jackpot;
            Message.Broadcast(MessageName.GE_BroadCast_Jackpot600);
        }

        [NetResponse(NetMsgDef.SC_GAME6_BROADCAST_ADD_AWARD)]
        public void GAME6_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME6_BROADCAST_ADD_AWARD data = (SC_GAME6_BROADCAST_ADD_AWARD)msgData;
            SGame6AwardInfo jackpotList = data.info[0];
            if (Game601Model.Instance.arrayAward.Count >= 10)
                Game601Model.Instance.arrayAward.RemoveAt(9);

            SGame6AwardInfo temp = new SGame6AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            Game601Model.Instance.arrayAward.Insert(0, temp);

            Message.Broadcast(MessageName.GE_BroadCast_JackpotList600);
        }

        #endregion




    }
}
