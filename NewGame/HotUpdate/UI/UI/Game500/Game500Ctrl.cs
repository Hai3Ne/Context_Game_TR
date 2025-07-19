using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game500Ctrl : Singleton<Game500Ctrl>
    {

        /// <summary>
        /// 请求500房间下注
        /// </summary>
        /// <param name="BET"></param>
        public void Send_CS_GAME1_BET_REQ(int bet)
        {
            CS_GAME1_BET_REQ data = new CS_GAME1_BET_REQ();
            data.nBet = bet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME1_BET_REQ, data);
        }


        #region

        [NetResponse(NetMsgDef.SC_GAME1_ROOM_INFO)]
        public void OnEnterRoomRet_600(MsgData msgData)
        {
            SC_GAME1_ROOM_INFO data = (SC_GAME1_ROOM_INFO)msgData;
            Game500Model.Instance.Init(data);
           // Debug.LogError("===============>>>");
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME1_BET_RET)]
        public void GameBetRecevied(MsgData msgData)
        {
            SC_GAME1_BET_RET data = (SC_GAME1_BET_RET)msgData;
            Game500Model.Instance.SetSpinData(data);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME1_BROADCAST_JACKPOT)]
        public void GAME1_BROADCAST_JACKPOT(MsgData msgData)
        {
            SC_GAME1_BROADCAST_JACKPOT data = (SC_GAME1_BROADCAST_JACKPOT)msgData;
            Game500Model.Instance.n64Jackpot = data.n64Jackpot;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.Ge_BROADCAST_JACKPOT, null);
        }

        [NetResponse(NetMsgDef.SC_GAME1_BROADCAST_ADD_AWARD)]
        public void GAME1_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME1_BROADCAST_ADD_AWARD data = (SC_GAME1_BROADCAST_ADD_AWARD)msgData;
            SGame1AwardInfo jackpotList = data.info[0];
            if (Game500Model.Instance.arrayAward.Count >= 10)
                Game500Model.Instance.arrayAward.RemoveAt(9);

            SGame1AwardInfo temp = new SGame1AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            Game500Model.Instance.arrayAward.Insert(0,temp);

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.Ge_BROADCAST_RefreshJACKPOT, null);
        }
        #endregion
    }
}
