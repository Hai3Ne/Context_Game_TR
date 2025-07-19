using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game800Ctrl : Singleton<Game800Ctrl>
    {
        public void Send_CS_GAME5_BET_REQ(int nBet)
        {
            CS_GAME5_BET_REQ data = new CS_GAME5_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME5_BET_REQ, data);
        }
        #region



        [NetResponse(NetMsgDef.SC_GAME5_ROOM_INFO)]
        public void OnEnterRoomRet_800(MsgData msgData)
        {
            SC_GAME5_ROOM_INFO data = (SC_GAME5_ROOM_INFO)msgData;
            Game800Model.Instance.Init(data);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME5_BET_RET)]
        public void OnGAME3_BET_RE_800(MsgData msgData)
        {
            SC_GAME5_BET_RET data = (SC_GAME5_BET_RET)msgData;
            Game800Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.BetGameRet800);
            //Game800Model.Instance.Init(data);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME5_BROADCAST_JACKPOT)]
        public void OnBROADCAST_JACKPOT(MsgData msgData)
        {
            SC_GAME5_BROADCAST_JACKPOT data = (SC_GAME5_BROADCAST_JACKPOT)msgData;
            Game800Model.Instance.n64Jackpot = data.n64Jackpot;
            Message.Broadcast(MessageName.BroadCast_Jackpot80);
        }

        [NetResponse(NetMsgDef.SC_GAME5_BROADCAST_ADD_AWARD)]
        public void OnBROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME5_BROADCAST_ADD_AWARD data = (SC_GAME5_BROADCAST_ADD_AWARD)msgData;
            SGame5AwardInfo jackpotList = data.info[0];
            if (Game500Model.Instance.arrayAward.Count >= 10)
                Game500Model.Instance.arrayAward.RemoveAt(9);

            SGame5AwardInfo temp = new SGame5AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            Game800Model.Instance.arrayAward.Insert(0, temp);

            Message.Broadcast(MessageName.BroadCast_JackpotList800);
        }
        #endregion
    }
}
