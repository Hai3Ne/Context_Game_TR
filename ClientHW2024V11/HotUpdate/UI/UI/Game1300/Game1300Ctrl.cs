using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game1300Ctrl : Singleton<Game1300Ctrl>
    {


        public void Send_CS_GAME9_BET_REQ(int nBet)
        {
            CS_GAME9_BET_REQ data = new CS_GAME9_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME9_BET_REQ, data);
        }

        [NetResponse(NetMsgDef.SC_GAME9_BET_RET)]
        public void OnGAME8_BET_RET(MsgData msgData)
        {
            SC_GAME9_BET_RET data = (SC_GAME9_BET_RET)msgData;
            Game1300Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet1300);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

    }
}
