using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1500Ctrl : Singleton<Game1500Ctrl>
    {
        public void Send_CS_GAME13_BET_REQ(int nBet)
        {
            CS_GAME13_BET_REQ data = new CS_GAME13_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME13_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME13_ROOM_INFO)]
        public void OnEnterRoomRet_1500(MsgData msgData)
        {
            SC_GAME13_ROOM_INFO data = (SC_GAME13_ROOM_INFO)msgData;
            Game1500Model.Instance.Init(data);
        }

        [NetResponse(NetMsgDef.SC_GAME13_BET_RET)]
        public void OnGAME10_BET_RET(MsgData msgData)
        {;
            SC_GAME13_BET_RET data = (SC_GAME13_BET_RET)msgData;
            Game1500Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet1500);
        }
        #endregion

    }
}
