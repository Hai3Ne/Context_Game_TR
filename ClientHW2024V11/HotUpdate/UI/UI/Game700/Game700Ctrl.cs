using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game700Ctrl : Singleton<Game700Ctrl>
    {
        //public void ReqBoxAward(int nBet)
        //{
        //    CS_GAME3_AWARD_BOX_REQ data = new CS_GAME3_AWARD_BOX_REQ();
        //    data.nBet = nBet;
        //    NetMgr.netMgr.send(NetMsgDef.CS_GAME3_AWARD_BOX_REQ, data);
        //}
        public void Send_CS_GAME3_BET_REQ(int nBet)
        {
            GAME2_BET_REQ data = new GAME2_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME2_BET_REQ, data);
        }
        #region



        [NetResponse(NetMsgDef.SC_GAME2_ROOM_INFO)]
        public void OnEnterRoomRet_600(MsgData msgData)
        {
            //Debug.LogError("===============");
            SC_GAME2_ROOM_INFO data = (SC_GAME2_ROOM_INFO)msgData;
            Game700Model.Instance.Init(data);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME2_BET_RET)]
        public void OnGAME3_BET_RE_700(MsgData msgData)
        {
            SC_GAME2_BET_RET data = (SC_GAME2_BET_RET)msgData;
            Game700Model.Instance.SetSpinData(data);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet700, null);
            //Game700Model.Instance.Init(data);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        //[NetResponse(NetMsgDef.SC_GAME3_AWARD_BOX_RET)]
        //public void OnGAME3_AWARD_BOX_RET(MsgData msgData)
        //{
        //    SC_GAME3_AWARD_BOX_RET data = (SC_GAME3_AWARD_BOX_RET)msgData;
        //    Game700Model.Instance.OnAWARD_BOX_700(data);
        //    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GAME3_AWARD_BOX_700, null);
        //    //Game700Model.Instance.Init(data);
        //    // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        //}

        [NetResponse(NetMsgDef.SC_GAME2_BROADCAST_JACKPOT)]
        public void GAME1_BROADCAST_JACKPOT(MsgData msgData)
        {
            SC_GAME2_BROADCAST_JACKPOT data = (SC_GAME2_BROADCAST_JACKPOT)msgData;
            Game700Model.Instance.n64Jackpot = data.n64Jackpot;
            Message.Broadcast(MessageName.GE_BroadCast_Jackpot700);
        }

        [NetResponse(NetMsgDef.SC_GAME2_BROADCAST_ADD_AWARD)]
        public void GAME2_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME2_BROADCAST_ADD_AWARD data = (SC_GAME2_BROADCAST_ADD_AWARD)msgData;
            SGame2AwardInfo jackpotList = data.info[0];
            if (Game700Model.Instance.arrayAward.Count >= 10)
                Game700Model.Instance.arrayAward.RemoveAt(9);

            SGame2AwardInfo temp = new SGame2AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            Game700Model.Instance.arrayAward.Insert(0, temp);

            Message.Broadcast(MessageName.GE_BroadCast_JackpotList700);
        }
        #endregion
    }
}
