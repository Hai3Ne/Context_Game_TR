using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1200Ctrl : Singleton<Game1200Ctrl>
    {
        public void Send_CS_GAME10_BET_REQ(int nBet)
        {
            CS_GAME10_BET_REQ data = new CS_GAME10_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME10_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME10_ROOM_INFO)]
        public void OnEnterRoomRet_1200(MsgData msgData)
        {
            SC_GAME10_ROOM_INFO data = (SC_GAME10_ROOM_INFO)msgData;
            Game1200Model.Instance.Init(data);
        }

        [NetResponse(NetMsgDef.SC_GAME10_BET_RET)]
        public void OnGAME10_BET_RET(MsgData msgData)
        {
            SC_GAME10_BET_RET data = (SC_GAME10_BET_RET)msgData;
            Game1200Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet1200);
        }
        #endregion

    }
}
