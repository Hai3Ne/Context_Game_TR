using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1600Ctrl : Singleton<Game1600Ctrl>
    {
        public void Send_CS_GAME14_BET_REQ(int nBet)
        {
            CS_GAME14_BET_REQ data = new CS_GAME14_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME14_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME14_ROOM_INFO)]
        public void OnEnterRoomRet_1600(MsgData msgData)
        {
            SC_GAME14_ROOM_INFO data = (SC_GAME14_ROOM_INFO)msgData;
            Game1600Model.Instance.Init(data);
        }

        [NetResponse(NetMsgDef.SC_GAME14_BET_RET)]
        public void OnGAME14_BET_RET(MsgData msgData)
        {
            SC_GAME14_BET_RET data = (SC_GAME14_BET_RET)msgData;
            Game1600Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet1600);
        }
        #endregion

    }
}
