using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate {
    [NetHandler]
    public class WealthCtrl : Singleton<WealthCtrl>
    {
        public void SendRankInfo()
        {
            CS_TASK_GOLDRANK_REQ data = new CS_TASK_GOLDRANK_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_GOLDRANK_REQ, data);
        }

        [NetResponse(NetMsgDef.SC_TASK_GOLDRANK_INFO)]
        public void GetRankInfoResult(MsgData msgData)
        {
            SC_TASK_GOLDRANK_INFO data = msgData as SC_TASK_GOLDRANK_INFO;

            WealthModel.Instance.GetRankData(data);
            Message.Broadcast(MessageName.RELOAD_WEALTH_UI);
        }
    }
}


