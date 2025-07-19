using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1000Ctrl : Singleton<Game1000Ctrl>
    {


        public void Send_CS_GAME11_BET_REQ(int nBet)
        {
            CS_GAME11_BET_REQ data = new CS_GAME11_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME11_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME11_ROOM_INFO)]
        public void OnEnterRoomRet_1000(MsgData msgData)
        {
            SC_GAME11_ROOM_INFO data = (SC_GAME11_ROOM_INFO)msgData;
            Game1000Model.Instance.Init(data);
        }

        [NetResponse(NetMsgDef.SC_GAME11_BET_RET)]
        public void OnGAME11_BET_RET(MsgData msgData)
        {
            SC_GAME11_BET_RET data = (SC_GAME11_BET_RET)msgData;
            Game1000Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet1000);
        }

        [NetResponse(NetMsgDef.SC_GAME11_BROADCAST_ADD_AWARD)]
        public void GAME11_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME11_BROADCAST_ADD_AWARD data = (SC_GAME11_BROADCAST_ADD_AWARD)msgData;
            SGame11AwardInfo jackpotList = data.info[0];
            if (Game1000Model.Instance.arrayAward.Count >= 10)
                Game1000Model.Instance.arrayAward.RemoveAt(9);

            SGame11AwardInfo temp = new SGame11AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            Game1000Model.Instance.arrayAward.Insert(0, temp);
            Message.Broadcast(MessageName.GE_BroadCast_JackpotList1000);
        }

        #endregion

    }
}
