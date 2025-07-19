using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game602Ctrl : Singleton<Game602Ctrl>
    {

        public void Send_CS_GAME19_BET_REQ(int nBet)
        {
            CS_GAME19_BET_REQ data = new CS_GAME19_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME19_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME19_ROOM_INFO)]
        public void OnEnterRoomRet_602(MsgData msgData)
        {
            SC_GAME19_ROOM_INFO data = (SC_GAME19_ROOM_INFO)msgData;
            Game602Model.Instance.Init(data);
        }

        [NetResponse(NetMsgDef.SC_GAME19_BET_RET)]
        public void OnGAME19_BET_RET(MsgData msgData)
        {
            SC_GAME19_BET_RET data = (SC_GAME19_BET_RET)msgData;
            Game602Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet602);
        }
    

        [NetResponse(NetMsgDef.SC_GAME19_BROADCAST_JACKPOT)]
        public void GAME19_BROADCAST_JACKPOT(MsgData msgData)
        {
            SC_GAME19_BROADCAST_JACKPOT data = (SC_GAME19_BROADCAST_JACKPOT)msgData;
            Game602Model.Instance.n64Jackpot = data.n64Jackpot;
            Message.Broadcast(MessageName.GE_BroadCast_Jackpot602);
        }

        [NetResponse(NetMsgDef.SC_GAME19_BROADCAST_ADD_AWARD)]
        public void GAME19_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME19_BROADCAST_ADD_AWARD data = (SC_GAME19_BROADCAST_ADD_AWARD)msgData;
            SGame19AwardInfo jackpotList = data.info[0];
            if (Game602Model.Instance.arrayAward.Count >= 10)
                Game602Model.Instance.arrayAward.RemoveAt(9);

            SGame19AwardInfo temp = new SGame19AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            Game602Model.Instance.arrayAward.Insert(0, temp);

            Message.Broadcast(MessageName.GE_BroadCast_JackpotList602);
        }

        #endregion

    }
}
